using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class InjuredNPC : MonoBehaviour, IInteractable
{
    [Header("Triage & Escape")]
    public int shardsRemaining = 3;
    public GameObject wrappedBandageMesh;
    public Animator animator;
    public Transform exitDoorDestination;
    private NavMeshAgent agent;

    [Header("Chronological Locks")]
    public BreakerBox mainBreaker; 
    public GrabbableItem bagGrabbableScript;

    [Header("Bandage Settings")]
    public float bandageTime = 5.0f; 
    public Image progressBarFill; 
    public GameObject timerCanvas; 
    
    private float currentBandageProgress = 0f;
    private bool isGazedAt = false;
    private bool isCured = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (animator != null) animator.SetBool("isCuredWalking", false);
                
                agent.isStopped = true;
                agent.updateRotation = false;
                agent.velocity = Vector3.zero;

                if (exitDoorDestination != null)
                {
                    Vector3 targetEuler = exitDoorDestination.rotation.eulerAngles;
                    Quaternion flatRotation = Quaternion.Euler(0, targetEuler.y, 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, flatRotation, 5f * Time.deltaTime);
                }
            }
        }

        // 5-Second Bandage Timer
        if (shardsRemaining <= 0 && isGazedAt && !isCured && HasBandage()) 
        {
            if (Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.E) || Input.GetMouseButton(0))
            {
                if (timerCanvas != null && !timerCanvas.activeSelf) timerCanvas.SetActive(true);

                currentBandageProgress += Time.deltaTime;

                if (progressBarFill != null)
                {
                    progressBarFill.fillAmount = currentBandageProgress / bandageTime;
                }

                if (currentBandageProgress >= bandageTime)
                {
                    ApplyBandage();
                }
            }
            else
            {
                ResetTimer();
            }
        }
    }
    
    void ResetTimer()
    {
        currentBandageProgress = 0f;
        if (progressBarFill != null) progressBarFill.fillAmount = 0f;
        if (timerCanvas != null && timerCanvas.activeSelf) timerCanvas.SetActive(false); 
    }

    public void RemoveShard()
    {
        shardsRemaining--;
        if (VRMessageUI.Instance != null) 
            VRMessageUI.Instance.ShowMessage("Shard removed! (" + shardsRemaining + " remaining)");
    }

    public void OnGazeEnter() { isGazedAt = true; }
    public void OnGazeExit() { isGazedAt = false; ResetTimer(); }

    public void OnInteract()
    {
        if (mainBreaker != null && !mainBreaker.isPowerOff)
        {
            if (VRMessageUI.Instance != null) VRMessageUI.Instance.ShowMessage("Turn off the breaker first!");
            return;
        }

        if (shardsRemaining > 0)
        {
            if (VRMessageUI.Instance != null) VRMessageUI.Instance.ShowMessage("Extract all glass shards first!");
            return;
        }

        if (!HasBandage())
        {
            if (VRMessageUI.Instance != null) VRMessageUI.Instance.ShowMessage("Find the bandage to heal the wound!");
        }
    }

    bool HasBandage()
    {
        GameObject bandageAnchorObj = GameObject.Find("BandageAnchor");
        if (bandageAnchorObj != null && bandageAnchorObj.transform.childCount > 0)
        {
            GrabbableItem heldItem = bandageAnchorObj.transform.GetChild(0).GetComponent<GrabbableItem>();
            return heldItem != null && heldItem.isMedicalSupply;
        }
        return false;
    }

    void ApplyBandage()
    {
        isCured = true;
        ResetTimer();

        GameObject bandageAnchorObj = GameObject.Find("BandageAnchor");
        GrabbableItem heldItem = bandageAnchorObj.transform.GetChild(0).GetComponent<GrabbableItem>();
        Destroy(heldItem.gameObject); 
        
        if (wrappedBandageMesh != null) wrappedBandageMesh.SetActive(true);
        if (animator != null)
        {
            animator.SetBool("isCured", true);
            animator.SetBool("isCuredWalking", true);
        }

        if (VRMessageUI.Instance != null) 
            VRMessageUI.Instance.ShowMessage("NPC healed. Grab the go bag and head to the exit door.");

        if (bagGrabbableScript != null)
        {
            bagGrabbableScript.enabled = true;
            bagGrabbableScript.gameObject.layer = LayerMask.NameToLayer("Interactable");
        }

        if (agent != null && exitDoorDestination != null)
        {
            agent.enabled = true;
            agent.SetDestination(exitDoorDestination.position);
        }
    }
}