using UnityEngine;

public class WeaponKick : MonoBehaviour
{
    [SerializeField] private Transform kickableTarget;
    [SerializeField] private Transform rollPivot;          // crosshair / reticle point — roll happens around this

    [Header("Position Kick")]
    [SerializeField] private float kickBack = 0.04f;
    [SerializeField] private float kickRandomness = 0.005f;  // random spread around the base kick
    [SerializeField] private float positionReturnSpeed = 14f;
    [SerializeField] private float positionSnappiness = 22f;

    [Header("Roll Kick (around pivot)")]
    [SerializeField] private float rollAmount = 2f;          // max degrees per shot
    [SerializeField] private float rollReturnSpeed = 10f;
    [SerializeField] private float rollSnappiness = 16f;

    private Vector3 currentPosOffset;
    private Vector3 targetPosOffset;
    private float currentRoll;
    private float targetRoll;
    private float lastAppliedRoll;

    public Vector3 GetPositionOffset() => currentPosOffset;
    public Quaternion GetRotationOffset() => Quaternion.identity; // roll handled separately via RotateAround

    private void LateUpdate()
    {
        // Position kick decay
        targetPosOffset = Vector3.Lerp(targetPosOffset, Vector3.zero, positionReturnSpeed * Time.deltaTime);
        currentPosOffset = Vector3.Lerp(currentPosOffset, targetPosOffset, positionSnappiness * Time.deltaTime);

        // Roll decay
        targetRoll = Mathf.Lerp(targetRoll, 0f, rollReturnSpeed * Time.deltaTime);
        currentRoll = Mathf.Lerp(currentRoll, targetRoll, rollSnappiness * Time.deltaTime);

        // Apply roll around the pivot (rolls the kickable target around the crosshair)
        if (kickableTarget != null && rollPivot != null)
        {
            float rollDelta = currentRoll - lastAppliedRoll;
            kickableTarget.RotateAround(rollPivot.position, kickableTarget.forward, rollDelta);
            lastAppliedRoll = currentRoll;
        }
    }

    public void Kick()
    {
        // Position kick with random jitter around the base values
        Vector3 jitter = new Vector3(
            Random.Range(0, 0),
            Random.Range(0, 0),
            Random.Range(-kickRandomness, kickRandomness)
        );
        targetPosOffset += new Vector3(0f, 0f, -kickBack) + jitter;

        // Random roll direction
        targetRoll += Random.Range(-rollAmount, rollAmount);
    }
}