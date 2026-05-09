using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float kickInDuration = 0.05f;  // how long each kick takes to apply (seconds)
    [SerializeField] private float returnSmoothness = 5f;   // how fast recovery happens after firing stops

    private PlayerControls controls;
    private Vector2 lookInput;
    private float xRotation = 0f;
    private float yRotation = 0f;

    private float targetRecoilPitch = 0f;
    private float targetRecoilYaw = 0f;
    private float currentRecoilPitch = 0f;
    private float currentRecoilYaw = 0f;

    // Per-shot kick easing
    private float kickProgress = 1f;
    private float kickStartPitch;
    private float kickStartYaw;
    private float kickTargetPitch;
    private float kickTargetYaw;

    private bool isFiring = false;
    private float pitchAtFireStart = 0f;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Update()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        yRotation += mouseX;

        if (isFiring)
        {
            // Apply per-shot kick using cubic ease-out (fast at start, eases to target)
            if (kickProgress < 1f)
            {
                kickProgress += Time.deltaTime / kickInDuration;
                kickProgress = Mathf.Clamp01(kickProgress);

                float eased = 1f - Mathf.Pow(1f - kickProgress, 3f);

                currentRecoilPitch = Mathf.Lerp(kickStartPitch, kickTargetPitch, eased);
                currentRecoilYaw = Mathf.Lerp(kickStartYaw, kickTargetYaw, eased);
            }
        }
        else
        {
            // Recovery — smooth lerp back to zero
            currentRecoilPitch = Mathf.Lerp(currentRecoilPitch, targetRecoilPitch, returnSmoothness * Time.deltaTime);
            currentRecoilYaw = Mathf.Lerp(currentRecoilYaw, targetRecoilYaw, returnSmoothness * Time.deltaTime);

            if (Mathf.Abs(currentRecoilPitch) > 0.01f)
            {
                xRotation = Mathf.Lerp(xRotation, pitchAtFireStart, returnSmoothness * Time.deltaTime);
            }
        }

        float combinedPitch = xRotation + currentRecoilPitch;
        combinedPitch = Mathf.Clamp(combinedPitch, -90f, 90f);

        if (combinedPitch != xRotation + currentRecoilPitch)
        {
            xRotation = combinedPitch - currentRecoilPitch;
        }

        transform.localRotation = Quaternion.Euler(combinedPitch, yRotation + currentRecoilYaw, 0f);
    }

    public void StartFiring()
    {
        isFiring = true;
        pitchAtFireStart = xRotation;
    }

    public void StopFiring()
    {
        if (!isFiring) return;
        isFiring = false;
        targetRecoilPitch = 0f;
        targetRecoilYaw = 0f;
        kickProgress = 1f; // mark kick as done so it doesn't continue easing during recovery
    }

    public void AddRecoil(float pitchDelta, float yawDelta)
    {
        targetRecoilPitch += pitchDelta;
        targetRecoilYaw += yawDelta;

        // Start a fresh ease-out kick from current values to the new target
        kickStartPitch = currentRecoilPitch;
        kickStartYaw = currentRecoilYaw;
        kickTargetPitch = targetRecoilPitch;
        kickTargetYaw = targetRecoilYaw;
        kickProgress = 0f;
    }

    public void DecayRecoil(float decayRate)
    {
        targetRecoilPitch = Mathf.Lerp(targetRecoilPitch, 0f, decayRate * Time.deltaTime);
        targetRecoilYaw = Mathf.Lerp(targetRecoilYaw, 0f, decayRate * Time.deltaTime);
    }

    public void ResetRecoil()
    {
        targetRecoilPitch = 0f;
        targetRecoilYaw = 0f;
        currentRecoilPitch = 0f;
        currentRecoilYaw = 0f;
        kickProgress = 1f;
    }
}