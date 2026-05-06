using UnityEngine;

public class Recoil : MonoBehaviour
{
    [Header("Recoil Per Shot")]
    [SerializeField] private float recoilX = 3f;
    [SerializeField] private float recoilY = 0.6f;

    [Header("Settings")]
    [SerializeField] private float snappiness = 14f;
    [SerializeField] private float returnSpeed = 5f;
    [SerializeField] private float recoveryDelay = 0.1f;

    [Header("Reference")]
    [SerializeField] private Transform aimTransform; // CameraRig — what holds the player's actual aim

    private Vector3 currentRotation;
    private Vector3 targetRotation;
    private float lastFireTime;
    private bool isFiring;
    private float storedAimPitch; // the pitch the player was at when they started firing

    private void Update()
    {
        // Detect when firing stops (a brief delay so single-tap doesn't trigger recovery)
        if (isFiring && Time.time - lastFireTime > 0.05f)
        {
            isFiring = false;
            // Trigger snap-back to stored pitch
            SnapToStoredAim();
        }

        // Smoothly drive currentRotation toward target
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    public void RecoilFire()
    {
        // First shot of a spray — store current aim pitch
        if (!isFiring)
        {
            isFiring = true;
            if (aimTransform != null)
                storedAimPitch = aimTransform.localEulerAngles.x;
        }

        Vector3 kick = new Vector3(-recoilX, Random.Range(-recoilY, recoilY), 0f);
        targetRotation += kick;
        lastFireTime = Time.time;
    }

    private void SnapToStoredAim()
    {
        if (aimTransform == null) return;

        // Get current pitch (handle Unity's 0-360 wrap)
        float currentPitch = aimTransform.localEulerAngles.x;
        if (currentPitch > 180f) currentPitch -= 360f;

        float storedPitch = storedAimPitch;
        if (storedPitch > 180f) storedPitch -= 360f;

        // Apply the stored pitch back to the aim transform
        Vector3 euler = aimTransform.localEulerAngles;
        euler.x = storedPitch;
        aimTransform.localEulerAngles = euler;
    }
}