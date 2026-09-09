using UnityEngine;

public class FireAlarm : MonoBehaviour, IInteractable
{
    [Header("Alarm Settings")]
    public AudioSource alarmAudio;
    public ExitDoor fireExitDoor;
    private bool isPulled = false;

    public void OnGazeEnter() { }
    public void OnGazeExit() { }

    public void OnInteract()
    {
        if (!isPulled)
        {
            isPulled = true;
            
            if (alarmAudio != null && !alarmAudio.isPlaying) 
                alarmAudio.Play();
                
            if (fireExitDoor != null) 
                fireExitDoor.isUnlocked = true;

            if (VRMessageUI.Instance != null)
                VRMessageUI.Instance.ShowMessage("Alarm activated! Evacuate through the fire exit.");
        }
    }
}