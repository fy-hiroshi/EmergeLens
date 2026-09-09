using UnityEngine;

public class EarthquakeSafeZone : MonoBehaviour
{
    public bool isPlayerSafe = false;

    void OnTriggerEnter(Collider other)
    {
        // Checks if the object entering the zone is the Player
        if (other.CompareTag("Player"))
        {
            isPlayerSafe = true;
            Debug.Log("Player is safely taking cover under the desk!");
            // Future step: Link this to your scoring system or disable falling debris damage
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerSafe = false;
            Debug.Log("Player left the safe zone! Danger!");
        }
    }
}