using UnityEngine;

public interface IInteractable
{
    void OnGazeEnter();
    void OnGazeExit();
    void OnInteract();
}