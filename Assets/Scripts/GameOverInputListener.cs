using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverInputListener : MonoBehaviour
{
    [Header("Scene Navigation")]
    public string mainMenuName = "MainMenu"; // Ensure this matches your menu scene perfectly

    void Update()
    {
        // A Button (JoystickButton0) or Enter key for PC testing[cite: 4]
        if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Return))
        {
            // Automatically detects whatever level you are currently playing and reloads it
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        // B Button (JoystickButton1) or Escape key for PC testing[cite: 4]
        else if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(mainMenuName);
        }
    }
}