using UnityEngine;
using System.Collections;

public class EarthquakeEventManager : MonoBehaviour
{
    [Header("Earthquake Settings")]
    public Transform shakeTarget; // Drag your Main Camera here
    public float shakeDuration = 10f;
    public float shakeMagnitude = 0.15f;

    private Vector3 originalPos;
    private bool isEarthquakeActive = false;

    void Update()
    {
        // Debug trigger: Press F1 to start the quake
        if (Input.GetKeyDown(KeyCode.F1) && !isEarthquakeActive)
        {
            StartCoroutine(StartEarthquake());
        }
    }

    IEnumerator StartEarthquake()
    {
        isEarthquakeActive = true;
        Debug.Log("Earthquake started! Drop, Cover, and Hold On!");
        
        // Lock in the camera's normal local position (usually 0,0,0)
        originalPos = shakeTarget.localPosition;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            // Generate violent random rumbling
            float x = originalPos.x + (Random.Range(-1f, 1f) * shakeMagnitude);
            float y = originalPos.y + (Random.Range(-1f, 1f) * (shakeMagnitude / 2f)); // Subtle vertical bounce
            float z = originalPos.z + (Random.Range(-1f, 1f) * shakeMagnitude);

            shakeTarget.localPosition = new Vector3(x, y, z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap the camera back to center when the shaking stops
        shakeTarget.localPosition = originalPos;
        isEarthquakeActive = false;
        Debug.Log("The shaking has stopped. Prepare for aftershocks.");
    }
}