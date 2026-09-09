using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void InsertVR()
    {
        SceneManager.LoadScene("Insert Into VR");
    }

    public void LevelSelect()
    {
        SceneManager.LoadScene("Level Selection");
    }

    public void OptionsPage()
    {
        SceneManager.LoadScene("Options Page");
    }

    public void HomeMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Application closed successfully.");
        Application.Quit();
    }
}