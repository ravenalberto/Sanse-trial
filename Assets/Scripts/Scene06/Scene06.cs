using System.Collections;
using System.Collections.Generic;
using TMP_Text = TMPro.TMP_Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene06 : MonoBehaviour
{
    private Dictionary<string, int> trustScores = new Dictionary<string, int>();

    public void AddTrust(string characterName, int amount)
    {
        if (!trustScores.ContainsKey(characterName)) trustScores[characterName] = 0;
        trustScores[characterName] += amount;
        Debug.Log($"<color=cyan>Trust/Affection Update:</color> {characterName} is now at {trustScores[characterName]}");
    }

    private Coroutine typingCoroutine;
    private Coroutine bgFadeCoroutine;
    private Coroutine fadeCoroutine;

    public float textSpeed = 0.02f;
    public bool skipMode = false;

    private DialogueLine currentLine;
    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private Queue<DialogueLine> tempQueue = new Queue<DialogueLine>();
    private bool usingTempQueue = false;
    private bool isTyping = false;

    [Header("UI Components")]
    public GameObject textBox;
    public TMP_Text charNameText;
    public TMP_Text dialogueText;
    public GameObject nextButton;
    public GameObject choicePanelApproach; // Choice panel for Marc's approach style
    public GameObject choicePanelWhy;      // Choice panel for "So why didn't you do anything?" on Rooftop
    public CanvasGroup fadeCanvasGroup;

    [Header("Backgrounds")]
    public GameObject currentBG;
    public GameObject highSchoolCanteenBG;
    public GameObject highSchoolClassroomBG;
    public GameObject blackBG;
    public GameObject SchoolBG;
    public GameObject rooftopBG;           // Sunset rooftop background

    [Header("SHS Portraits")]
    public GameObject marcNeutral;
    public GameObject marcLaugh;
    public GameObject marcChide;
    public GameObject highschoolCristel; // Using SHS Cristel portrait

    [Header("College Portraits")]
    public GameObject collegeMarcNeutral;
    public GameObject collegeMarcLaugh;
    public GameObject collegeMarcChide;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioClip bellSFX;
    public AudioClip glitchSFX;
    public AudioClip bgFadeSFX;
    public AudioClip tearSniffleSFX;

    // Structure to hold default portrait state for crisp coordinate restoration
    private struct PortraitLayoutState
    {
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
        public Vector2 anchoredPosition;
        public Vector3 localScale;
    }

    private Dictionary<GameObject, PortraitLayoutState> originalMarcLayouts = new Dictionary<GameObject, PortraitLayoutState>();

    void Start()
    {
        // Force unlock mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Start scene at the high school canteen flashback
        if (SchoolBG != null)
        {
            currentBG = SchoolBG;
            SchoolBG.SetActive(true);
        }

        // Safe UI resets
        if (textBox != null) textBox.SetActive(false);
        if (choicePanelApproach != null) choicePanelApproach.SetActive(false);
        if (choicePanelWhy != null) choicePanelWhy.SetActive(false);
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 0;

        // Cache original layouts of Marc's portraits for fourth-wall warping
        CacheMarcLayouts();

        EnqueueScene06();
        ShowNextLine();
    }

    private void CacheMarcLayouts()
    {
        GameObject[] marcPortraits = { marcNeutral, marcLaugh, marcChide, collegeMarcNeutral, collegeMarcLaugh, collegeMarcChide };
        foreach (var portrait in marcPortraits)
        {
            if (portrait != null)
            {
                RectTransform rect = portrait.GetComponent<RectTransform>();
                if (rect != null)
                {
                    PortraitLayoutState state = new PortraitLayoutState
                    {
                        anchorMin = rect.anchorMin,
                        anchorMax = rect.anchorMax,
                        pivot = rect.pivot,
                        anchoredPosition = rect.anchoredPosition,
                        localScale = rect.localScale
                    };
                    originalMarcLayouts[portrait] = state;
                }
            }
        }
    }

    private void SetMarcToCenter(bool isCenter)
    {
        GameObject[] marcPortraits = { marcNeutral, marcLaugh, marcChide, collegeMarcNeutral, collegeMarcLaugh, collegeMarcChide };
        foreach (var portrait in marcPortraits)
        {
            if (portrait != null)
            {
                RectTransform rect = portrait.GetComponent<RectTransform>();
                if (rect != null)
                {
                    if (isCenter)
                    {
                        // Break the fourth wall: Force Marc straight to screen center and scale him up
                        rect.anchorMin = new Vector2(0.5f, 0.5f);
                        rect.anchorMax = new Vector2(0.5f, 0.5f);
                        rect.pivot = new Vector2(0.5f, 0.5f);
                        rect.anchoredPosition = new Vector2(0f, -80f); // Positioned slightly down to keep gaze leveled
                        rect.localScale = new Vector3(1.35f, 1.35f, 1f); // Intimidating/Intimate zoom
                    }
                    else
                    {
                        // Restore native inspector configuration layout
                        if (originalMarcLayouts.TryGetValue(portrait, out PortraitLayoutState defaultLayout))
                        {
                            rect.anchorMin = defaultLayout.anchorMin;
                            rect.anchorMax = defaultLayout.anchorMax;
                            rect.pivot = defaultLayout.pivot;
                            rect.anchoredPosition = defaultLayout.anchoredPosition;
                            rect.localScale = defaultLayout.localScale;
                        }
                    }
                }
            }
        }
    }

    void Update()
    {
        skipMode = Input.GetKey(KeyCode.LeftControl);

        bool isAnyChoiceActive = (choicePanelApproach != null && choicePanelApproach.activeSelf) ||
                                 (choicePanelWhy != null && choicePanelWhy.activeSelf);

        if (skipMode && !isTyping && !isAnyChoiceActive) ShowNextLine();
    }

    void EnqueueScene06()
    {
        // --- ACT I: THE TOMBOY MORENA GIRL ---
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_SCHOOL]"));
        dialogueQueue.Enqueue(new DialogueLine("", "..."));
        dialogueQueue.Enqueue(new DialogueLine("", "I was just minding my own business back then..."));
        dialogueQueue.Enqueue(new DialogueLine("", "But his eyes always drifted towards a specific short-haired, tomboy morena girl who walked around with a certain maangas vibe."));

        dialogueQueue.Enqueue(new DialogueLine("", "Napapansin ko, she walked behind them a lot. Parang laging buntot sa sarili niyang grupo..."));
        dialogueQueue.Enqueue(new DialogueLine("", "He occasionally saw her sitting alone or quietly watching people in the school canteen."));
        dialogueQueue.Enqueue(new DialogueLine("", "Maybe I had a small crush on her. Who wouldn't? Kaso... I already knew my other friends liked her too. Kaya nanahimik na lang ako."));

        // --- ACT II: AFTER THE BELL RINGS ---
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[SFX_BELL]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_CLASSROOM]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));

        dialogueQueue.Enqueue(new DialogueLine("", "Then that day came. The end-of-day bell rang, echoing through the old hallways."));
        dialogueQueue.Enqueue(new DialogueLine("", "Everyone packed their bags and hurriedly left for home, laughing and shouting."));
        dialogueQueue.Enqueue(new DialogueLine("", "But as the classroom emptied, the familiar short-haired girl remained behind. She sat quietly at her desk, looking down."));
        dialogueQueue.Enqueue(new DialogueLine("", "I took it as my only chance to finally approach her. No friends around to judge."));

        // Choice 9: Approaching Cristel
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE_APPROACH]"));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[SFX_SNIFFLE]"));
        dialogueQueue.Enqueue(new DialogueLine("", "The girl slowly looks up. Her eyes are red, glassy with unshed tears, and she gently sniffles.", highschoolCristel));

        dialogueQueue.Enqueue(new DialogueLine("Marc", "Uy! Ano nangyari baks? Okay ka lang?", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("", "The girl blinks, startled by his sudden shift in tone, and suddenly lets out a small laugh.", highschoolCristel));
        dialogueQueue.Enqueue(new DialogueLine("", "Ahaha, oo... okay lang ako.", highschoolCristel));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Alam mo? Isa ka sa nakilala kong pinaka magaling magsinungaling.", marcChide));
        dialogueQueue.Enqueue(new DialogueLine("", "The girl snorts."));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Ah so may kilala ka pang iba?", highschoolCristel));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Depende, ano ba pangalan mo?", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "...Cristel", highschoolCristel));

        // --- ACT III: ROOFTOP TRUTH (FOURTH WALL BREAK) ---
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_ROOFTOP]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[MARC_CENTER]")); // Position Marc directly facing the player
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));

        dialogueQueue.Enqueue(new DialogueLine("Marc", "Yeah so that’s actually what happened.", collegeMarcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "It’s that easy, she was bullied, depressed and suicidal. I knew it all along. And I admit to being responsible. But as well as others.", collegeMarcChide));

        // Choice 10: "So why didn't you do anything?"
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE_WHY]"));

        dialogueQueue.Enqueue(new DialogueLine("Marc", "What was I supposed to do? She pushed me away.", collegeMarcChide));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Enough of this. She’s gone but none of them know, because they couldn’t accept it.", collegeMarcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Now, it’s up to you to decide to how to end this.", collegeMarcNeutral));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[MARC_NORMAL]")); // Restore Marc's layout state before leaving
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[GOTO_MARC_TEST]"));
    }

    public void OnApproachSelected(int index)
    {
        if (choicePanelApproach != null) choicePanelApproach.SetActive(false);
        usingTempQueue = true;
        tempQueue.Clear();

        if (index == 0) // Choice 9A: Playful / Gay Persona Approach
        {
            AddTrust("Cristel", 15);
            tempQueue.Enqueue(new DialogueLine("Marc", "Hi bonita, di ka pa nauwi girl?", marcLaugh));
            tempQueue.Enqueue(new DialogueLine("", "He pretended to sound gay, pitching his voice up slightly. He figured it was the best way to make her feel comfortable."));
        }
        else // Choice 9B: Gentle / Direct Approach
        {
            AddTrust("Cristel", 10);
            tempQueue.Enqueue(new DialogueLine("Marc", "Hey... okay ka lang? Bakit andito ka pa?", marcNeutral));
            tempQueue.Enqueue(new DialogueLine("", "He spoke softly, hoping his sudden presence wouldn't startle her away."));
        }

        ShowNextLine();
    }

    public void OnWhySelected()
    {
        if (choicePanelWhy != null) choicePanelWhy.SetActive(false);
        ShowNextLine();
    }

    public void OnNextClick()
    {
        bool isAnyChoiceActive = (choicePanelApproach != null && choicePanelApproach.activeSelf) ||
                                 (choicePanelWhy != null && choicePanelWhy.activeSelf);

        if (isAnyChoiceActive) return; // Block skip input when choosing

        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = currentLine.text;
            isTyping = false;
            nextButton.SetActive(true);
            return;
        }
        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (dialogueQueue.Count == 0 && tempQueue.Count == 0) return;

        bool isAnyChoiceActive = (choicePanelApproach != null && choicePanelApproach.activeSelf) ||
                                 (choicePanelWhy != null && choicePanelWhy.activeSelf);

        if (isAnyChoiceActive) return; // Pause processing queue

        DialogueLine line;
        if (usingTempQueue && tempQueue.Count > 0)
        {
            line = tempQueue.Dequeue();
            if (tempQueue.Count == 0 && line.speaker != "SYSTEM") usingTempQueue = false;
        }
        else
        {
            line = dialogueQueue.Dequeue();
        }

        if (line.speaker == "SYSTEM")
        {
            if (!HandleSystemCommand(line.text)) ShowNextLine();
            return;
        }

        currentLine = line;
        if (textBox != null) textBox.SetActive(true);

        HideAllPortraits();
        if (line.portrait != null) line.portrait.SetActive(true);
        charNameText.text = line.speaker;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(line.text));
    }

    private bool HandleSystemCommand(string command)
    {
        switch (command)
        {
            case "[CHOICE_APPROACH]":
                if (choicePanelApproach != null) choicePanelApproach.SetActive(true);
                if (nextButton != null) nextButton.SetActive(false);
                return true;
            case "[CHOICE_WHY]":
                if (choicePanelWhy != null) choicePanelWhy.SetActive(true);
                if (nextButton != null) nextButton.SetActive(false);
                return true;
            case "[SFX_BELL]":
                if (sfxSource && bellSFX) sfxSource.PlayOneShot(bellSFX);
                return false;
            case "[SFX_SNIFFLE]":
                if (sfxSource && tearSniffleSFX) sfxSource.PlayOneShot(tearSniffleSFX);
                return false;
            case "[SFX_GLITCH]":
                if (sfxSource && glitchSFX) sfxSource.PlayOneShot(glitchSFX);
                return false;
            case "[MARC_CENTER]":
                SetMarcToCenter(true);
                return false;
            case "[MARC_NORMAL]":
                SetMarcToCenter(false);
                return false;
            case "[FADE_OUT]":
                StartFade(1.0f);
                return true;
            case "[FADE_IN]":
                StartFade(0.2f);
                return true;
            case "[BG_CANTEEN]": StartBGTransition(highSchoolCanteenBG); return false;
            case "[BG_CLASSROOM]": StartBGTransition(highSchoolClassroomBG); return false;
            case "[BG_BLACK]": StartBGTransition(blackBG); return false;
            case "[BG_SCHOOL]": StartBGTransition(SchoolBG); return false;
            case "[BG_ROOFTOP]": StartBGTransition(rooftopBG); return false;
            case "[GOTO_MARC_TEST]":
                SceneManager.LoadScene("Marc_TestScene");
                return true;
            default: return false;
        }
    }

    void StartBGTransition(GameObject newBG)
    {
        if (newBG == currentBG) return;
        if (bgFadeCoroutine != null) StopCoroutine(bgFadeCoroutine);
        bgFadeCoroutine = StartCoroutine(FadeBackground(newBG));
    }

    IEnumerator FadeBackground(GameObject newBG)
    {
        float duration = 0.5f;
        if (bgFadeSFX != null && sfxSource != null) sfxSource.PlayOneShot(bgFadeSFX);

        if (currentBG != null)
        {
            CanvasGroup oldCG = currentBG.GetComponent<CanvasGroup>();
            if (oldCG != null)
            {
                float t = 0;
                while (t < duration)
                {
                    t += Time.deltaTime;
                    oldCG.alpha = 1 - (t / duration);
                    yield return null;
                }
                oldCG.alpha = 0;
            }
            currentBG.SetActive(false);
        }

        newBG.SetActive(true);
        CanvasGroup newCG = newBG.GetComponent<CanvasGroup>();
        if (newCG != null)
        {
            newCG.alpha = 0;
            float t2 = 0;
            while (t2 < duration)
            {
                t2 += Time.deltaTime;
                newCG.alpha = t2 / duration;
                yield return null;
            }
            newCG.alpha = 1;
        }

        currentBG = newBG;
    }

    void StartFade(float targetAlpha)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    IEnumerator FadeRoutine(float targetAlpha)
    {
        float duration = 0.8f;
        float start = fadeCanvasGroup.alpha;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(start, targetAlpha, elapsed / duration);
            yield return null;
        }
        fadeCanvasGroup.alpha = targetAlpha;
        ShowNextLine();
    }

    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        if (nextButton != null) nextButton.SetActive(false);
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(skipMode ? 0.001f : textSpeed);
        }
        isTyping = false;
        if (nextButton != null) nextButton.SetActive(true);
    }

    void DisableAllBGs()
    {
        if (highSchoolCanteenBG) highSchoolCanteenBG.SetActive(false);
        if (highSchoolClassroomBG) highSchoolClassroomBG.SetActive(false);
        if (blackBG) blackBG.SetActive(false);
        if (SchoolBG) SchoolBG.SetActive(false);
        if (rooftopBG) rooftopBG.SetActive(false);
        currentBG = null;
    }

    void HideAllPortraits()
    {
        GameObject[] ports = { marcNeutral, marcLaugh, marcChide, highschoolCristel, collegeMarcNeutral, collegeMarcLaugh, collegeMarcChide };
        foreach (var p in ports) if (p) p.SetActive(false);
    }
}