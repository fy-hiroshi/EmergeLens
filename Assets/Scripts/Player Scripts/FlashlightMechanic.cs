using UnityEngine;
using System.Collections;

public class FlashlightMechanic : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public Light spotLight;
    public float normalIntensity = 2.0f;
    public float minTimeBetweenFails = 15f;
    public float maxTimeBetweenFails = 35f;

    [Header("QTE Settings")]
    public GameObject qtePromptUI;
    public int mashesRequired = 5;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip flickerSound; // Played on every individual flicker
    public AudioClip mashSound;    // Played on every button mash while broken
    [Range(0f, 1f)] public float flickerVolume = 1f;
    [Range(0f, 1f)] public float mashVolume = 1f;

    private bool isBroken = false;
    private int currentMashes = 0;

    void Start()
    {
        StartCoroutine(FlashlightSurvivalRoutine());
    }

    void Update()
    {
        if (isBroken)
        {
            // Listen for Bluetooth Button Y, PC Left Click, or R key
            if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.R))
            {
                currentMashes++;

                if (audioSource != null && mashSound != null) audioSource.PlayOneShot(mashSound, mashVolume);

                if (currentMashes >= mashesRequired)
                {
                    FixFlashlight();
                }
            }
        }
    }

    IEnumerator FlashlightSurvivalRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenFails, maxTimeBetweenFails));
            
            if (!isBroken && spotLight != null && spotLight.enabled) 
            {
                // NEW: Call the flicker sequence instead of breaking instantly
                StartCoroutine(FlickerAndBreak());
            }
        }
    }

    // NEW: Replaces your old BreakFlashlight() method
    IEnumerator FlickerAndBreak()
    {
        // Randomly flicker the light 3 to 6 times
        int flickers = Random.Range(3, 6);
        for (int i = 0; i < flickers; i++)
        {
            if (audioSource != null && flickerSound != null) audioSource.PlayOneShot(flickerSound, flickerVolume);

            spotLight.intensity = 0.1f; 
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            spotLight.intensity = normalIntensity;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
        }

        // Lock it into the broken state and show the UI
        isBroken = true;
        currentMashes = 0;
        spotLight.intensity = 0.1f; 
        if (qtePromptUI != null) qtePromptUI.SetActive(true);
    }
    void FixFlashlight()
    {
        isBroken = false;
        spotLight.intensity = normalIntensity;
        if (qtePromptUI != null) qtePromptUI.SetActive(false);
    }
}