using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private float sensX = 50f;
    [SerializeField] private float sensY = 50f;

    [SerializeField] private Transform cameraHolder; // Also assigned in inspector
    [SerializeField] private Transform orientation;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private void Start()
    {
        if (!IsOwner)
        {
            cameraHolder.gameObject.SetActive(false);
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (!IsOwner) return;

        //if (!RoundManager.Instance.playersCanPlay.Value)
        //    return;

        float mouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime * 100;
        float mouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime * 100;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -75f, 30f);


        // Rotate camera up/down
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        // Rotate orientation left/right
    }

    private void Update()
    {
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        
    }
    public Transform GetOrientation()
    {
        return orientation;
    }
}
