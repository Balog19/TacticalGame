using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float recoilSmoothness = 14f; // higher = snappier, lower = floatier

    private PlayerControls controls;
    private Vector2 lookInput;
    private float xRotation = 0f;
    private float yRotation = 0f;

    // Recoil targets and current values (smoothed)
    private float targetRecoilPitch = 0f;
    private float targetRecoilYaw = 0f;
    private float currentRecoilPitch = 0f;
    private float currentRecoilYaw = 0f;

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

        // Smoothly lerp recoil
        currentRecoilPitch = Mathf.Lerp(currentRecoilPitch, targetRecoilPitch, recoilSmoothness * Time.deltaTime);
        currentRecoilYaw = Mathf.Lerp(currentRecoilYaw, targetRecoilYaw, recoilSmoothness * Time.deltaTime);

        // Clamp the COMBINED display value, not xRotation alone
        float combinedPitch = xRotation + currentRecoilPitch;
        combinedPitch = Mathf.Clamp(combinedPitch, -90f, 90f);

        // If clamping happened, "spend" the excess against player input so they don't keep building unrecovered overpull
        if (combinedPitch != xRotation + currentRecoilPitch)
        {
            xRotation = combinedPitch - currentRecoilPitch;
        }

        transform.localRotation = Quaternion.Euler(combinedPitch, yRotation + currentRecoilYaw, 0f);
    }

    public void AddRecoil(float pitchDelta, float yawDelta)
    {
        targetRecoilPitch += pitchDelta;
        targetRecoilYaw += yawDelta;
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
    }
}