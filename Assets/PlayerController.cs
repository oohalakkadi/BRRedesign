using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Walking speed of the player.")]
    public float speed = 20.0f;
    [Tooltip("Jumping speed (initial upward velocity).")]
    public float jumpSpeed = 10.0f;
    [Tooltip("Gravity applied to the player.")]
    public float gravity = 20.0f;

    [Header("Mouse Look Settings")]
    [Tooltip("Sensitivity for mouse movement.")]
    public float mouseSensitivity = 10.0f;
    [Tooltip("Reference to the player camera (child object).")]
    public Transform playerCamera;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float verticalLookRotation = 0.0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock the cursor to the center of the screen and hide it.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }


    void Update()
    {
        // If the left mouse button was released, re-lock the cursor.
        if (Input.GetMouseButtonUp(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        HandleMouseLook();
        HandleMovement();
    }

    /// <summary>
    /// Handles the mouse look input.
    /// The twist: if the left mouse button is held down (i.e. click and drag),
    /// do not update the camera’s rotation.
    /// </summary>
    void HandleMouseLook()
    {
        // If the left mouse button is down, assume you’re dragging and ignore mouse movement.
        if (Input.GetMouseButton(0))
        {
            return;
        }

        // Otherwise, process mouse look normally.
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the player horizontally (yaw)
        transform.Rotate(0f, mouseX, 0f);

        // Adjust the vertical rotation (pitch) and clamp it.
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        if (playerCamera != null)
        {
            playerCamera.localEulerAngles = new Vector3(verticalLookRotation, 0f, 0f);
        }
    }

    /// <summary>
    /// Handles the player movement and jumping.
    /// </summary>
    void HandleMovement()
    {
        if (characterController.isGrounded)
        {
            // Get movement input (WASD or arrow keys).
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            // Convert local movement to world space direction.
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            moveDirection = (forward * moveZ + right * moveX) * speed;

            // Apply jump if the jump button (typically space) is pressed.
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity continuously.
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the player.
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
