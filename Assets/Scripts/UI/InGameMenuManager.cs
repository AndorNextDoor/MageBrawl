using UnityEngine;

public class InGameMenuManager : MonoBehaviour
{
    public static InGameMenuManager Instance { get; private set; }
    [SerializeField] private GameObject menuCanvas;


    private void Awake()
    {
        Instance = this;
    }
    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideShowMenu();
        }
    }

    public void HideShowMenu()
    {
        menuCanvas.SetActive(!menuCanvas.activeSelf);
        if (menuCanvas.activeSelf)
        {
            ShowCursor();
        }
        else
        {
            HideCursor();
        }
    }

}
