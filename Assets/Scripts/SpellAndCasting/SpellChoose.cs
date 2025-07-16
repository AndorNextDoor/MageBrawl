using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SpellChoose : NetworkBehaviour
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private Transform spellHolder;
    [SerializeField] private GameObject toDisableHolder;

    public void ShowRandomSkills(int amount)
    {
        if (!IsOwner) return;

        toDisableHolder.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // Clear old UI
        foreach (Transform child in spellHolder)
        {
            Destroy(child.gameObject);
        }

        int spellCount = SpellDatabase.Instance.SpellCount();

        if (amount > spellCount)
        {
            Debug.LogWarning("Requested more spells than available in the database.");
            amount = spellCount;
        }

        List<int> randomIndices = GenerateUniqueRandomIndices(amount, spellCount);

        foreach (int index in randomIndices)
        {
            SpellSO foundSpell = SpellDatabase.Instance.GetSpellByIndex(index);

            if (foundSpell == null)
            {
                Debug.LogWarning($"No spell found at index {index}");
                continue;
            }

            GameObject newSpellUI = Instantiate(spellPrefab, spellHolder);
            Button button = newSpellUI.GetComponentInChildren<Button>();

            if (button != null)
            {
                button.image.sprite = foundSpell.icon;

                // Local copy for lambda capture
                SpellSO spellToEquip = foundSpell;
                button.onClick.AddListener(() =>
                {
                    GetComponent<PlayerSpellCastingNew>()?.EquipSpell(0, spellToEquip);
                    toDisableHolder.SetActive(false);

                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                });
            }
        }
    }

    private List<int> GenerateUniqueRandomIndices(int count, int max)
    {
        HashSet<int> used = new();
        while (used.Count < count)
        {
            int value = Random.Range(0, max);
            used.Add(value);
        }

        return new List<int>(used);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            ShowRandomSkills(3);
        }
    }
}
