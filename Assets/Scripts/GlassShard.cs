using UnityEngine;
using UnityEngine.UI;

public class GlassShard : MonoBehaviour, IInteractable
{
    [Header("Extraction Settings")]
    public float extractionTime = 2.5f;
    public Image progressBarFill;
    public GameObject timerCanvas;
    
    [Header("NPC Link")]
    public InjuredNPC parentNPC;

    private float currentProgress = 0f;
    private bool isGazedAt = false;

    void Update()
    {
        if (isGazedAt)
        {
            if (Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.E) || Input.GetMouseButton(0))
            {
                if (timerCanvas != null && !timerCanvas.activeSelf) timerCanvas.SetActive(true);
                
                currentProgress += Time.deltaTime;
                if (progressBarFill != null) progressBarFill.fillAmount = currentProgress / extractionTime;

                if (currentProgress >= extractionTime)
                {
                    ExtractShard();
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
        currentProgress = 0f;
        if (progressBarFill != null) progressBarFill.fillAmount = 0f;
        if (timerCanvas != null && timerCanvas.activeSelf) timerCanvas.SetActive(false);
    }

    void ExtractShard()
    {
        ResetTimer();
        if (parentNPC != null) parentNPC.RemoveShard();
        Destroy(gameObject); 
    }

    public void OnGazeEnter() { isGazedAt = true; }
    public void OnGazeExit() { isGazedAt = false; ResetTimer(); }
    
    public void OnInteract() 
    {
        if (VRMessageUI.Instance != null) VRMessageUI.Instance.ShowMessage("Hold the button to extract the shard!");
    }
}