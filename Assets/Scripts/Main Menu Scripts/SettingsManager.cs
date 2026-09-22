using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public Slider volumeSlider;

    void Start()
    {
        // 1. --- LOAD SAVED GRAPHICS ---
        // Check if the player has saved a graphics preference before
        if (PlayerPrefs.HasKey("GraphicsQuality"))
        {
            int savedQuality = PlayerPrefs.GetInt("GraphicsQuality");
            QualitySettings.SetQualityLevel(savedQuality, true);
        }

        // 2. --- LOAD SAVED VOLUME ---
        if (volumeSlider != null)
        {
            // If they have a saved volume, use it. Otherwise, default to 1.0 (max volume)
            float savedVolume = PlayerPrefs.HasKey("MasterVolume") ? PlayerPrefs.GetFloat("MasterVolume") : 1f;
            
            // Apply the volume to the game's AudioListener
            AudioListener.volume = savedVolume * 0.25f;
            
            // Sync the UI slider so the knob sits at the correct visual position
            volumeSlider.value = savedVolume;
            
            // Start listening for slider drags in real-time
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    // --- VOLUME LOGIC ---
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume * 0.25f;
        
        // Save the new volume value and force it to device storage
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save(); 
        
        Debug.Log("Master Volume saved as: " + volume);
    }

    // --- GRAPHICS LOGIC ---
    // 0 = Low, 1 = Medium, 2 = High 
    public void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
        
        // Save the new integer index and force it to device storage
        PlayerPrefs.SetInt("GraphicsQuality", qualityIndex);
        PlayerPrefs.Save();
        
        Debug.Log("Graphics Quality saved as: " + QualitySettings.names[qualityIndex]);
    }
}