using UnityEngine;

public class EvacuationChoice : MonoBehaviour, IInteractable
{
    public bool isCorrectChoice = false;
    public LevelCompleteManager levelManager; // Reusing your existing scene transition manager

    public void OnGazeEnter() { }
    public void OnGazeExit() { }

    public void OnInteract()
    {
        if (isCorrectChoice)
        {
            if (levelManager != null) levelManager.TriggerLevelComplete();
        }
        else
        {
            if (VRMessageUI.Instance != null) 
                VRMessageUI.Instance.ShowMessage("I don't think that's the right choice..");
        }
    }
}