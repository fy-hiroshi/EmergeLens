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

    void HandleCrouch()
    {
        // (Use "JoystickButton1" instead of LeftControl if editing VRControllerMovement)
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                controller.height = crouchingHeight;
                if (cameraOffset != null) cameraOffset.localPosition = new Vector3(0, cameraCrouchingHeight, 0);
                if (characterModel != null) characterModel.gameObject.SetActive(false);
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