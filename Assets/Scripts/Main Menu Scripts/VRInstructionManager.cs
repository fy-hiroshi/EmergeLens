using UnityEngine;
using UnityEngine.SceneManagement;

public class VRInstructionManager : MonoBehaviour
{
    void Update()
    {
        // Instantly abort the check if a finger is touching the phone screen
        if (Input.touchCount > 0) return;

        // Safely listen for Left Click (0), Right Click (1), or any keyboard/controller button
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            // Check if we actually have a saved level to load
            if (!string.IsNullOrEmpty(MainMenuManager.levelToLoad))
            {
                Debug.Log("Input detected! Loading Level: " + MainMenuManager.levelToLoad);
                SceneManager.LoadScene(MainMenuManager.levelToLoad);
            }
            else
            {
                Debug.LogWarning("No level was selected! Returning to Main Menu.");
                // Fallback scene name - change this if your menu scene is named differently
                SceneManager.LoadScene("Level Selection"); 
            }
        }
    }
}