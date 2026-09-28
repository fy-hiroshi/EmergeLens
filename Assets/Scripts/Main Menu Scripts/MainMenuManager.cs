using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelEntry
    {
        public string scenarioName = "Earthquake";
        public string levelName = "Level 1 - Minor Tremors";
        public string sceneName;
        [Range(1, 3)] public int difficulty = 1;
        public Sprite badgeSprite;
        public Color badgeColor = Color.white; // Allows unique coloring per level
        public Sprite previewImage;
        [TextArea(3, 6)] public string description;
    }

    public static string levelToLoad;
    static int lastIndex = 0;

    [Header("Scenes")]
    public string vrInstructionSceneName = "Insert Into VR";
    public string backSceneName = "Main Menu";

    [Header("Levels (order = carousel order: Earthquake 1-3, then Fire 1-3)")]
    public LevelEntry[] levels;

    [Header("Content (the group that fades/slides between levels)")]
    public CanvasGroup contentGroup;
    public Image previewImage;
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    [Header("Difficulty Pips")]
    public Image[] difficultyPips;
    public Color pipFilledColor = new Color(0.91f, 0.64f, 0.09f, 1f);
    public Color pipEmptyColor = new Color(1f, 1f, 1f, 0.25f);

    [Header("Scenario Badge")]
    public RectTransform badgeRect;
    public Image badgeImage; 

    [Header("Dots Indicator")]
    public RectTransform dotsContainer;
    public Sprite dotSprite;
    public float dotSize = 16f;
    [Tooltip("Adds an extra gap after every N dots (3 = earthquake | fire). Set 0 for no gap.")]
    public int groupSize = 3;
    public float groupGap = 10f;
    public Color dotActiveColor = new Color(0.91f, 0.64f, 0.09f, 1f);
    public Color dotInactiveColor = new Color(1f, 1f, 1f, 0.35f);
    public float dotActiveScale = 1.4f;

    [Header("Transition")]
    public float slideDistance = 60f;
    public float fadeOutTime = 0.12f;
    public float fadeInTime = 0.22f;

    [Header("Buttons (drag them in, no OnClick setup needed)")]
    public Button previousButton;
    public Button nextButton;
    public Button backButton;
    public Button playButton;

    [Header("Options")]
    public bool wrapAround = true;

    int currentIndex;
    string shownScenario;
    RectTransform contentRect;
    Vector2 contentBasePos;
    Coroutine transitionRoutine;
    readonly List<Image> dots = new List<Image>();

    void Awake()
    {
        if (previousButton) previousButton.onClick.AddListener(Previous);
        if (nextButton)     nextButton.onClick.AddListener(Next);
        if (backButton)     backButton.onClick.AddListener(GoBack);
        if (playButton)     playButton.onClick.AddListener(PlaySelectedLevel);

        if (contentGroup)
        {
            contentRect = (RectTransform)contentGroup.transform;
            contentBasePos = contentRect.anchoredPosition;
        }
    }

    void Start()
    {
        if (levels == null || levels.Length == 0)
        {
            Debug.LogWarning("MainMenuManager: no levels assigned in the Inspector!");
            return;
        }

        BuildDots();
        SelectIndex(Mathf.Clamp(lastIndex, 0, levels.Length - 1));
        ApplyContent(levels[currentIndex]);
        AnimateDots(1f);
    }

    void Update()
    {
        AnimateDots(1f - Mathf.Exp(-14f * Time.unscaledDeltaTime));
    }

    // ---------------- Navigation ----------------

    public void Next()
    {
        if (levels == null || levels.Length == 0) return;

        int next = currentIndex + 1;
        if (next >= levels.Length)
        {
            if (!wrapAround) return;
            next = 0;
        }
        GoTo(next, +1);
    }

    public void Previous()
    {
        if (levels == null || levels.Length == 0) return;

        int prev = currentIndex - 1;
        if (prev < 0)
        {
            if (!wrapAround) return;
            prev = levels.Length - 1;
        }
        GoTo(prev, -1);
    }

    void GoTo(int index, int direction)
    {
        SelectIndex(index);

        if (contentGroup == null)
        {
            ApplyContent(levels[index]);
            return;
        }

        if (transitionRoutine != null) StopCoroutine(transitionRoutine);
        transitionRoutine = StartCoroutine(Transition(index, direction));
    }

    void SelectIndex(int index)
    {
        currentIndex = index;
        lastIndex = index;
        levelToLoad = levels[index].sceneName;

        if (!wrapAround)
        {
            if (previousButton) previousButton.interactable = index > 0;
            if (nextButton)     nextButton.interactable = index < levels.Length - 1;
        }

        Debug.Log("Level Selected: " + levelToLoad);
    }

    // ---------------- Visuals ----------------

    void ApplyContent(LevelEntry entry)
    {
        if (previewImage)
        {
            previewImage.sprite = entry.previewImage;
            previewImage.enabled = entry.previewImage != null;
        }

        if (titleText)       titleText.text = entry.levelName;
        if (descriptionText) descriptionText.text = entry.description;

        if (difficultyPips != null)
        {
            for (int i = 0; i < difficultyPips.Length; i++)
            {
                if (difficultyPips[i])
                    difficultyPips[i].color = i < entry.difficulty ? pipFilledColor : pipEmptyColor;
            }
        }

        if (badgeImage && entry.badgeSprite)
        {
            badgeImage.sprite = entry.badgeSprite; 
            badgeImage.color = entry.badgeColor; // Applies the selected color in the Inspector
        }
        shownScenario = entry.scenarioName;
    }

    IEnumerator Transition(int index, int direction)
    {
        LevelEntry entry = levels[index];
        if (badgeRect) badgeRect.localScale = Vector3.one;

        float startAlpha = contentGroup.alpha;
        Vector2 startPos = contentRect.anchoredPosition;
        Vector2 outPos = contentBasePos + new Vector2(-direction * slideDistance, 0f);

        for (float t = 0f; t < fadeOutTime; t += Time.unscaledDeltaTime)
        {
            float k = t / fadeOutTime;
            contentGroup.alpha = Mathf.Lerp(startAlpha, 0f, k);
            contentRect.anchoredPosition = Vector2.Lerp(startPos, outPos, k * k);
            yield return null;
        }
        contentGroup.alpha = 0f;

        bool scenarioChanged = entry.scenarioName != shownScenario;
        ApplyContent(entry);

        Vector2 inPos = contentBasePos + new Vector2(direction * slideDistance, 0f);

        for (float t = 0f; t < fadeInTime; t += Time.unscaledDeltaTime)
        {
            float k = t / fadeInTime;
            float eased = 1f - (1f - k) * (1f - k); 

            contentGroup.alpha = eased;
            contentRect.anchoredPosition = Vector2.Lerp(inPos, contentBasePos, eased);

            if (scenarioChanged && badgeRect)
                badgeRect.localScale = Vector3.one * Mathf.LerpUnclamped(0.6f, 1f, EaseOutBack(k));

            yield return null;
        }

        contentGroup.alpha = 1f;
        contentRect.anchoredPosition = contentBasePos;
        if (badgeRect) badgeRect.localScale = Vector3.one;
        transitionRoutine = null;
    }

    static float EaseOutBack(float k)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(k - 1f, 3f) + c1 * Mathf.Pow(k - 1f, 2f);
    }

    // ---------------- Dots ----------------

    void BuildDots()
    {
        if (!dotsContainer) return;

        for (int i = dotsContainer.childCount - 1; i >= 0; i--)
            Destroy(dotsContainer.GetChild(i).gameObject);
        dots.Clear();

        for (int i = 0; i < levels.Length; i++)
        {
            if (groupSize > 0 && i > 0 && i % groupSize == 0)
            {
                var spacer = new GameObject("Spacer", typeof(RectTransform));
                spacer.transform.SetParent(dotsContainer, false);
                ((RectTransform)spacer.transform).sizeDelta = new Vector2(groupGap, 1f);
            }

            var dotObj = new GameObject("Dot " + (i + 1), typeof(RectTransform), typeof(Image));
            dotObj.transform.SetParent(dotsContainer, false);
            ((RectTransform)dotObj.transform).sizeDelta = new Vector2(dotSize, dotSize);

            var img = dotObj.GetComponent<Image>();
            img.sprite = dotSprite;
            img.raycastTarget = false;
            img.color = dotInactiveColor;
            dots.Add(img);
        }
    }

    void AnimateDots(float k)
    {
        for (int i = 0; i < dots.Count; i++)
        {
            bool active = i == currentIndex;
            Color targetColor = active ? dotActiveColor : dotInactiveColor;
            Vector3 targetScale = Vector3.one * (active ? dotActiveScale : 1f);

            dots[i].color = Color.Lerp(dots[i].color, targetColor, k);
            dots[i].rectTransform.localScale = Vector3.Lerp(dots[i].rectTransform.localScale, targetScale, k);
        }
    }

    // ---------------- Buttons ----------------

    public void PlaySelectedLevel()
    {
        if (!string.IsNullOrEmpty(levelToLoad))
        {
            Debug.Log("Play clicked! Loading VR Instructions for: " + levelToLoad);
            SceneManager.LoadScene(vrInstructionSceneName);
        }
        else
        {
            Debug.LogWarning("No scene name set for this level in the Inspector!");
        }
    }

    public void GoBack()
    {
        SceneManager.LoadScene(backSceneName);
    }
}