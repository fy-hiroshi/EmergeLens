using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class SlidingFurniture : MonoBehaviour
{
    [Header("Slide Settings")]
    public float minSlideForce = 2f; 
    public float maxSlideForce = 6f; 
    public float slideInterval = 0.5f; 

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void TriggerSlide(float duration)
    {
        StartCoroutine(SlideRoutine(duration));
    }

    IEnumerator SlideRoutine(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // Picks a random horizontal direction on the X and Z axes
            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
            float currentForce = Random.Range(minSlideForce, maxSlideForce);

            // Applies the physical push to scrape it across the floor
            rb.AddForce(randomDirection * currentForce, ForceMode.Impulse);
            
            yield return new WaitForSeconds(slideInterval);
            elapsed += slideInterval;
        }
    }
}