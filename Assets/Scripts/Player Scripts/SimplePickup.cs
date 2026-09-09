using UnityEngine;

public class SimplePickup : MonoBehaviour, IInteractable
{
    public void OnGazeEnter()
    {
        // Optional: Highlight mesh outline or sound
    }

    public void OnGazeExit()
    {
        // Optional: Remove outline
    }

    public void OnInteract()
    {
        Debug.Log("Item picked up: " + gameObject.name);
        Destroy(gameObject); // Simulates collecting the item
    }
}