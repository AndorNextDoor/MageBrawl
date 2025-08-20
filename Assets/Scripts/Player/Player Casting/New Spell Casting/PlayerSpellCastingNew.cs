using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using static PlayerSpellCastingAnimator;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

public interface ISpellBehaviour
{
    void Init(SpellSO data, Transform shootOrigin, ulong casterClientId);
    void OnCast();
}

public class PlayerSpellCastingNew : NetworkBehaviour
{
    [SerializeField] private Transform shootOrigin;
    [SerializeField] private SpellSO testSpell;

    private SpellSlot[] equippedSlots = new SpellSlot[4];
    private PlayerSpellUI _ui;

    public event Action<SpellCastAnimationType> OnSpellCasted;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _ui = GetComponent<PlayerSpellUI>();

        for (int i = 0; i < equippedSlots.Length; i++)
            equippedSlots[i] = new SpellSlot();

        //if (testSpell != null)
        //    EquipSpell(0, testSpell);
    }

    void InitSpell(int slotIndex, SpellSO spellData)
    {
        if (IsServer) return;

        var behaviour = equippedSlots[slotIndex].behaviour;
        

        equippedSlots[slotIndex].spellData = spellData;
    }


    // ========= CLIENT: Requests to equip spell =========
    public void EquipSpell(int slotIndex, SpellSO spellData)
    {
        if (!IsOwner || slotIndex < 0 || slotIndex >= equippedSlots.Length)
            return;

        EquipSpellLocal(slotIndex, spellData); // local setup for UI
        EquipSpellServerRpc(slotIndex, spellData.name, NetworkManager.LocalClientId); // tell server to spawn
    }

    // ========= CLIENT: Local setup (UI, visuals only) =========
    private void EquipSpellLocal(int slotIndex, SpellSO spellData)
    {
        equippedSlots[slotIndex].spellData = spellData;
        _ui?.EquipSpell(spellData);
    }

    // ========= SERVER: Actually spawn prefab & init =========
    [ServerRpc]
    private void EquipSpellServerRpc(int slotIndex, string spellName, ulong clientID)
    {
        SpellSO spellData = SpellDatabase.Instance.GetSpellByName(spellName);
        if (spellData == null) return;

        GameObject castGO = Instantiate(spellData.castingPrefab);
        var netObj = castGO.GetComponent<NetworkObject>();
        netObj.SpawnWithOwnership(OwnerClientId);

        var behaviour = castGO.GetComponent<ISpellBehaviour>();
        behaviour.Init(spellData, shootOrigin, OwnerClientId);

        equippedSlots[slotIndex] = new SpellSlot
        {
            spellData = spellData,
            behaviour = behaviour
        };

        // Tell client to also locally set up behaviour (no spawn)
        EquipSpellClientRpc(slotIndex, spellName, netObj.NetworkObjectId, clientID);
    }

    [ClientRpc]
    private void EquipSpellClientRpc(int slotIndex, string spellName, ulong spellObjectID, ulong targetClientID)
    {
        if (IsServer) return; // Server already did InitSpell

        SpellSO spellData = SpellDatabase.Instance.GetSpellByName(spellName);
        if (spellData == null) return;


        if (NetworkManager.LocalClientId != targetClientID || IsHost)
            return; // ✅ Host already added the spell on server

        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(spellObjectID, out var netObj))
        {
            ISpellBehaviour newSpell = netObj.GetComponent<ISpellBehaviour>();
            equippedSlots[slotIndex] = new SpellSlot
            {
                spellData = spellData,
                behaviour = newSpell
            };

            newSpell.Init(spellData, shootOrigin, targetClientID);
        }

        InitSpell(slotIndex, spellData); // only instantiate locally
    }

    // ========= CLIENT: Cast a spell =========
    public void CastSpell(int slotIndex)
    {
        if (!IsOwner || slotIndex < 0 || slotIndex >= equippedSlots.Length)
            return;

        var slot = equippedSlots[slotIndex];
        if (slot.spellData == null || slot.cooldown > 0f)
            return;

        slot.cooldown = slot.spellData.cooldown;
        equippedSlots[slotIndex] = slot;

        slot.behaviour?.OnCast();
        OnSpellCasted?.Invoke(slot.spellData.animationType);
        _ui?.OnSpellCasted(slotIndex, slot.spellData.cooldown);
    }

    // ========= Update: Reduce cooldown and check input =========
    void Update()
    {
        if (!IsOwner || !RoundManager.Instance.playersCanPlay.Value)
            return;


        for (int i = 0; i < equippedSlots.Length; i++)
        {
            if (equippedSlots[i].cooldown > 0f)
                equippedSlots[i].cooldown -= Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0)) CastSpell(0);
        if (Input.GetMouseButtonDown(1)) CastSpell(1);
        if (Input.GetKeyDown(KeyCode.LeftShift)) CastSpell(2);
        if (Input.GetKeyDown(KeyCode.Space)) CastSpell(3);
    }


    // ========= Spell Slot Class =========
    [System.Serializable]
    public class SpellSlot
    {
        public SpellSO spellData;
        public ISpellBehaviour behaviour;
        public float cooldown;
    }
}
