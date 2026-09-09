using UnityEngine;

public class GarbageTrigger : MonoBehaviour
{
    public GameObject steamParticles;
    public GameObject fireParticles;
    public AudioSource smokeAlarm;

    void OnTriggerEnter(Collider other)
    {
        // Checks if whatever fell in the bin has your grabbable script attached
        GrabbableItem item = other.GetComponentInParent<GrabbableItem>();
        
        if (item != null)
        {
            item.gameObject.SetActive(false); 
            if (steamParticles != null) steamParticles.SetActive(false);
            if (fireParticles != null) fireParticles.SetActive(true);
            if (smokeAlarm != null) smokeAlarm.Play();
            
            if (VRMessageUI.Instance != null)
                VRMessageUI.Instance.ShowMessage("Grease fire! Use the PASS method on the extinguisher.");
        }
    }
}