using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class VRMessageUI : MonoBehaviour
{
    public static VRMessageUI Instance;
    public TextMeshProUGUI messageText; 

    void Awake() 
    { 
        Instance = this; 
    }

    public void ShowMessage(string text, float duration = 4f)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayRoutine(text, duration));
    }

    IEnumerator DisplayRoutine(string text, float duration)
    {
        messageText.text = text;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        messageText.gameObject.SetActive(false);
    }
}