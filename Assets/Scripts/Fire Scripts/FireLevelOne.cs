using UnityEngine;

public class FireLevelOne : MonoBehaviour
{
    [Header("Transition Manager")]
    public LevelCompleteManager levelCompleteManager; 
    
    public void TriggerLevelComplete()
    {
        Debug.Log("Fire successfully extinguished!");
        
        // Tells your master transition script to take over and spawn the UI
        if (levelCompleteManager != null) 
        {
            levelCompleteManager.TriggerLevelComplete();
        }
        else
        {
            Debug.LogWarning("Level Complete Manager is missing from the Inspector!");
        }
    }
}