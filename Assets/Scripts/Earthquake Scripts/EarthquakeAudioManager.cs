using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class EarthquakeAudioManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource quakeAudio;
    public float maxVolume = 0.8f;
    public float fadeDuration = 2.0f;

    void Start()
    {
        if (quakeAudio == null) quakeAudio = GetComponent<AudioSource>();
        
        // Ensures the track loops endlessly and starts completely silent
        quakeAudio.loop = true;
        quakeAudio.volume = 0f;
    }

    public void StartRumble()
    {
        // Safety check: Only runs if you actually drag an audio file into the Inspector
        if (quakeAudio != null && quakeAudio.clip != null)
        {
            quakeAudio.Play();
            StartCoroutine(FadeVolume(0f, maxVolume, fadeDuration));
        }
    }

    public void StopRumble()
    {
        if (quakeAudio != null && quakeAudio.isPlaying)
        {
            StartCoroutine(FadeVolume(quakeAudio.volume, 0f, fadeDuration));
        }
    }

    IEnumerator FadeVolume(float startVol, float endVol, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            quakeAudio.volume = Mathf.Lerp(startVol, endVol, time / duration);
            yield return null;
        }
        
        // Physically stop the audio player once it fades to zero
        if (endVol <= 0f) quakeAudio.Stop();
    }
}