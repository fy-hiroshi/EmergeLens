using UnityEngine;
using System.Collections;

public class EarthquakeDoor : MonoBehaviour
{
    [Header("Swing Settings")]
    public float targetYAngle = -200f; // Type the exact Y rotation it needs to end at
    public float swingSpeed = 3f;

    private bool isOpen = false;

    public void SwingOpen()
    {
        if (!isOpen) StartCoroutine(OpenRoutine());
    }

    IEnumerator OpenRoutine()
    {
        isOpen = true;
        
        // Grabs the exact starting angles so X and Z never change
        Vector3 startAngles = transform.localEulerAngles;
        Vector3 endAngles = new Vector3(startAngles.x, targetYAngle, startAngles.z);
        
        float time = 0;

        while (time < 1f)
        {
            time += Time.deltaTime * swingSpeed;
            // Smoothly transitions between the two exact Y values
            transform.localRotation = Quaternion.Lerp(Quaternion.Euler(startAngles), Quaternion.Euler(endAngles), time);
            yield return null;
        }
    }
}