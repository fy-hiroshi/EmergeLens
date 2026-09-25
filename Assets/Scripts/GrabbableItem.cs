using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableItem : MonoBehaviour, IInteractable
{
    [Header("Hold Settings")]
    public Transform customHoldPosition; 

    [Header("Blackout Highlight Settings")]
    public MeshRenderer itemRenderer; 
    public Material highlightMaterial; 
    private Material[] originalMaterials; // Stores the base textures

    [Header("Optional Settings")]
    public Light itemLight; 
    public bool canDrop = true; 
    public bool isFlashlight = false;
    public bool isGoBagItem = false;
    public bool isMedicalSupply = false;

    [Header("Audio Settings")]
    public AudioClip interactSound; 
    private AudioSource audioSource; 

    private Transform holdPosition;
    private Rigidbody rb;
    private Collider col;
    private bool isHeld = false;
    public bool IsHeld => isHeld;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        
        // Save all default materials when the game starts
        if (itemRenderer != null) originalMaterials = itemRenderer.materials;
        
        if (itemLight != null) itemLight.enabled = false;
        
        if (customHoldPosition != null) holdPosition = customHoldPosition;
        else
        {
            GameObject anchor = GameObject.Find("HoldAnchor");
            if (anchor == null) anchor = GameObject.Find("HoldPosition");
            if (anchor != null) holdPosition = anchor.transform;
        }
    }

    void Update()
    {
        if (canDrop && isHeld && VRInputConfig.InteractReleased())
        {
            Drop();
        }
    }

    public void OnGazeEnter() { }
    public void OnGazeExit() { }

    public void OnInteract()
    {
        if (interactSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(interactSound);
        }

        if (!isHeld) Pickup();
    }

    public void EnableHighlight()
    {
        // Append the outline material without deleting the original textures
        if (!isHeld && itemRenderer != null && highlightMaterial != null && originalMaterials != null)
        {
            // Safety check to prevent infinitely adding the material
            if (itemRenderer.materials.Length == originalMaterials.Length)
            {
                Material[] newMats = new Material[originalMaterials.Length + 1];
                for (int i = 0; i < originalMaterials.Length; i++)
                {
                    newMats[i] = originalMaterials[i];
                }
                newMats[originalMaterials.Length] = highlightMaterial;
                
                itemRenderer.materials = newMats;
            }
        }
    }

    void Pickup()
    {
        if (holdPosition == null) return;
        
        isHeld = true;
        rb.isKinematic = true;
        
        // Strip away the outline material once picked up
        if (itemRenderer != null && originalMaterials != null)
        {
            itemRenderer.materials = originalMaterials;
        }

        if (col != null) col.enabled = false;
        transform.position = holdPosition.position;
        transform.parent = holdPosition;
        transform.localRotation = Quaternion.identity;

        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

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
        if (col != null) col.enabled = true;

        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
}