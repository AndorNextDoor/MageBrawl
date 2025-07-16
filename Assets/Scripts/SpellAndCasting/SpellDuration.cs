using Unity.Netcode;
using UnityEngine;

public class SpellDuration : NetworkBehaviour
{
    [SerializeField] private float duration = 10f;


    private void Update()
    {
        if (!IsServer)
            return;

        duration -= Time.deltaTime;

        if(duration <= 0)
        {
            this.GetComponent<NetworkObject>().Despawn(true);
        }
    }
}
