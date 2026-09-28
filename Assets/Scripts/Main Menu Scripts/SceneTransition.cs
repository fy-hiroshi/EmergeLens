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

    private bool isLoading = false;

    void Start()
    {
        // 1. Start the countdown automatically when the scene opens
        StartCoroutine(BeginCountdown());
    }

    void Update()
    {
        // 2. If the transition has already been triggered, ignore further inputs
        if (isLoading) return;

        // 3. Listen for manual input (controller, keyboard, mouse, or touch)
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Debug.Log("Manual input detected! Skipping timer.");
            TriggerTransition();
        }
    }

    IEnumerator BeginCountdown()
    {
        // Pause in the background for the set amount of seconds
        yield return new WaitForSeconds(timeToWait);
        
        // If the player hasn't pressed anything yet, load automatically
        if (!isLoading)
        {
            Debug.Log("Timer finished! Loading automatically.");
            TriggerTransition();
        }
    }

    void TriggerTransition()
    {
        isLoading = true; // Lock the input so the scene only loads once
        SceneManager.LoadScene(sceneToLoad);
    }
}