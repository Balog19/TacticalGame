using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private RecoilSystem recoilSystem;

    private PlayerControls controls;
    private Vector2 lookInput;
    private float xRotation = 0f;

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
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply recoil on top of mouse aim
        Vector3 recoilDelta = recoilSystem != null ? recoilSystem.GetCameraRecoilDelta() : Vector3.zero;

        // Camera looks up/down (with recoil pitch added)
        transform.localRotation = Quaternion.Euler(xRotation + recoilDelta.x, 0f, 0f);
        // Player body turns left/right (with recoil yaw added)
        playerBody.Rotate(Vector3.up * (mouseX + recoilDelta.y));
    }
}