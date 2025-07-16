using Unity.Netcode;
using UnityEngine;

public class Test_StartHost : MonoBehaviour
{

    private void Start()
    {
        NetworkManager.Singleton.StartHost();
    }
}
