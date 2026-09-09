using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableItem : MonoBehaviour, IInteractable
{
    [Header("Hold Settings")]
    public Transform customHoldPosition; 

    [Header("Optional Settings")]
    public Light itemLight; 
    public bool canDrop = true; 
    public bool isFlashlight = false;
    public bool isGoBagItem = false;
    public bool isMedicalSupply = false;

    [Header("Audio Settings")]
    public AudioClip interactSound; // NEW: Drag your audio file here
    private AudioSource audioSource; // NEW: The component that plays the sound

    private Transform holdPosition;
    private Rigidbody rb;
    private bool isHeld = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // NEW: Automatically set up the audio player
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        if (itemLight != null) itemLight.enabled = false;
        
        if (customHoldPosition != null)
        {
            holdPosition = customHoldPosition;
        }
        else
        {
            GameObject anchor = GameObject.Find("HoldAnchor");
            if (anchor == null) anchor = GameObject.Find("HoldPosition");
            
            if (anchor != null) holdPosition = anchor.transform;
        }
    }

    void Update()
    {
        if (canDrop && isHeld && (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.E))) 
        {
            Drop();
        }
    }

    public void OnGazeEnter() { }
    public void OnGazeExit() { }

    public void OnInteract()
    {
        // NEW: Play the sound immediately when clicked
        if (interactSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(interactSound);
        }

        if (!isHeld) Pickup();
    }

    void Pickup()
    {
        if (holdPosition == null) return;
        
        isHeld = true;
        rb.isKinematic = true; 
        transform.position = holdPosition.position;
        transform.parent = holdPosition;
        transform.localRotation = Quaternion.identity;

        if (itemLight != null) itemLight.enabled = true;

        if (isFlashlight && VRMessageUI.Instance != null)
        {
            VRMessageUI.Instance.ShowMessage("Collect items to make a go-bag!");
        }
    }

    void Drop()
    {
        isHeld = false;
        transform.parent = null;
        rb.isKinematic = false; 
        rb.useGravity = true; 
    }
}