using UnityEngine;

public class InteractableDoor3: MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool isOpen = false;

    void Start()
    {
        closedRotation = transform.rotation;

        openRotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y + openAngle,
            transform.eulerAngles.z
        );
    }

    void Update()
    {
        Quaternion targetRotation = isOpen
            ? openRotation
            : closedRotation;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * openSpeed
        );
    }

    public void OnGazeEnter() { }
    public void OnGazeExit() { }

    public void OnInteract()
    {
        isOpen = !isOpen; // Always toggles: open if closed, close if open
    }
}