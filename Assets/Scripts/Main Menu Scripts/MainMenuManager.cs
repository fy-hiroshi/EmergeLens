using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // This remembers your choice in the background
    public static string levelToLoad; 
    public string vrInstructionSceneName = "Insert Into VR"; 

    // 1. Your Level Buttons (Minor Tremors, etc.) will trigger this
    public void SelectLevel(string levelName)
    {
        levelToLoad = levelName;
        Debug.Log("Level Selected: " + levelToLoad);
    }

    // 2. Your PLAY Button will trigger this
    public void PlaySelectedLevel()
    {
        // Safety check to ensure the player actually clicked a level first
        if (!string.IsNullOrEmpty(levelToLoad))
        {
            Debug.Log("Play clicked! Loading VR Instructions for: " + levelToLoad);
            SceneManager.LoadScene(vrInstructionSceneName);
        }
        else
        {
            Debug.LogWarning("No level selected yet!");
        }
    }
}