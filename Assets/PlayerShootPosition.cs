using UnityEngine;

public class PlayerShootPosition : MonoBehaviour
{
    [SerializeField] private Transform shootTransform;

    public Transform GetShootTranform() { return shootTransform; }
}
