using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float swayClamp = 0.9f;
    public float smoothing = 3f;
    [SerializeField] private AimDownSights aimDownSights;

    private Vector3 originPosition;
    private PlayerControls controls;
    private Vector2 lookInput;

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
        originPosition = transform.localPosition;
    }

    public void SnapToOrigin()
    {
        transform.localPosition = originPosition;
    }
    private void Update()
    {
        bool isAiming = aimDownSights != null && aimDownSights.IsAiming();

        if(!isAiming)
        {
            lookInput.x = Mathf.Clamp(lookInput.x * swayClamp, -swayClamp, swayClamp);
            lookInput.y = Mathf.Clamp(lookInput.y * swayClamp, -swayClamp, swayClamp);

            Vector3 targetPosition = new Vector3(-lookInput.x, -lookInput.y, 0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition + originPosition, Time.deltaTime * smoothing);
        }

    }
}