using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class VRControllerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform vrCamera;       
    public Transform cameraOffset;
    public Transform characterModel;   
    public Animator animator;      

    [Header("Movement Settings")]
    public float walkSpeed = 3.0f;
    public float rotationSpeed = 10f;
    private CharacterController controller;

    [Header("Crouch Settings")]
    public float standingHeight = 2.0f;
    public float crouchingHeight = 1.0f;
    private bool isCrouching = false;
    public float cameraStandingHeight = 1.6f;
    public float cameraCrouchingHeight = 0.8f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement();
        HandleCrouch();
        HandleModelRotation();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 forward = vrCamera.forward;
        Vector3 right = vrCamera.right;
        forward.y = 0f; right.y = 0f;
        forward.Normalize(); right.Normalize();

        Vector3 moveDirection = (forward * moveZ) + (right * moveX);
        moveDirection.y = -9.81f; 

        controller.Move(moveDirection * walkSpeed * Time.deltaTime);

        // --- UPDATED: Animation Logic ---
        bool isMoving = (Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f);
        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving && !isCrouching);
            animator.SetBool("isCrouching", isCrouching);
            animator.SetBool("isCrouchWalking", isMoving && isCrouching);
        }
    }

    // NEW: The model now tracks where the camera/head is looking (yaw only),
    // continuously, instead of only turning while walking.
    void HandleModelRotation()
    {
        if (characterModel == null || vrCamera == null) return;

        Vector3 flatForward = vrCamera.forward;
        flatForward.y = 0f;

        if (flatForward.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatForward.normalized);
            characterModel.rotation = Quaternion.Slerp(characterModel.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    void HandleCrouch()
    {
        // Now driven by VRInputConfig.crouchButton (B button by default).
        if (VRInputConfig.CrouchPressed())
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                controller.height = crouchingHeight;
                if (cameraOffset != null) cameraOffset.localPosition = new Vector3(0, cameraCrouchingHeight, 0);
            }
            else
            {
                controller.height = standingHeight;
                if (cameraOffset != null) cameraOffset.localPosition = new Vector3(0, cameraStandingHeight, 0);
                if (characterModel != null) characterModel.gameObject.SetActive(true);
            }
        }
    }
}