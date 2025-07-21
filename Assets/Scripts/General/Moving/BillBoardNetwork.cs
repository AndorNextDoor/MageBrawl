using Unity.Netcode;
using UnityEngine;

public class BillboardNetwork : NetworkBehaviour
{
    private Transform cam;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (cam != null & IsOwner)
        {
            transform.LookAt(transform.position + cam.forward);
        }
    }
}
