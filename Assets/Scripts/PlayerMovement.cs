using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float sprintSpeed = 9f;
    public float acceleration = 10f;
    public float deceleration = 12f;

    [Header("Jumping & Gravity")]
    public float jumpHeight = 1.5f;
    public float gravity = -20f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Mouse Look")]
    public Transform playerCamera;
    public float mouseSensitivity = 100f;
    public float verticalClamp = 85f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private Vector3 currentMove;
    private float xRotation = 0f;
    private bool isGrounded;
    private bool jumpPressed;
    private bool sprintPressed;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleMouseLook();
        HandleGravityAndJump();
    }

    private void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f; // keep player grounded
    }

    private void HandleMovement()
    {
        float targetSpeed = sprintPressed ? sprintSpeed : moveSpeed;
        Vector3 targetMove = (transform.right * moveInput.x + transform.forward * moveInput.y) * targetSpeed;

        // Smooth acceleration & deceleration
        currentMove = Vector3.Lerp(currentMove, targetMove,
            (targetMove.magnitude > currentMove.magnitude ? acceleration : deceleration) * Time.deltaTime);

        controller.Move(currentMove * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        // Get mouse input
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        // Vertical rotation (camera only)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation (player root object)
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleGravityAndJump()
    {
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpPressed = false; // consume jump
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    #region Input System Callbacks
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Look(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
            jumpPressed = true;
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        sprintPressed = context.ReadValueAsButton();
    }
    #endregion
}
