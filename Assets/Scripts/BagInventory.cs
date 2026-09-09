using UnityEngine;

public class BagInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int requiredItems = 3;
    public GrabbableItem bagGrabbableScript; 

    public int currentItems = 0;

    void OnTriggerEnter(Collider other)
    {
        GrabbableItem item = other.GetComponent<GrabbableItem>();

        if (item != null && item.canDrop && item.isGoBagItem)
        {
            currentItems++;
            string itemName = other.gameObject.name; 
            
            if (VRMessageUI.Instance != null)
            {
                // Checks if this is the final required item
                if (currentItems >= requiredItems)
                {
                    VRMessageUI.Instance.ShowMessage("Bag full! Turn off the main breaker before grabbing the go-bag.");
                }
                else
                {
                    VRMessageUI.Instance.ShowMessage(itemName + " has been collected! (" + currentItems + "/" + requiredItems + ")");
                }
            }

            other.gameObject.SetActive(false);

            if (currentItems >= requiredItems) CompleteBag();
        }
    }
    
    void CompleteBag()
    {
        if (VRMessageUI.Instance != null)
        {
            VRMessageUI.Instance.ShowMessage("Go bag complete. Turn off the breaker first.");
        }
        // We removed the bag unlock logic here!
    }
}