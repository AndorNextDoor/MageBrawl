using Unity.Netcode;
using UnityEngine;

public class SpellGrowBigger : NetworkBehaviour
{
    [SerializeField] private Vector3 originalSize;
    [SerializeField] private float increasedArea = 1;

    private float growDuration = 999f;
    private Vector3 startSize;
    private Vector3 targetSize;

    private float growthTimer = 0f;
    private bool isGrowing = false;

    public void SetGrowDuration(float value) => growDuration = value;
    public void SetIncreasedArea(float value) => increasedArea = value;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsServer)
            return;

        startSize = transform.localScale;
        targetSize = originalSize * increasedArea;
        transform.localScale = Vector3.zero;
        isGrowing = true;
    }

    private void Update()
    {
        if (!IsServer || !isGrowing) return;

        growthTimer += Time.deltaTime;
        float t = Mathf.Clamp01(growthTimer / growDuration);
        transform.localScale = Vector3.Lerp(Vector3.zero, targetSize, t);

        if (t >= 1f)
        {
            isGrowing = false;
        }
    }
}
