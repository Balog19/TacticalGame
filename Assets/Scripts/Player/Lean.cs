using UnityEngine;

public class Lean : MonoBehaviour
{
    [Header("Camera Lean Settings")]
    [SerializeField] private float leanAngle = 15f;
    [SerializeField] private float leanOffset = 0.4f;
    [SerializeField] private float leanSpeed = 8f;

    [Header("Weapon Lean")]
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private Transform pivotPoint; // the camera, or a transform at the camera's position
    [SerializeField] private float weaponLeanAngle = 10f; // how many degrees the weapon rotates around the camera

    private PlayerControls controls;
    private float leanInput;
    private float currentLean;
    private float lastAppliedWeaponLean;

    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;

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

        // Camera lean (this script's GameObject)
        Vector3 leanedPos = originalLocalPos + new Vector3(currentLean * leanOffset, 0f, 0f);
        Quaternion leanedRot = originalLocalRot * Quaternion.Euler(0f, 0f, -currentLean * leanAngle);
        transform.localPosition = leanedPos;
        transform.localRotation = leanedRot;

        // Weapon rotation around the pivot point
        if (weaponTransform != null && pivotPoint != null)
        {
            float targetWeaponLean = currentLean * weaponLeanAngle;
            float delta = targetWeaponLean - lastAppliedWeaponLean;

            // Rotate around the camera/pivot's forward axis
            weaponTransform.RotateAround(pivotPoint.position, pivotPoint.forward, delta);
            lastAppliedWeaponLean = targetWeaponLean;
        }
    }

    public float GetCurrentLean() => currentLean;
}