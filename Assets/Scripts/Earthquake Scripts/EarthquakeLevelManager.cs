using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EarthquakeLevelManager : MonoBehaviour
{
    [Header("Core Scripts")]
    public VRControllerMovement mobileMovement; // Changed from MonoBehaviour
    public EditorVRTestingControls pcMovement;  // Changed from MonoBehaviour
    public EarthquakeSafeZone safeZone;
    public ExitDoor exitDoor;
    public LevelTwoChecklist checklist;
    public LevelTwoChecklist levelTwoManager;

    [Header("Camera & UI")]
    public Transform mainCamera;
    public Image qtePromptImage;
    public Sprite[] controllerButtonSprites; 
    public GameObject gameOverScreen;
    public GameObject alertUI;

    [Header("Level Timers")]
    public float timeToHide = 15f;
    public float shakeMagnitude = 0.15f;

    public bool isQuakeActive = false;
    private Vector3 originalCamPos;
    
    // Store original speeds so we can restore them later
    private float defaultMobileSpeed = 3f;
    private float defaultPCSpeed = 3.5f;
    

    void Start()
    {
        if (mobileMovement != null) defaultMobileSpeed = mobileMovement.walkSpeed;
        if (pcMovement != null) defaultPCSpeed = pcMovement.walkSpeed;
        
        StartCoroutine(LevelOneSequence());
    }

    IEnumerator LevelOneSequence()
    {
        // 1. Look and walk around freely for 5 seconds
        yield return new WaitForSeconds(5f);

        // 2. Earthquake Starts
        isQuakeActive = true;
        originalCamPos = mainCamera.localPosition;

        // NEW: Drop the debris right as the shaking begins
        if (checklist != null)
        {
            checklist.DropAllDebris();
        }

        StartCoroutine(CameraShakeRoutine());

        // 3. QTE Sequence (Freeze legs, keep head look active)
        SetWalkFrozen(true);
        yield return StartCoroutine(QTERoutine());

        // 4. Alert Mode (Restore walking)
        SetWalkFrozen(false);
        if (alertUI != null) alertUI.SetActive(true);

        // 5. Hide Timer Countdown
        float timer = timeToHide;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // 6. Check if player is safely under the desk
        if (!safeZone.isPlayerSafe)
        {
            GameOver();
            yield break; 
        }

        // 7. Survive the remaining quake
        yield return new WaitForSeconds(5f);
        
        isQuakeActive = false;
        mainCamera.localPosition = originalCamPos;
        if (alertUI != null) alertUI.SetActive(false);
        
        // NEW: Disable the safe zone so it stops throwing danger warnings
        if (safeZone != null) safeZone.gameObject.SetActive(false);
        
        // Shut off the lights for Level 2
        if (levelTwoManager != null) levelTwoManager.TriggerBlackout(); 
        
        // 8. Guarantee movement is unlocked to exit the door
        SetWalkFrozen(false);
        
        if (exitDoor != null) exitDoor.isUnlocked = true; 
        
        Debug.Log("Quake stopped! You can now walk out the door.");
    }
    IEnumerator QTERoutine()
    {
        int consecutiveHits = 0;
        KeyCode[] mobileKeys = { KeyCode.JoystickButton0, KeyCode.JoystickButton1, KeyCode.JoystickButton2, KeyCode.JoystickButton3 };
        KeyCode[] pcKeys = { KeyCode.S, KeyCode.D, KeyCode.A, KeyCode.W }; 

        while (consecutiveHits < 3)
        {
            int rndIndex = Random.Range(0, 4);
            if (controllerButtonSprites.Length > 0) 
                qtePromptImage.sprite = controllerButtonSprites[rndIndex % controllerButtonSprites.Length];
            
            qtePromptImage.gameObject.SetActive(true);

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
            
            qtePromptImage.gameObject.SetActive(false);
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

    void SetWalkFrozen(bool freeze)
    {
        // Sets speed to 0 to stop walking, or back to default to move again
        if (mobileMovement != null) mobileMovement.walkSpeed = freeze ? 0f : defaultMobileSpeed;
        if (pcMovement != null) pcMovement.walkSpeed = freeze ? 0f : defaultPCSpeed;
    }

    public void GameOver()
    {
        isQuakeActive = false;
        SetWalkFrozen(true);
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
    }
}