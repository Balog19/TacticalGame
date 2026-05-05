using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 20f;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform cameraRig;

    private CharacterController controller;
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 currentVelocity;
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
        Vector2 targetVelocity = moveInput;

        float xRate = Mathf.Abs(targetVelocity.x) > Mathf.Abs(currentVelocity.x) ? acceleration : deceleration;
        float yRate = Mathf.Abs(targetVelocity.y) > Mathf.Abs(currentVelocity.y) ? acceleration : deceleration;

        currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, targetVelocity.x, xRate * Time.deltaTime);
        currentVelocity.y = Mathf.MoveTowards(currentVelocity.y, targetVelocity.y, yRate * Time.deltaTime);

        if (animator != null)
        {
            animator.SetFloat("SidewaysSpeed", currentVelocity.x);
            animator.SetFloat("ForwardSpeed", currentVelocity.y);
        }

        // Use CameraRig's right/forward for direction, but flatten to horizontal plane
        Vector3 camRight = cameraRig.right;
        Vector3 camForward = cameraRig.forward;
        camRight.y = 0; camRight.Normalize();
        camForward.y = 0; camForward.Normalize();

        Vector3 move = camRight * currentVelocity.x + camForward * currentVelocity.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}