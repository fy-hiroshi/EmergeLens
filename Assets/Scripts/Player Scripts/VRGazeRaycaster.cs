using UnityEngine;
using UnityEngine.UI;

public class VRGazeRaycaster : MonoBehaviour
{
    // 1. ADD THE INSTANCE VARIABLE HERE
    public static VRGazeRaycaster Instance;

    [Header("Raycast Settings")]
    public float interactionDistance = 3.0f;
    public LayerMask interactableLayer = ~0; 

    [Header("Reticle Feedback")]
    public Image reticleDot;
    public Color normalColor = Color.white;
    public Color highlightColor = Color.green;
    public Color goBagColor = Color.blue; 

    public IInteractable currentTarget;

    // 2. ADD THE AWAKE METHOD HERE
    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        HandleGazeRaycast();
        HandleControllerInput();
    }

    void HandleGazeRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (currentTarget != interactable)
                {
                    if (currentTarget != null) currentTarget.OnGazeExit();
                    currentTarget = interactable;
                    currentTarget.OnGazeEnter();
                    
                    // Check if the object is a VIP go-bag item
                    GrabbableItem grabItem = hit.collider.GetComponent<GrabbableItem>();
                    if (grabItem != null && grabItem.isGoBagItem)
                    {
                        SetReticleActive(true, goBagColor);
                    }
                    else
                    {
                        SetReticleActive(true, highlightColor);
                    }
                }
                return;
            }
        }

        if (currentTarget != null)
        {
            currentTarget.OnGazeExit();
            currentTarget = null;
            SetReticleActive(false, normalColor);
        }
    }

    void HandleControllerInput()
    {
        if (currentTarget != null && (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)))
        {
            currentTarget.OnInteract();
        }
    }

    void SetReticleActive(bool isTargeted, Color activeColor)
    {
        if (reticleDot != null)
        {
            reticleDot.color = isTargeted ? activeColor : normalColor;
            reticleDot.transform.localScale = isTargeted ? Vector3.one * 1.5f : Vector3.one;
        }
    }
}