using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Footstep Audio")]
    public AudioClip[] footstepSounds;
    public float walkStepInterval = 0.5f;
    public float crouchStepInterval = 0.7f; 

    private AudioSource audioSource;
    private CharacterController controller;
    private float stepTimer;
    private int currentStepIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
        
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        
        // Start the timer at the interval limit so the first step plays instantly
        stepTimer = walkStepInterval;
    }

    void Update()
    {
        // Check if the player is physically moving on the ground
        if (controller != null && controller.isGrounded && controller.velocity.magnitude > 0.1f)
        {
            stepTimer += Time.deltaTime;

            // Dynamically adjust the footstep speed if the player is crouching
            float currentInterval = (controller.height < 1.5f) ? crouchStepInterval : walkStepInterval;

            if (stepTimer >= currentInterval)
            {
                PlayNextStep();
                stepTimer = 0f;
            }
        }
        else
        {
            // Reset timer when standing still
            stepTimer = walkStepInterval; 
        }
    }

    void PlayNextStep()
    {
        if (footstepSounds.Length == 0) return;

        // Play the sound at the current array index
        audioSource.PlayOneShot(footstepSounds[currentStepIndex]);

        // Move to the next index, looping back to 0 if it reaches the end of the folder
        currentStepIndex++;
        if (currentStepIndex >= footstepSounds.Length)
        {
            currentStepIndex = 0;
        }
    }
}