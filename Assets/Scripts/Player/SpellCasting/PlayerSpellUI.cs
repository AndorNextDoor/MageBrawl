using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpellUI : NetworkBehaviour
{
    [SerializeField] private List<GameObject> spells = new List<GameObject>();
    [SerializeField] private GameObject spellsHolder;
    [SerializeField] private GameObject spellPrefab;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            spellsHolder.SetActive(false);
            return;
        }
    }

    public void EquipSpell(SpellSO _spellSO)
    {
        if (!IsOwner)
            return;

          GameObject newSpell = Instantiate(spellPrefab, spellsHolder.transform);
        newSpell.transform.GetChild(1).GetComponent<Image>().sprite = _spellSO.icon;

        spells.Add(newSpell);
    }


    public void OnSpellCasted(int spellID, float duration)
    {
        if (!IsOwner)
            return;

        Animation _cooldownAnim = spells[spellID].transform.GetComponentInChildren<Animation>();

        _cooldownAnim["SpellCooldown"].speed = 1/duration;
        _cooldownAnim.Play();
        
    }

}
