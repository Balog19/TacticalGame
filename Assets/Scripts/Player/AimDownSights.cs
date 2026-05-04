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
    private PlayerControls controls;
    private bool isAiming;

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
    }

    private void LateUpdate()
    {
        if (isAiming)
        {
            // Where the IronSightsPoint currently is, in the gun's parent's local space
            Vector3 sightInParentSpace = gun.parent.InverseTransformPoint(ironSightsPoint.position);
            // Where the camera is, in that same space
            Vector3 cameraInParentSpace = gun.parent.InverseTransformPoint(cam.transform.position);
            // Shift the gun by the difference so the sight ends up at the camera
            Vector3 targetLocalPos = gun.localPosition + (cameraInParentSpace - sightInParentSpace);

            gun.localPosition = Vector3.Lerp(gun.localPosition, targetLocalPos, adsSpeed * Time.deltaTime);
        }
        else
        {
            gun.localPosition = Vector3.Lerp(gun.localPosition, hipPosition, adsSpeed * Time.deltaTime);
        }

        float targetFOV = isAiming ? adsFOV : hipFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, adsSpeed * Time.deltaTime);
    }
}