using UnityEngine;
using System.Collections;

public class InteractableDoor : MonoBehaviour, IInteractable
{
    [Header("Door Angles")]
    public float closedYAngle = 0f;
    public float openYAngle = -115f; 
    public float swingSpeed = 3f;

    private bool isOpen = false;
    private bool isSwinging = false; 

    public void OnGazeEnter() { }
    public void OnGazeExit() { }

    public void OnInteract()
    {
        // Prevent the player from breaking the math by clicking mid-swing
        if (!isSwinging)
        {
            StartCoroutine(ToggleDoorRoutine());
        }
    }

    IEnumerator ToggleDoorRoutine()
    {
        isSwinging = true;
        isOpen = !isOpen; // Flips the state between true and false
        
        // Decide which angle to target based on the new state
        float targetAngle = isOpen ? openYAngle : closedYAngle;
        
        Vector3 startAngles = transform.localEulerAngles;
        Vector3 endAngles = new Vector3(startAngles.x, targetAngle, startAngles.z);
        
        float time = 0;

        while (time < 1f)
        {
            time += Time.deltaTime * swingSpeed;
            transform.localRotation = Quaternion.Lerp(Quaternion.Euler(startAngles), Quaternion.Euler(endAngles), time);
            yield return null;
        }
        
        isSwinging = false;
    }
}