using UnityEngine;

public class Lean : MonoBehaviour
{
    [Header("Lean Settings")]
    [SerializeField] private float leanAngle = 15f;
    [SerializeField] private float leanOffset = 0.4f;
    [SerializeField] private float leanSpeed = 8f;

    private PlayerControls controls;
    private float leanInput;          // -1, 0, or +1
    private float currentLean;
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
        // If already leaning this direction, return to center
        if (Mathf.Approximately(leanInput, direction))
            leanInput = 0f;
        // Otherwise lean in the new direction (works for first lean OR switching sides)
        else
            leanInput = direction;
    }

    private void LateUpdate()
    {
        currentLean = Mathf.Lerp(currentLean, leanInput, leanSpeed * Time.deltaTime);

        Quaternion leanedRot = originalLocalRot * Quaternion.Euler(0f, 0f, -currentLean * leanAngle);

        transform.localRotation = leanedRot;
    }
}