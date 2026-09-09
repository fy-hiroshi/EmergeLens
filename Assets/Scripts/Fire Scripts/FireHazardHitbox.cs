using UnityEngine;

public class FireHazardHitbox : MonoBehaviour
{
    [Header("Manager Reference")]
    public FireLevelTwoManager levelManager;

    void OnTriggerEnter(Collider other)
    {
        // Checks if the object stepping into the fire is explicitly tagged as the Player
        if (other.CompareTag("Player"))
        {
            if (VRMessageUI.Instance != null)
            {
                VRMessageUI.Instance.ShowMessage("You got too close to the flames!");
            }

            if (levelManager != null)
            {
                levelManager.GameOver();
            }
            else
            {
                Debug.LogError("GAME OVER: Burned, but FireLevelTwoManager is missing!");
            }
        }
    }
}