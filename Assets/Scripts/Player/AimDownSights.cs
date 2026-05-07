using UnityEngine;

public class AimDownSights : MonoBehaviour
{
    [SerializeField] private Transform gun;
    [SerializeField] private Transform ironSightsPoint;
    [SerializeField] private float adsSpeed = 12f;
    [SerializeField] private float adsFOV = 50f;
    [SerializeField] private float hipFOV = 75f;
    [SerializeField] private WeaponKick weaponKick; // optional

    private Camera cam;
    private Vector3 hipPosition;
    private Quaternion hipRotation;
    private Vector3 adsPosition;
    private Quaternion adsRotation;
    private Vector3 currentBasePosition;
    private Quaternion currentBaseRotation;
    private PlayerControls controls;
    private bool isAiming;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        controls = new PlayerControls();
        controls.Player.Aim.performed += ctx => StartAiming();
        controls.Player.Aim.canceled += ctx => isAiming = false;
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Start()
    {
        hipPosition = gun.localPosition;
        hipRotation = gun.localRotation;
        currentBasePosition = hipPosition;
        currentBaseRotation = hipRotation;
    }

    private void StartAiming()
    {
        isAiming = true;
        Vector3 originalPos = gun.localPosition;
        Quaternion originalRot = gun.localRotation;

        gun.localPosition = hipPosition;
        gun.localRotation = Quaternion.identity;

        Vector3 sightInParentSpace = gun.parent.InverseTransformPoint(ironSightsPoint.position);
        Vector3 cameraInParentSpace = gun.parent.InverseTransformPoint(cam.transform.position);
        adsPosition = hipPosition + (cameraInParentSpace - sightInParentSpace);
        adsRotation = Quaternion.identity;

        gun.localPosition = originalPos;
        gun.localRotation = originalRot;
    }

    private void LateUpdate()
    {
        Vector3 targetPos = isAiming ? adsPosition : hipPosition;
        Quaternion targetRot = isAiming ? adsRotation : hipRotation;

        // Lerp the BASE position separately from kick
        currentBasePosition = Vector3.Lerp(currentBasePosition, targetPos, adsSpeed * Time.deltaTime);
        currentBaseRotation = Quaternion.Slerp(currentBaseRotation, targetRot, adsSpeed * Time.deltaTime);

        // Apply kick offsets on top of base
        Vector3 kickPos = weaponKick != null ? weaponKick.GetPositionOffset() : Vector3.zero;
        Quaternion kickRot = weaponKick != null ? weaponKick.GetRotationOffset() : Quaternion.identity;

        gun.localPosition = currentBasePosition + kickPos;
        gun.localRotation = currentBaseRotation * kickRot;

        float targetFOV = isAiming ? adsFOV : hipFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, adsSpeed * Time.deltaTime);
    }
}