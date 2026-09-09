using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class FallingDebris : MonoBehaviour
{
    [Header("Settings")]
    public float dropDelay = 0f;
    public bool pushOffShelf = false;
    
    [Header("Push Force")]
    public float minPushForce = 5f; 
    public float maxPushForce = 12f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Suspend the object initially
    }

    public void TriggerDrop()
    {
        StartCoroutine(DropRoutine());
    }

    IEnumerator DropRoutine()
    {
        yield return new WaitForSeconds(dropDelay);
        rb.isKinematic = false;
        rb.useGravity = true;

        if (pushOffShelf)
        {
            float pushForce = Random.Range(minPushForce, maxPushForce);
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            rb.AddForce(randomDirection * pushForce, ForceMode.Impulse);
        }
    }
}