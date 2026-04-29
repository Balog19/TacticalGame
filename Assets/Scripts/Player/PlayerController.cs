using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float strafeSpeedMultiplier = 0.6f;
    [SerializeField] private float backwardSpeedMultiplier = 0.7f;
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector3 velocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void Update()
    {
        // Animator uses raw input so blend tree plays full-intensity animations
        animator.SetFloat("SidewaysSpeed", moveInput.x, 0.1f, Time.deltaTime);
        animator.SetFloat("ForwardSpeed", moveInput.y, 0.1f, Time.deltaTime);

        // Movement uses scaled input
        Vector2 adjustedInput = moveInput;
        adjustedInput.x *= strafeSpeedMultiplier;
        if (adjustedInput.y < 0) adjustedInput.y *= backwardSpeedMultiplier;

        // Move relative to where the player is facing
        Vector3 move = transform.right * adjustedInput.x + transform.forward * adjustedInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}