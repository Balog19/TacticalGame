using UnityEngine;

public class IdleHandSway : MonoBehaviour
{
    [Header("Position Sway")]
    [SerializeField] private float positionAmplitude = 0.005f;  // how far the gun drifts
    [SerializeField] private float positionFrequency = 1.5f;     // how fast the drift cycles

    [Header("Rotation Sway")]
    [SerializeField] private float rotationAmplitude = 0.5f;     // degrees of subtle rotation
    [SerializeField] private float rotationFrequency = 1.2f;     // breathing-rate motion

    [Header("Smoothing")]
    [SerializeField] private float smoothing = 4f;

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private Vector3 currentPositionOffset;
    private Vector3 currentRotationOffset;

    private void Start()
    {
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
    }

    private void LateUpdate()
    {
        float t = Time.time;

        // Position offset — figure-8 pattern with offset frequencies for organic feel
        Vector3 targetPosOffset = new Vector3(
            Mathf.Sin(t * positionFrequency) * positionAmplitude,
            Mathf.Cos(t * positionFrequency * 0.7f) * positionAmplitude,
            0f
        );

        // Rotation offset — slow breathing-like sway
        Vector3 targetRotOffset = new Vector3(
            Mathf.Sin(t * rotationFrequency * 0.8f) * rotationAmplitude,
            Mathf.Cos(t * rotationFrequency * 0.6f) * rotationAmplitude,
            Mathf.Sin(t * rotationFrequency * 0.4f) * rotationAmplitude * 0.5f
        );

        // Smooth toward targets so changes are gradual
        currentPositionOffset = Vector3.Lerp(currentPositionOffset, targetPosOffset, smoothing * Time.deltaTime);
        currentRotationOffset = Vector3.Lerp(currentRotationOffset, targetRotOffset, smoothing * Time.deltaTime);

        transform.localPosition = originalLocalPos + currentPositionOffset;
        transform.localRotation = originalLocalRot * Quaternion.Euler(currentRotationOffset);
    }
}