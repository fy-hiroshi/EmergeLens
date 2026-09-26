using UnityEngine;
using UnityEngine.UI;

public class FireExtinguisher : MonoBehaviour
{
    public GrabbableItem grabScript; 
    public GameObject sprayParticles;
    public Transform fireHazard; 
    public Transform fireVisual; // The fire's mesh/particle system — assign this separately from fireHazard. This is what visually shrinks; fireHazard's collider stays full-size so it stays hittable throughout.
    public FireLevelOne levelManager; 
    public Transform aimSource; // Assign whatever transform represents the reticle's aim direction (e.g. CameraOffset, or a camera). Falls back to Camera.main if left empty — see note below if aimSource doesn't rotate with head look.
    public float aimForgiveness = 0.15f; // Radius of the spray "beam". Keeps the fire hittable even as its hitbox shrinks toward zero. Set to 0 for a pin-point raycast.
    public Light fireLight; // The fire's light source — its intensity fades to 0 alongside the shrinking flame.
    
    [Header("Spray UI")]
    public float timeToExtinguish = 3.0f;
    public Image progressBarFill;
    public GameObject timerCanvas;
    
    private float sprayProgress = 0f;
    private bool isUnlocked = false;
    private bool requiresRelease = false; // The safety catch
    private float initialLightIntensity;

    void Start()
    {
        if (fireLight != null) initialLightIntensity = fireLight.intensity;
    }

    public void UnlockExtinguisher()
    {
        isUnlocked = true;
        requiresRelease = true; // Arms the safety catch the moment the pin drops
        
        if (grabScript != null) grabScript.enabled = true; 
        gameObject.layer = LayerMask.NameToLayer("Interactable"); 
        
        if (VRMessageUI.Instance != null) 
            VRMessageUI.Instance.ShowMessage("Pin pulled! Grab it and aim at the fire base.");
    }

    void Update()
    {
        bool isSqueezingTrigger = VRInputConfig.InteractHeld();

        // Forces the player to physically let go of the button before spraying is allowed
        if (requiresRelease)
        {
            if (!isSqueezingTrigger) requiresRelease = false;
            return; 
        }

        if (isUnlocked && transform.parent != null && isSqueezingTrigger)
        {
            if (sprayParticles != null) sprayParticles.SetActive(true);

            Transform cam = aimSource != null ? aimSource : (Camera.main != null ? Camera.main.transform : null);

            RaycastHit hit;
            if (cam != null && Physics.SphereCast(cam.position, aimForgiveness, cam.forward, out hit, 8f))
            {
                if (hit.collider.transform == fireHazard || hit.collider.transform.IsChildOf(fireHazard))
                {
                    if (timerCanvas != null && !timerCanvas.activeSelf) timerCanvas.SetActive(true);

                    sprayProgress += Time.deltaTime;
                    if (progressBarFill != null) progressBarFill.fillAmount = sprayProgress / timeToExtinguish;
                    
                    Transform visual = fireVisual != null ? fireVisual : fireHazard;
                    visual.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, sprayProgress / timeToExtinguish);

                    if (fireLight != null) fireLight.intensity = Mathf.Lerp(initialLightIntensity, 0f, sprayProgress / timeToExtinguish);

                    if (sprayProgress >= timeToExtinguish)
                    {
                        if (timerCanvas != null) timerCanvas.SetActive(false);
                        fireHazard.gameObject.SetActive(false);
                        if (sprayParticles != null) sprayParticles.SetActive(false);
                        if (levelManager != null) levelManager.TriggerLevelComplete(); 
                        this.enabled = false; 
                    }
                    return; 
                }
            }
        }
        else
        {
            if (sprayParticles != null) sprayParticles.SetActive(false);
        }
        
        if (timerCanvas != null && timerCanvas.activeSelf) timerCanvas.SetActive(false);
    }
}