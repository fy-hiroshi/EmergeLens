using UnityEngine;
using UnityEngine.SceneManagement;

public class VRInstructionManager : MonoBehaviour
{
    [Tooltip("Scene to return to if no level was selected")]
    public string levelSelectSceneName = "Level Selection";

    [Tooltip("Seconds to ignore input after this scene loads, so the Play click doesn't skip the instructions")]
    public float inputDelay = 0.5f;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < inputDelay) return;

        // Instantly abort the check if a finger is touching the phone screen
        if (Input.touchCount > 0) return;

        // Listen for Left Click (0), Right Click (1), or any keyboard/controller button
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (!string.IsNullOrEmpty(MainMenuManager.levelToLoad))
            {
                Debug.Log("Input detected! Loading Level: " + MainMenuManager.levelToLoad);
                SceneManager.LoadScene(MainMenuManager.levelToLoad);
            }
            else
            {
                Debug.LogWarning("No level was selected! Returning to Level Selection.");
                SceneManager.LoadScene(levelSelectSceneName);
            }
        }
    }
}