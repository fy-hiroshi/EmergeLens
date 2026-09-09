using UnityEngine;

public class BreakerBox : MonoBehaviour, IInteractable
{
    [Header("State")]
    public bool isPowerOff = false;
    public bool isLevelThree = false; // Check this ONLY in your Level 3 scene

    public MeshRenderer indicatorLight; 
    public Material offMaterial;
    public BagInventory bagInventory;

    public void OnGazeEnter() { }
    public void OnGazeExit() { }

    public void OnInteract()
    {
        // Level 2 Constraint: Block interaction if the bag isn't fully packed yet
        if (!isLevelThree && bagInventory != null && bagInventory.currentItems < bagInventory.requiredItems)
        {
            if (VRMessageUI.Instance != null) VRMessageUI.Instance.ShowMessage("Pack the go bag first!");
            return;
        }

        if (!isPowerOff)
        {
            isPowerOff = true;
            // (Keep your existing material swap logic here if you have it)
            
            if (isLevelThree)
            {
                // Level 3 Sequence: Direct the player to the NPC
                if (VRMessageUI.Instance != null)
                {
                    VRMessageUI.Instance.ShowMessage("Breaker turned off. Cure the injured NPC first before grabbing the go bag.");
                }
            }
            else
            {
                // Level 2 Sequence: Unlock the bag and direct the player to evacuate
                if (bagInventory != null && bagInventory.bagGrabbableScript != null)
                {
                    bagInventory.bagGrabbableScript.enabled = true;
                    bagInventory.bagGrabbableScript.gameObject.layer = LayerMask.NameToLayer("Interactable");
                }

                if (VRMessageUI.Instance != null)
                {
                    VRMessageUI.Instance.ShowMessage("Breaker turned off. Grab the go bag and evacuate.");
                }
            }
        }
    }
}