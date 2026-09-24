using UnityEngine;

public class MobileGyroTracking : MonoBehaviour
{
    private bool gyroSupported;
    private Gyroscope gyro;

    void Start()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            gyroSupported = true;
        }
        else
        {
            Debug.LogWarning("Gyroscope not supported on this device.");
        }
    }

    void Update()
    {
        if (gyroSupported)
        {
            // Convert the right-handed Android gyro attitude to Unity's left-handed system
            Quaternion deviceRotation = new Quaternion(gyro.attitude.x, gyro.attitude.y, -gyro.attitude.z, -gyro.attitude.w);
            
            // Rotate the camera 90 degrees to account for the Landscape Left phone orientation
            transform.localRotation = Quaternion.Euler(90f, 0f, 0f) * deviceRotation;
        }
    }
}