using UnityEngine;

public class EditorVRTestingControls : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("References")]
    public CharacterController characterController;
    public Transform cameraTransform;
    public Transform cameraOffset;    // NEW: Needed to lower the camera when crouching

    [Header("Visuals & Animation")]
    public Animator animator;
    public Transform characterModel; 
    public float rotationSpeed = 10f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2.0f;
    public float maxLookAngle = 85.0f;
    private float rotationX = 0f;
    private float rotationY = 0f;

    [Header("Movement Settings")]
    public float walkSpeed = 3.5f;
    public float gravity = -9.81f;

    [Header("Crouch Settings")]
    public float standingHeight = 2.0f;
    public float crouchingHeight = 1.0f;
    private bool isCrouching = false;
    public float cameraStandingHeight = 1.6f;
    public float cameraCrouchingHeight = 0.8f;

    void Start()
    {
        if (characterController == null)
            characterController = GetComponentInParent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleEditorMouseLook();
        HandleEditorMovement();
        HandleCrouch();
    }

    void HandleEditorMouseLook()
    {
        if (Input.GetMouseButton(1) && cameraTransform != null)
        {
            rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
            rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            rotationY = Mathf.Clamp(rotationY, -maxLookAngle, maxLookAngle);
            cameraTransform.localRotation = Quaternion.Euler(rotationY, rotationX, 0);
        }
    }

    void HandleEditorMovement()
    {
        if (characterController == null || cameraTransform == null) return;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f; right.y = 0f;
        forward.Normalize(); right.Normalize();

        Vector3 moveDirection = (forward * moveZ) + (right * moveX);
        moveDirection.y = gravity; 

        characterController.Move(moveDirection * walkSpeed * Time.deltaTime);

        // --- UPDATED: Animation Logic ---
        bool isMoving = (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f);
        if (animator != null)
        {
            // Turn off normal walking if crouching, and vice versa
            animator.SetBool("isWalking", isMoving && !isCrouching);
            animator.SetBool("isCrouching", isCrouching);
            animator.SetBool("isCrouchWalking", isMoving && isCrouching);
        }

        // --- Rotation Logic ---
        Vector3 flatMove = new Vector3(moveDirection.x, 0f, moveDirection.z);
        if (flatMove != Vector3.zero && characterModel != null)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatMove);
            characterModel.rotation = Quaternion.Slerp(characterModel.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    void HandleCrouch()
    {
        // (Use "JoystickButton1" instead of LeftControl if editing VRControllerMovement)
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                characterController.height = crouchingHeight;
                if (cameraOffset != null) cameraOffset.localPosition = new Vector3(0, cameraCrouchingHeight, 0);
                if (characterModel != null) characterModel.gameObject.SetActive(false);
            }
            else
            {
                characterController.height = standingHeight;
                if (cameraOffset != null) cameraOffset.localPosition = new Vector3(0, cameraStandingHeight, 0);
                if (characterModel != null) characterModel.gameObject.SetActive(true);
            }
        }
    }
#endif
}