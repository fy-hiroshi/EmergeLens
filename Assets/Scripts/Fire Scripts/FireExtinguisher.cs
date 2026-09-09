using UnityEngine;
using UnityEngine.UI;

public class FireExtinguisher : MonoBehaviour
{
    public GrabbableItem grabScript; 
    public GameObject sprayParticles;
    public Transform fireHazard; 
    public FireLevelOne levelManager; 
    
    [Header("Spray UI")]
    public float timeToExtinguish = 3.0f;
    public Image progressBarFill;
    public GameObject timerCanvas;
    
    private float sprayProgress = 0f;
    private bool isUnlocked = false;
    private bool requiresRelease = false; // NEW: The safety catch

    public void UnlockExtinguisher()
    {
        isUnlocked = true;
        requiresRelease = true; // NEW: Arms the safety catch the moment the pin drops
        
        if (grabScript != null) grabScript.enabled = true; 
        gameObject.layer = LayerMask.NameToLayer("Interactable"); 
        
        if (VRMessageUI.Instance != null) 
            VRMessageUI.Instance.ShowMessage("Pin pulled! Grab it and aim at the fire base.");
    }

    void Update()
    {
        bool isSqueezingTrigger = Input.GetMouseButton(0) || Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.E);

        // NEW: Forces the player to physically let go of the button before spraying is allowed
        if (requiresRelease)
        {
            if (!isSqueezingTrigger) requiresRelease = false;
            return; 
        }

        if (isUnlocked && transform.parent != null && isSqueezingTrigger)
        {
            if (sprayParticles != null) sprayParticles.SetActive(true);

            RaycastHit hit;
            if (Camera.main != null && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 8f))
            {
                if (hit.collider.transform == fireHazard || fireHazard.IsChildOf(hit.collider.transform))
                {
                    if (timerCanvas != null && !timerCanvas.activeSelf) timerCanvas.SetActive(true);

                    sprayProgress += Time.deltaTime;
                    if (progressBarFill != null) progressBarFill.fillAmount = sprayProgress / timeToExtinguish;
                    
                    fireHazard.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, sprayProgress / timeToExtinguish);

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