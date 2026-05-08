using UnityEngine;

public class AimDownSights : MonoBehaviour
{
    [SerializeField] private Transform gun;
    [SerializeField] private Transform ironSightsPoint;
    [SerializeField] private float adsSpeed = 12f;
    [SerializeField] private float adsFOV = 50f;
    [SerializeField] private float hipFOV = 75f;

    [SerializeField] private float adsDuration = 0.15f;

    [SerializeField] private WeaponKick weaponKick;
    private float aimProgress = 0f; // 0 = hip, 1 = ADS

    

    private Camera cam;
    private Vector3 hipPosition;
    private Quaternion hipRotation;
    private PlayerControls controls;
    private bool isAiming;
    private bool fullyAimed = false;

    public bool IsAiming() => isAiming;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        controls = new PlayerControls();
        controls.Player.Aim.performed += ctx => isAiming = true;
        controls.Player.Aim.canceled += ctx => isAiming = false;
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Start()
    {
        hipPosition = gun.localPosition;
        hipRotation = gun.localRotation;
    }

private void LateUpdate()
{
    Vector3 originalPos = gun.localPosition;
    Quaternion originalRot = gun.localRotation;

    gun.localPosition = hipPosition;
    gun.localRotation = Quaternion.identity;

    Vector3 sightInParentSpace = gun.parent.InverseTransformPoint(ironSightsPoint.position);
    Vector3 cameraInParentSpace = gun.parent.InverseTransformPoint(cam.transform.position);
    Vector3 adsPosition = hipPosition + (cameraInParentSpace - sightInParentSpace);

    gun.localPosition = originalPos;
    gun.localRotation = originalRot;

    float direction = isAiming ? 1f : -1f;
    aimProgress = Mathf.Clamp01(aimProgress + direction * Time.deltaTime / adsDuration);

    // Compute base position from progress
    Vector3 basePos = Vector3.Lerp(hipPosition, adsPosition, aimProgress);
    Quaternion baseRot = Quaternion.Slerp(hipRotation, Quaternion.identity, aimProgress);

    // Add kick offsets on top
    Vector3 kickPos = weaponKick != null ? weaponKick.GetPositionOffset() : Vector3.zero;
    Quaternion kickRot = weaponKick != null ? weaponKick.GetRotationOffset() : Quaternion.identity;

    gun.localPosition = basePos + kickPos;
    gun.localRotation = baseRot * kickRot;

    cam.fieldOfView = Mathf.Lerp(hipFOV, adsFOV, aimProgress);
}
}