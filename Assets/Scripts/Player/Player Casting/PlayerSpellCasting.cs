using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpellCasting : NetworkBehaviour
{
    [SerializeField] private Transform shootPosition;
    [SerializeField] private List<ICast> playerSpells = new();
    private PlayerSpellUI _spellsUI;

    [SerializeField] private int maxSpells = 4;


    [SerializeField] private SpellSO testSpell;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
            return;

        _spellsUI = GetComponent<PlayerSpellUI>();
        EquipSpell(testSpell);
    }

    public void EquipSpell(SpellSO spellSO)
    {
        if (IsOwner)
        {
            EquipSpellServerRpc(NetworkManager.LocalClientId);
        }
    }

    public void EquipSpellServer(ulong clientId)
    {
        if (playerSpells.Count >= maxSpells)
            return;

        GameObject newSpell = Instantiate(testSpell.castingPrefab);
        NetworkObject netObj = newSpell.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId);

        var cast = newSpell.GetComponent<ICast>();
        if(_spellsUI == null)
        {
            _spellsUI = GetComponent<PlayerSpellUI>();
        }
        cast.SetStats(testSpell, shootPosition, _spellsUI, playerSpells.Count);

        // ✅ Only add to playerSpells if this is the actual player's object
        if (clientId == NetworkManager.ServerClientId)
        {
            // Server's own spell list
            playerSpells.Add(cast);
            _spellsUI.EquipSpell(testSpell);
        }

        AssignSpellClientRpc(netObj.NetworkObjectId, clientId);
    }




    [ServerRpc]
    public void EquipSpellServerRpc(ulong clientID)
    {
        EquipSpellServer(clientID);
    }

    [ClientRpc]
    private void AssignSpellClientRpc(ulong spellObjectId, ulong targetClientId)
    {
        if (NetworkManager.LocalClientId != targetClientId || IsHost)
            return; // ✅ Host already added the spell on server

        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(spellObjectId, out var netObj))
        {
            var cast = netObj.GetComponent<ICast>();
            cast.SetStats(testSpell, shootPosition, _spellsUI, playerSpells.Count);
            playerSpells.Add(cast);
            _spellsUI.EquipSpell(testSpell);
        }
    }


    public void CastSpell(int spellID)
    {
        if (spellID < 0 || spellID >= playerSpells.Count)
            return;

        playerSpells[spellID]?.Cast();
    }


    public void Update()
    {
        if (!IsOwner)
            return;

        // TO DO:
        // Create input manager and maybe enum dictionary
        if (Input.GetMouseButtonDown(0)) CastSpell(0); // 1-4 spells
        if (Input.GetMouseButtonDown(1)) CastSpell(1); 
        if (Input.GetKeyDown(KeyCode.LeftShift)) CastSpell(2);
        if (Input.GetKeyDown(KeyCode.Space)) CastSpell(3);
    }


}
