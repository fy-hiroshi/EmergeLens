using UnityEngine;

public class FireLevelOne : MonoBehaviour
{
    public GameObject levelCompleteUI;
    
    public void TriggerLevelComplete()
    {
        Debug.Log("Fire successfully extinguished!");
        if (levelCompleteUI != null) levelCompleteUI.SetActive(true);
    }
}