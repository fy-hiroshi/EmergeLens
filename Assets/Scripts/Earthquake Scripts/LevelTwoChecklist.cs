using UnityEngine;

public class LevelTwoChecklist : MonoBehaviour
{
    [Header("Environment")]
    public GameObject[] roomLights;
    public ExitDoor exitDoor; 
    public FallingDebris[] levelDebris;

    [Header("Survival Objectives")]
    public int itemsInBag = 0;
    public int totalItemsRequired = 3;
    public bool isBagCollected = false;
    public bool isBreakerOff = false;

    [Header("Final Objectives")]
    public BagInventory bagInventory;
    public BreakerBox mainBreaker;
    

    // NEW: Call this from your EarthquakeLevelManager when the shaking STARTS
    public void DropAllDebris()
    {
        foreach (FallingDebris debris in levelDebris)
        {
            if (debris != null) debris.TriggerDrop();
        }
    }

    // Call this from your existing EarthquakeLevelManager when the shaking STOPS
    public void TriggerBlackout()
    {
        foreach (GameObject lightObj in roomLights)
        {
            if (lightObj != null) lightObj.SetActive(false);
        }
        
        if (VRMessageUI.Instance != null) 
            VRMessageUI.Instance.ShowMessage("Power lost! Find a flashlight!");
    }

    void Update()
    {
        if (bagInventory != null && mainBreaker != null && exitDoor != null)
        {
            if (bagInventory.currentItems >= bagInventory.requiredItems && mainBreaker.isPowerOff)
            {
                exitDoor.isUnlocked = true; 
            }
        }
    }
}