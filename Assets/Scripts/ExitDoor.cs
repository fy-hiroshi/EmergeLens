using UnityEngine;

public class ExitDoor : MonoBehaviour, IInteractable
{
    [Header("Level 1 & 2 State")]
    public bool isUnlocked = false;
    public LevelCompleteManager levelManager; 
    public bool prematureExitIsFatal = false;
    public EarthquakeLevelManager earthquakeManager; 

    [Header("Fire Level 2 State (New)")]
    public bool requiresFireAlarm = false; 

    [Header("Level 3 State")]
    public bool isBlocked = false;
    public bool isFinalExit = false;
    public LevelThreeManager levelThreeManager; 
    public BagInventory playerGoBag; 

    public void OnGazeEnter() { }
    public void OnGazeExit() { }

    public void OnInteract()
    {
        // 1. Blocked door check (Level 3)
        if (isBlocked)
        {
            if (VRMessageUI.Instance != null) VRMessageUI.Instance.ShowMessage("This door is blocked by debris!");
            return;
        }

        // 2. Final Exit Decision Trigger (Level 3)
        if (isFinalExit)
        {
            if (playerGoBag != null && playerGoBag.currentItems >= playerGoBag.requiredItems)
            {
                if (levelThreeManager != null) levelThreeManager.TriggerEvacuationDecision();
            }
            else
            {
                if (VRMessageUI.Instance != null) VRMessageUI.Instance.ShowMessage("I need to pack my emergency Go-Bag first!");
            }
            return;
        }

        // 3. Normal Exit Logic (Level 1, Level 2, and completed Level 3)
        if (isUnlocked && levelManager != null)
        {
            levelManager.TriggerLevelComplete(); 
        }
        else
        {
            // NEW: Fire Level 2 early exit check
            if (requiresFireAlarm)
            {
                if (VRMessageUI.Instance != null) 
                    VRMessageUI.Instance.ShowMessage("Alert the whole building about the fire using the fire alarm first.");
            }
            // Existing Earthquake early exit check
            else if (prematureExitIsFatal && earthquakeManager != null)
            {
                Debug.LogError("GAME OVER: Player evacuated early!");
                earthquakeManager.GameOver();
            }
            else
            {
                if (VRMessageUI.Instance != null) VRMessageUI.Instance.ShowMessage("The door is jammed or it is not safe to leave yet!");
            }
        }
    }
}