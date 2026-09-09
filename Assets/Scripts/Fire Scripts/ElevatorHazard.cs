using UnityEngine;

public class ElevatorHazard : MonoBehaviour, IInteractable
{
    [Header("Fatal Hazard")]
    public FireLevelTwoManager levelManager;

    public void OnGazeEnter() { }
    public void OnGazeExit() { }

    public void OnInteract()
    {
        if (VRMessageUI.Instance != null)
        {
            VRMessageUI.Instance.ShowMessage("Never use an elevator during a fire!");
        }

        if (levelManager != null)
        {
            levelManager.GameOver(); // Triggers the existing Game Over screen and paralyzes the player[cite: 1, 3]
        }
        else
        {
            Debug.LogError("GAME OVER: Player used the elevator, but LevelManager reference is missing!");
        }
    }
}