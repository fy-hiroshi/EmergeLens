using UnityEngine;
using UnityEngine.UI;

public class CookingTask : MonoBehaviour, IInteractable
{
    [Header("Cooking Settings")]
    public float cookTime = 5.0f;
    public Image progressBarFill;
    public GameObject timerCanvas;
    public GrabbableItem garbageBagScript; 

    private float currentProgress = 0f;
    private bool isGazedAt = false;
    private bool taskComplete = false;

    void Start()
    {
        // Locks the garbage bag until cooking is done
        if (garbageBagScript != null) garbageBagScript.enabled = false;
        
        if (VRMessageUI.Instance != null)
            VRMessageUI.Instance.ShowMessage("Tend to the food you are cooking.", 5f);
    }

    void Update()
    {
        if (isGazedAt && !taskComplete)
        {
            // Bypasses string-based Input Manager to prevent crashes
            if (Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.E) || Input.GetMouseButton(0))
            {
                if (timerCanvas != null && !timerCanvas.activeSelf) timerCanvas.SetActive(true);
                
                currentProgress += Time.deltaTime;
                if (progressBarFill != null) progressBarFill.fillAmount = currentProgress / cookTime;

                if (currentProgress >= cookTime) CompleteCooking();
            }
            else
            {
                ResetTimer();
            }
        }
    }

    void ResetTimer()
    {
        currentProgress = 0f;
        if (progressBarFill != null) progressBarFill.fillAmount = 0f;
        if (timerCanvas != null && timerCanvas.activeSelf) timerCanvas.SetActive(false);
    }

    void CompleteCooking()
    {
        taskComplete = true;
        ResetTimer();
        
        if (VRMessageUI.Instance != null)
            VRMessageUI.Instance.ShowMessage("Food is done! Take the trash to the trash can.", 5f);
            
        if (garbageBagScript != null) garbageBagScript.enabled = true; 
    }

    public void OnGazeEnter() { isGazedAt = true; }
    public void OnGazeExit() { isGazedAt = false; ResetTimer(); }
    public void OnInteract() { }
}