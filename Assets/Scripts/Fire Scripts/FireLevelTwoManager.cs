using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FireLevelTwoManager : MonoBehaviour
{
    [Header("Intro Sequence")]
    public GameObject bedModel;
    public Camera bedCamera;
    public GameObject mainPlayerRig;
    public Image blackScreenUI;
    public AudioSource smokeAlarmAudio;

    [Header("QTE Settings")]
    public Image qtePromptImage;
    public Sprite[] controllerButtonSprites; 
    public int requiredWakes = 3;

    [Header("Smoke Suffocation Meter")]
    public Image smokeMeterBar;
    public Transform playerCamera;
    public float smokeCeilingHeight = 1.2f; 
    public float baseFillRate = 0.02f;
    public float toxicFillRate = 0.15f;
    public GameObject gameOverScreen;
    
    private float currentSmokeToxicity = 0f;
    private bool isAwake = false;

    void Start()
    {
        mainPlayerRig.SetActive(false);
        blackScreenUI.gameObject.SetActive(true);
        StartCoroutine(WakeUpSequence());
    }

    void Update()
    {
        if (isAwake)
        {
            // Smoke fills slowly by default
            float currentRate = baseFillRate;

            // Punish the player rapidly if their camera rises into the smoke layer
            if (playerCamera.position.y >= smokeCeilingHeight)
            {
                currentRate = toxicFillRate;
            }

            currentSmokeToxicity += currentRate * Time.deltaTime;
            if (smokeMeterBar != null) smokeMeterBar.fillAmount = currentSmokeToxicity;

            if (currentSmokeToxicity >= 1.0f)
            {
                GameOver();
            }
        }
    }

    IEnumerator WakeUpSequence()
    {
        // 1. Sleep in darkness
        yield return new WaitForSeconds(4f);

        // 2. Trigger the alarms
        if (smokeAlarmAudio != null) smokeAlarmAudio.Play();

        // 3. Run the QTE to wake up
        yield return StartCoroutine(WakeUpQTE());

        // 4. Instantly remove the black screen to reveal the burning room from the bed
        if (blackScreenUI != null) blackScreenUI.gameObject.SetActive(false);
        
        // 5. Player lies on the bed for 3 seconds observing the fire
        yield return new WaitForSeconds(3f);
        
        // 6. Switch cameras and activate main player
        bedModel.SetActive(false);
        bedCamera.gameObject.SetActive(false);
        
        mainPlayerRig.SetActive(true);
        isAwake = true;
        
        // 7. Trigger the instructional warning immediately after standing up
        if (VRMessageUI.Instance != null) 
            VRMessageUI.Instance.ShowMessage("Stay low and evacuate! Toxic smoke gathers near the ceiling.", 3f);

        // 8. Wait 3 seconds for the player to read the warning
        yield return new WaitForSeconds(3f);
        
        // 9. Activate the smoke meter hazard
        if (smokeMeterBar != null) 
            smokeMeterBar.transform.parent.gameObject.SetActive(true);
    }

    IEnumerator WakeUpQTE()
    {
        int consecutiveHits = 0;
        // Keeping your exact hybrid mapping for PC testing[cite: 2]
        KeyCode[] mobileKeys = { KeyCode.JoystickButton0, KeyCode.JoystickButton1, KeyCode.JoystickButton2, KeyCode.JoystickButton3 };
        KeyCode[] pcKeys = { KeyCode.S, KeyCode.D, KeyCode.A, KeyCode.W }; 

        while (consecutiveHits < requiredWakes)
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

    public void GameOver()
    {
        isAwake = false;
        Debug.LogError("GAME OVER: Player succumbed to smoke inhalation.");
        
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
    }
}