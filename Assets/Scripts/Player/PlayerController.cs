using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 20f;
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 currentVelocity; // smoothed horizontal velocity (X = strafe, Y = forward)
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
        // Target velocity based on input
        Vector2 targetVelocity = moveInput;

        // Pick accel or decel rate per axis depending on whether we're speeding up or slowing down
        float xRate = Mathf.Abs(targetVelocity.x) > Mathf.Abs(currentVelocity.x) ? acceleration : deceleration;
        float yRate = Mathf.Abs(targetVelocity.y) > Mathf.Abs(currentVelocity.y) ? acceleration : deceleration;

        currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, targetVelocity.x, xRate * Time.deltaTime);
        currentVelocity.y = Mathf.MoveTowards(currentVelocity.y, targetVelocity.y, yRate * Time.deltaTime);

        // Animator uses the smoothed velocity so animations ease in/out too
        animator.SetFloat("SidewaysSpeed", currentVelocity.x);
        animator.SetFloat("ForwardSpeed", currentVelocity.y);

        // Move relative to where the player is facing
        Vector3 move = transform.right * currentVelocity.x + transform.forward * currentVelocity.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}