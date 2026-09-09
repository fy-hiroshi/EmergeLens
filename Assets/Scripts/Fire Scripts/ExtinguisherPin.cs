using UnityEngine;
using UnityEngine.UI;

public class ExtinguisherPin : MonoBehaviour, IInteractable
{
    public float pullTime = 2.0f;
    public Image progressBarFill; 
    public GameObject timerCanvas; 
    public FireExtinguisher mainExtinguisher;

    private float currentProgress = 0f;
    private bool isGazedAt = false;

    void Update()
    {
        if (isGazedAt && (Input.GetKey(KeyCode.JoystickButton0) || Input.GetKey(KeyCode.E) || Input.GetMouseButton(0)))
        {
            if (timerCanvas != null && !timerCanvas.activeSelf) timerCanvas.SetActive(true);
            
            currentProgress += Time.deltaTime;
            if (progressBarFill != null) progressBarFill.fillAmount = currentProgress / pullTime;

            if (currentProgress >= pullTime)
            {
                if (mainExtinguisher != null) mainExtinguisher.UnlockExtinguisher();
                Destroy(gameObject); // Now this only destroys the tiny pin
            }
        }
        else
        {
            currentProgress = 0f;
            if (progressBarFill != null) progressBarFill.fillAmount = 0f; 
            if (timerCanvas != null && timerCanvas.activeSelf) timerCanvas.SetActive(false); 
        }
    }

    public void OnGazeEnter() { isGazedAt = true; } 
    public void OnGazeExit() { isGazedAt = false; } 
    public void OnInteract() { }

    // Guarantees the canvas turns off the exact frame the pin is deleted
    void OnDestroy()
    {
        if (timerCanvas != null && timerCanvas.activeSelf) timerCanvas.SetActive(false);
    }
}