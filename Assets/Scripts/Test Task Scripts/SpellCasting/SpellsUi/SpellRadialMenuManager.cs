using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellRadialMenu : MonoBehaviour
{
    [SerializeField] private GameObject spellButtonPrefab;
    [SerializeField] private float radius = 100f;
    [SerializeField] private List<SpellData> spellDataList;
    [SerializeField] private RectTransform circlePanel;

    [SerializeField] private float verticalOffset;
    [SerializeField] private float horizontalOffset;

    private List<GameObject> spawnedIcons = new();
    private List<SpellRadialButton> activeButtons = new();

    [SerializeField] private PlayerSpellManager playerSpellManager;

    public void ShowMenu()
    {
        circlePanel.gameObject.SetActive(true);
        GenerateButtons();
    }

    public void HideMenu()
    {
        ClearButtons();
        circlePanel.gameObject.SetActive(false);
    }

    private void GenerateButtons()
    {
        ClearButtons();

        // It looks bad if it's lower than 6 objects gonna simulate more for it to look better
        int slotCount = 0;
        if (spellDataList.Count < 6)
        {
            slotCount = 6;
        }
        else
        {
            slotCount = spellDataList.Count;
        }

        float angleStep = 360f / slotCount;

        for (int i = 0; i < spellDataList.Count; i++)
        {
            float angle = (i * angleStep - 90f) * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            pos.y += verticalOffset; 
            pos.x += horizontalOffset;

            GameObject icon = Instantiate(spellButtonPrefab, circlePanel);
            SpellData newSpell = spellDataList[i];
            icon.GetComponentInChildren<Button>().onClick.AddListener(() => playerSpellManager.AddNewSpellToQueue(newSpell));
            RectTransform rt = icon.GetComponent<RectTransform>();
            rt.anchoredPosition = pos;

            icon.GetComponentInChildren<Button>().image.sprite = spellDataList[i].icon;

            spawnedIcons.Add(icon);
        }
    }



    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            circlePanel.gameObject.SetActive(!circlePanel.gameObject.activeSelf);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ShowMenu();

        }

        if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            HideMenu();
        }
    }

    private void ClearButtons()
    {
        foreach (var icon in spawnedIcons)
            Destroy(icon);

        spawnedIcons.Clear();
    }

}
