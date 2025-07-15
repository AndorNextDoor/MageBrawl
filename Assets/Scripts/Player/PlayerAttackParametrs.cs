using Unity.Netcode;
using UnityEngine;

public class PlayerAttackParametrs : NetworkBehaviour
{
    [SerializeField] private Transform shootTransform;
    public Transform _ShootTransform => shootTransform;
}
 