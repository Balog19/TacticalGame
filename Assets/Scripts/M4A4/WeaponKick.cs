using UnityEngine;

public class WeaponKick : MonoBehaviour
{
    [SerializeField] private Transform kickableTarget;

    [Header("Forward/Back Kick")]
    [SerializeField] private float kickAmount = 0.04f;
    [SerializeField] private float returnSpeed = 12f;
    [SerializeField] private float snappiness = 18f;

    [Header("Side-to-Side Kick")]
    [SerializeField] private float sideKickAmount = 0.01f;   // max horizontal jitter per shot
    [SerializeField] private float sideReturnSpeed = 10f;
    [SerializeField] private float sideSnappiness = 16f;

    private float currentOffset;
    private float targetOffset;
    private float currentSideOffset;
    private float targetSideOffset;
    private Vector3 originalLocalPos;

    private void Start()
    {
        if (kickableTarget != null)
            originalLocalPos = kickableTarget.localPosition;
    }

    private void LateUpdate()
    {
        // Forward/back spring
        targetOffset = Mathf.Lerp(targetOffset, 0f, returnSpeed * Time.deltaTime);
        currentOffset = Mathf.Lerp(currentOffset, targetOffset, snappiness * Time.deltaTime);

        // Side-to-side spring
        targetSideOffset = Mathf.Lerp(targetSideOffset, 0f, sideReturnSpeed * Time.deltaTime);
        currentSideOffset = Mathf.Lerp(currentSideOffset, targetSideOffset, sideSnappiness * Time.deltaTime);

        if (kickableTarget != null)
        {
            // Update base position when both kicks are minimal
            if (Mathf.Abs(currentOffset) < 0.0001f && Mathf.Abs(currentSideOffset) < 0.0001f)
            {
                originalLocalPos = kickableTarget.localPosition;
            }

            Vector3 offset = new Vector3(currentSideOffset, 0f, -currentOffset);
            kickableTarget.localPosition = originalLocalPos + offset;
        }
    }

    public void Kick()
    {
        targetOffset += kickAmount;
        // Random direction on each shot — sometimes left, sometimes right
        targetSideOffset += Random.Range(-sideKickAmount, sideKickAmount);
    }
}