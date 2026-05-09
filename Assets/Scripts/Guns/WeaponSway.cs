using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Mouse Sway")]
    [SerializeField] private float swayClamp = 0.9f;
    [SerializeField] private float swaySmoothing = 3f;

    [Header("Idle Sway")]
    [SerializeField] private float idlePositionAmplitude = 0.005f;
    [SerializeField] private float idlePositionFrequency = 1.5f;
    [SerializeField] private float idleRotationAmplitude = 0.5f;
    [SerializeField] private float idleRotationFrequency = 1.2f;

    [Header("References")]
    [SerializeField] private AimDownSights aimDownSights;

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private PlayerControls controls;
    private Vector2 lookInput;
    private bool wasAiming;

    private Vector3 currentMousePos;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Start()
    {
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;
    }

    private void Update()
    {
        bool isAiming = aimDownSights != null && aimDownSights.IsAiming();

        if (isAiming)
        {
            if (!wasAiming)
            {
                transform.localPosition = originalLocalPos;
                transform.localRotation = originalLocalRot;
                wasAiming = true;
            }
            return;
        }
        wasAiming = false;

        // Mouse-based sway
        Vector2 clampedInput = new Vector2(
            Mathf.Clamp(lookInput.x * swayClamp, -swayClamp, swayClamp),
            Mathf.Clamp(lookInput.y * swayClamp, -swayClamp, swayClamp)
        );
        Vector3 targetMouseOffset = new Vector3(-clampedInput.x, -clampedInput.y, 0f);
        currentMousePos = Vector3.Lerp(currentMousePos, targetMouseOffset, swaySmoothing * Time.deltaTime);

        // Idle sine-wave sway
        float t = Time.time;
        Vector3 idlePosOffset = new Vector3(
            Mathf.Sin(t * idlePositionFrequency) * idlePositionAmplitude,
            Mathf.Cos(t * idlePositionFrequency * 0.7f) * idlePositionAmplitude,
            0f
        );
        Vector3 idleRotOffset = new Vector3(
            Mathf.Sin(t * idleRotationFrequency * 0.8f) * idleRotationAmplitude,
            Mathf.Cos(t * idleRotationFrequency * 0.6f) * idleRotationAmplitude,
            Mathf.Sin(t * idleRotationFrequency * 0.4f) * idleRotationAmplitude * 0.5f
        );

        // Combine and apply
        transform.localPosition = originalLocalPos + currentMousePos + idlePosOffset;
        transform.localRotation = originalLocalRot * Quaternion.Euler(idleRotOffset);
    }
}