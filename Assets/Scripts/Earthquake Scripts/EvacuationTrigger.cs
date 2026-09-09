using UnityEngine;

public class EvacuationTrigger : MonoBehaviour
{
    public GameObject[] choiceTextObjects; 
    public ExitDoor exitDoor; // NEW: Track the physical door's lock state
    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        // NEW: Only trigger if the player steps in AND the door is unlocked
        if (!hasTriggered && other.CompareTag("Player") && exitDoor != null && exitDoor.isUnlocked)
        {
            hasTriggered = true;
            
            if (VRMessageUI.Instance != null)
                VRMessageUI.Instance.ShowMessage("Choose a proper evacuation site.");

            foreach (GameObject choice in choiceTextObjects)
            {
                if (choice != null) choice.SetActive(true);
            }
        }
    }
}