using UnityEngine;

public class Lean : MonoBehaviour
{
    [Header("Camera Lean Settings")]
    [SerializeField] private float leanAngle = 15f;
    [SerializeField] private float leanOffset = 0.4f;
    [SerializeField] private float leanSpeed = 8f;

    [Header("Weapon Offset")]
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private Vector3 weaponLeanRightOffset = new Vector3(0.05f, 0.03f, 0f);
    [SerializeField] private Vector3 weaponLeanLeftOffset = new Vector3(-0.05f, 0.03f, 0f);
    [SerializeField] private float weaponCantAngle = 8f; // gun rolls opposite the lean direction

    private PlayerControls controls;
    private float leanInput;
    private float currentLean;

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private Vector3 weaponOriginalLocalPos;
    private Quaternion weaponOriginalLocalRot;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.LeanLeft.performed += ctx => ToggleLean(1f);
        controls.Player.LeanRight.performed += ctx => ToggleLean(-1f);
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Start()
    {
        originalLocalPos = transform.localPosition;
        originalLocalRot = transform.localRotation;

        if (weaponTransform != null)
        {
            weaponOriginalLocalPos = weaponTransform.localPosition;
            weaponOriginalLocalRot = weaponTransform.localRotation;
        }
    }

    private void ToggleLean(float direction)
    {
        if (Mathf.Approximately(leanInput, direction))
            leanInput = 0f;
        else
            leanInput = direction;
    }

    private void LateUpdate()
    {
        currentLean = Mathf.Lerp(currentLean, leanInput, leanSpeed * Time.deltaTime);

        // Camera lean
        Vector3 leanedPos = originalLocalPos + new Vector3(currentLean * leanOffset, 0f, 0f);
        Quaternion leanedRot = originalLocalRot * Quaternion.Euler(0f, 0f, -currentLean * leanAngle);
        transform.localPosition = leanedPos;
        transform.localRotation = leanedRot;

        // Weapon offset + cant
        if (weaponTransform != null)
        {
            Vector3 offset = Vector3.zero;

            if (currentLean < 0f)        // leaning right
                offset = weaponLeanRightOffset * Mathf.Abs(currentLean);
            else if (currentLean > 0f)   // leaning left
                offset = weaponLeanLeftOffset * currentLean;

            weaponTransform.localPosition = weaponOriginalLocalPos + offset;

            // Cant the gun opposite the lean direction
            float cant = -currentLean * weaponCantAngle;
            weaponTransform.localRotation = weaponOriginalLocalRot * Quaternion.Euler(0f, 0f, cant);
        }
    }

    public float GetCurrentLean() => currentLean;
}