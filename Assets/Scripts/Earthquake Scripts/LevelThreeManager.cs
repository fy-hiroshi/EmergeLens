using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelThreeManager : MonoBehaviour
{
    [Header("Core Scripts & References")]
    public MonoBehaviour mobileMovement; 
    public MonoBehaviour pcMovement;  
    public EarthquakeSafeZone[] safeZones; 
    public ExitDoor finalExitDoor; 

    [Header("Level 3 Environment")]
    public GameObject[] roomLights;          
    public FallingDebris[] levelDebris;  
    public EarthquakeDoor bedroomDoor;    
    public GameObject evacuationDecisionMatrix; 
    public SlidingFurniture[] slidingItems;

    [Header("Final Objectives")]
    public BagInventory bagInventory;
    public BreakerBox mainBreaker;
    public InjuredNPC injuredNPC;

    [Header("Camera & UI")]
    public Transform mainCamera;
    public Image qtePromptImage;
    public Sprite[] controllerButtonSprites; 
    public GameObject alertUI;
    public GameObject gameOverScreen;
    public GameObject levelCompleteScreen;

    [Header("Level Timers")]
    public float timeToHide = 15f;
    public float shakeMagnitude = 0.35f; 

    private bool isQuakeActive = false;
    private Vector3 originalCamPos;
    
    private float defaultMobileSpeed = 3f;
    private float defaultPCSpeed = 3.5f;

    [Header("Aftershock Settings")]
    public float minTimeBetweenShocks = 5f; // Lowered for rapid testing
    public float maxTimeBetweenShocks = 8f; // Lowered for rapid testing
    public float aftershockDuration = 3f;
    public float aftershockMagnitude = 0.1f;

    void Start()
    {
        SetWalkFrozen(true);
        StartCoroutine(LevelThreeSequence());
    }

    // 2. ADD THE UPDATE LOOP RIGHT HERE, BELOW START
    void Update()
    {
        if (bagInventory != null && mainBreaker != null && injuredNPC != null && finalExitDoor != null)
        {
            if (bagInventory.currentItems >= bagInventory.requiredItems && mainBreaker.isPowerOff && injuredNPC.shardsRemaining <= 0)
            {
                // Flips the specific lock your EvacuationTrigger is monitoring
                finalExitDoor.isUnlocked = true; 
            }
        }
    }

    IEnumerator LevelThreeSequence()
    {
        yield return new WaitForSeconds(3f);

        isQuakeActive = true;
        originalCamPos = mainCamera.localPosition;

        if (bedroomDoor != null) bedroomDoor.SwingOpen();

        foreach (FallingDebris debris in levelDebris)
        {
            if (debris != null) debris.TriggerDrop();
        }

        foreach (SlidingFurniture item in slidingItems)
        {
            if (item != null) item.TriggerSlide(15f);
        }

        StartCoroutine(CameraShakeRoutine());

        yield return StartCoroutine(QTERoutine());

        SetWalkFrozen(false);
        if (alertUI != null) alertUI.SetActive(true);

        float timer = timeToHide;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        bool hasSurvived = false;
        foreach (EarthquakeSafeZone zone in safeZones)
        {
            if (zone != null && zone.isPlayerSafe)
            {
                hasSurvived = true;
                break; 
            }
        }

        if (!hasSurvived)
        {
            GameOver();
            yield break; 
        }

        yield return new WaitForSeconds(6f);
        
        TriggerBlackout();

        isQuakeActive = false;
        mainCamera.localPosition = originalCamPos;
        if (alertUI != null) alertUI.SetActive(false);
        
        SetWalkFrozen(false);

        // NEW: This physically starts the aftershock loop!
        StartCoroutine(AftershockRoutine());
    }

    IEnumerator QTERoutine()
    {
        int consecutiveHits = 0;
        KeyCode[] mobileKeys = { KeyCode.JoystickButton0, KeyCode.JoystickButton1, KeyCode.JoystickButton2, KeyCode.JoystickButton3 };
        KeyCode[] pcKeys = { KeyCode.S, KeyCode.D, KeyCode.A, KeyCode.W }; 

        while (consecutiveHits < 3)
        {
            int rndIndex = Random.Range(0, 4);
            if (controllerButtonSprites.Length > 0 && qtePromptImage != null) 
                qtePromptImage.sprite = controllerButtonSprites[rndIndex % controllerButtonSprites.Length];
            
            if (qtePromptImage != null) qtePromptImage.gameObject.SetActive(true);

            bool hit = false;
            while (!hit)
            {
                if (Input.GetKeyDown(mobileKeys[rndIndex]) || Input.GetKeyDown(pcKeys[rndIndex]))
                {
                    consecutiveHits++;
                    hit = true;
                }
                else if (Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
                {
                    consecutiveHits = 0; 
                    hit = true;
                }
                yield return null;
            }
            
            if (qtePromptImage != null) qtePromptImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator CameraShakeRoutine()
    {
        while (isQuakeActive)
        {
            float x = originalCamPos.x + Random.Range(-1f, 1f) * shakeMagnitude;
            float y = originalCamPos.y + Random.Range(-1f, 1f) * (shakeMagnitude / 2f);
            float z = originalCamPos.z + Random.Range(-1f, 1f) * shakeMagnitude;
            mainCamera.localPosition = new Vector3(x, y, z);
            yield return null;
        }
    }

    IEnumerator AftershockRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenShocks, maxTimeBetweenShocks));

            if (VRMessageUI.Instance != null)
                VRMessageUI.Instance.ShowMessage("Aftershock! Get under cover!");
            
            if (alertUI != null) alertUI.SetActive(true);

            float hideTimer = 5f;
            while (hideTimer > 0)
            {
                hideTimer -= Time.deltaTime;
                yield return null;
            }

            bool hasSurvived = false;
            foreach (EarthquakeSafeZone zone in safeZones)
            {
                if (zone != null && zone.isPlayerSafe)
                {
                    hasSurvived = true;
                    break;
                }
            }

            if (!hasSurvived)
            {
                GameOver();
                yield break; 
            }

            float elapsed = 0f;
            originalCamPos = mainCamera.localPosition;

            foreach (SlidingFurniture item in slidingItems)
            {
                if (item != null) item.TriggerSlide(aftershockDuration);
            }

            while (elapsed < aftershockDuration)
            {
                float x = originalCamPos.x + Random.Range(-1f, 1f) * aftershockMagnitude;
                float y = originalCamPos.y + Random.Range(-1f, 1f) * (aftershockMagnitude / 2f);
                float z = originalCamPos.z + Random.Range(-1f, 1f) * aftershockMagnitude;
                mainCamera.localPosition = new Vector3(x, y, z);
                
                elapsed += Time.deltaTime;
                yield return null;
            }

            mainCamera.localPosition = originalCamPos;
            if (alertUI != null) alertUI.SetActive(false);
        }
    }

    public void TriggerBlackout()
    {
        foreach (GameObject lightObj in roomLights)
        {
            if (lightObj != null) lightObj.SetActive(false);
        }
        
        if (VRMessageUI.Instance != null) 
            VRMessageUI.Instance.ShowMessage("Power lost! Find a flashlight!");
    }

    public void TriggerEvacuationDecision()
    {
        if (evacuationDecisionMatrix != null)
        {
            evacuationDecisionMatrix.SetActive(true);
            if (VRMessageUI.Instance != null) 
                VRMessageUI.Instance.ShowMessage("Where should we evacuate to?");
        }
    }

    public void TriggerLevelComplete()
    {
        SetWalkFrozen(true);
        if (levelCompleteScreen != null) levelCompleteScreen.SetActive(true);
    }

    void SetWalkFrozen(bool freeze)
    {
        if (mobileMovement != null) mobileMovement.SendMessage("SetSpeed", freeze ? 0f : defaultMobileSpeed, SendMessageOptions.DontRequireReceiver);
        if (pcMovement != null) pcMovement.SendMessage("SetSpeed", freeze ? 0f : defaultPCSpeed, SendMessageOptions.DontRequireReceiver);
    }

    public void GameOver()
    {
        Debug.LogError("GAME OVER: Player failed a survival check!");
        isQuakeActive = false;
        SetWalkFrozen(true);
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
    }
}