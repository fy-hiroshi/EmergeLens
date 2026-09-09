using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteManager : MonoBehaviour
{
    public GameObject levelCompleteUI;
    public string nextLevelName = "FireLevel"; 
    public string mainMenuName = "MainMenu";
    
    private bool isLevelComplete = false;

    void Update()
    {
        if (isLevelComplete)
        {
            // A Button (JoystickButton0) or Enter key for PC testing
            if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene(nextLevelName);
            }
            // B Button (JoystickButton1) or Escape key for PC testing
            else if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(mainMenuName);
            }
        }
    }

    public void TriggerLevelComplete()
    {
        isLevelComplete = true;
        if (levelCompleteUI != null) levelCompleteUI.SetActive(true);
    }
}