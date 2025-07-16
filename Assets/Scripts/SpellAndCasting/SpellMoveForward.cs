using Unity.Netcode;
using UnityEngine;

public class SpellMoveForward : NetworkBehaviour
{
    [SerializeField] private float speed = 5;

    public void SetSpeed(float value) => speed = value;

    private void Update()
    {
        if (!IsServer)
            return;

        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
