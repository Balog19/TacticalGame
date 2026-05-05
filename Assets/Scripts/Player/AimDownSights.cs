using UnityEngine;

public class AimDownSights : MonoBehaviour
{
    [SerializeField] private Transform gun;
    [SerializeField] private Transform ironSightsPoint;
    [SerializeField] private float adsSpeed = 12f;
    [SerializeField] private float adsFOV = 50f;
    [SerializeField] private float hipFOV = 75f;

    private Camera cam;
    private Vector3 hipPosition;
    private Quaternion hipRotation;
    private Vector3 adsPosition;       // computed once when ADS starts
    private Quaternion adsRotation;    // computed once when ADS starts
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
    }

    private void StartAiming()
    {
        isAiming = true;
        // Compute ADS position once based on gun's REST pose, not its current state
        // First, temporarily set gun to its rest pose to compute the offset accurately
        Vector3 originalPos = gun.localPosition;
        Quaternion originalRot = gun.localRotation;

        gun.localPosition = hipPosition;
        gun.localRotation = Quaternion.identity; // ADS rotation is identity

        Vector3 sightInParentSpace = gun.parent.InverseTransformPoint(ironSightsPoint.position);
        Vector3 cameraInParentSpace = gun.parent.InverseTransformPoint(cam.transform.position);
        adsPosition = hipPosition + (cameraInParentSpace - sightInParentSpace);
        adsRotation = Quaternion.identity;

        // Restore current pose (so we don't visually snap)
        gun.localPosition = originalPos;
        gun.localRotation = originalRot;
    }

    private void LateUpdate()
    {
        Vector3 targetPos = isAiming ? adsPosition : hipPosition;
        Quaternion targetRot = isAiming ? adsRotation : hipRotation;

        gun.localPosition = Vector3.Lerp(gun.localPosition, targetPos, adsSpeed * Time.deltaTime);
        gun.localRotation = Quaternion.Slerp(gun.localRotation, targetRot, adsSpeed * Time.deltaTime);

        float targetFOV = isAiming ? adsFOV : hipFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, adsSpeed * Time.deltaTime);
    }
}