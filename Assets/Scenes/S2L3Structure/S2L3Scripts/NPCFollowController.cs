using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCFollowController : MonoBehaviour, IInteractable
{
    [Header("References")]
    [Tooltip("Leave empty to auto-find the GameObject tagged 'Player'.")]
    public Transform player;
    public Animator animator;
    public NavMeshAgent agent;

    [Header("Movement")]
    [Tooltip("How fast the NPC moves while crouch-following.")]
    public float crouchSpeed = 1.2f;

    [Tooltip("How close the agent stops from the player's position.")]
    public float stoppingDistance = 1.0f;

    private bool isFollowing = false;
    public bool HasBeenFound { get; private set; } = false;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        agent.stoppingDistance = stoppingDistance;
        agent.speed = crouchSpeed;
    }

    // Call this to make the NPC start crouch-following the player (e.g. from FireAlarm.OnInteract).
    public void StartFollowing()
    {
        if (isFollowing) return;

        isFollowing = true;

        if (animator != null)
            animator.SetBool("isCrouching", true);
    }

    // Optional: call this to stop the NPC from following.
    public void StopFollowing()
    {
        isFollowing = false;

        if (agent != null)
            agent.ResetPath();

        if (animator != null)
        {
            animator.SetBool("isCrouching", false);
            animator.SetBool("isCrouchWalking", false);
        }
    }

    public void OnGazeEnter() { }
    public void OnGazeExit() { }

    public void OnInteract()
    {
        HasBeenFound = true;
        StartFollowing();

        if (VRMessageUI.Instance != null)
            VRMessageUI.Instance.ShowMessage("Your Distressed Familiy Member is now following you! Find a viable exit!");
    }

    private void Update()
    {
        if (!isFollowing || player == null || agent == null) return;

        agent.SetDestination(player.position);

        bool isMoving = agent.velocity.sqrMagnitude > 0.04f; // ~0.2 m/s threshold

        if (animator != null)
            animator.SetBool("isCrouchWalking", isMoving);
    }
}