using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections;

public class AutomaticSceneLoader : MonoBehaviour
{
    [Header("Transition Settings")]
    [Tooltip("Seconds to wait before automatically loading the next scene.")]
    public float timeToWait = 3.0f;
    
    [Tooltip("The exact name of the scene you want to load (e.g., EarthquakeLevel).")]
    public string sceneToLoad = "NextSceneNameHere"; 

    void Start()
    {
        // This runs automatically the moment the scene loads!
        StartCoroutine(BeginCountdown());
    }

    IEnumerator BeginCountdown()
    {
        // Pause the script for the set amount of seconds
        yield return new WaitForSeconds(timeToWait);
        
        // Load the target scene automatically
        SceneManager.LoadScene(sceneToLoad);
    }
}