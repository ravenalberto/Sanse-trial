using System.Collections;
using System.Collections.Generic;
using TMP_Text = TMPro.TMP_Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene001 : MonoBehaviour
{
    private Dictionary<string, int> trustScores = new Dictionary<string, int>();

    public void AddTrust(string characterName, int amount)
    {
        if (!trustScores.ContainsKey(characterName)) trustScores[characterName] = 0;
        trustScores[characterName] += amount;
        Debug.Log($"<color=cyan>Trust Update:</color> {characterName} is now at {trustScores[characterName]}");
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
    public GameObject choicePanelComfort; // Choice for comforting Cristel
    public GameObject choicePanelMarc;    // Choice for answering Marc
    public GameObject choicePanelMarcReality; // Grouped here under UI Components for easy Inspector visibility!
    public CanvasGroup fadeCanvasGroup;

    [Header("Backgrounds")]
    public GameObject currentBG;
    public GameObject complabBG;
    public GameObject classroomBG;
    public GameObject hallwayBG;
    public GameObject blackBG;
    public GameObject doorBG;
    public GameObject openDoorBG;

    [Header("Portraits")]
    public GameObject cristelNeutral; public GameObject cristelFrown; public GameObject cristelSmile;
    public GameObject marcLaugh; public GameObject marcNeutral; public GameObject marcChide;
    public GameObject kuhNeutral;
    public GameObject ravenNeutral;
    public GameObject darleneNeutral; public GameObject darleneSad;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioClip intercomCrackle;
    public AudioClip glitchSFX;
    public AudioClip bgFadeSFX;
    public AudioClip bellSFX;
    
    
    [Header("Special Effects")]
    public GameObject glassBreakOverlay; // Drag your Glass Shatter Image/UI Overlay Panel here!
    public GameObject glassBreakSoundObject;     // Drag your Glass Shattering Audio Clip here!
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
        // --- CURSOR RESCUE SYSTEM ---
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (complabBG != null)
        {
            currentBG = complabBG;
            complabBG.SetActive(true);
        }

        // Defensive initial state setup
        if (textBox != null) textBox.SetActive(false);
        if (choicePanelComfort != null) choicePanelComfort.SetActive(false);
        if (choicePanelMarc != null) choicePanelMarc.SetActive(false);
        if (choicePanelMarcReality != null) choicePanelMarcReality.SetActive(false);
        if (glassBreakOverlay != null) glassBreakOverlay.SetActive(false);
        if (glassBreakSoundObject != null) glassBreakSoundObject.SetActive(false);
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 0;

        // Cache Marc's layout states so we can warp him seamlessly back and forth
        CacheMarcLayouts();

        EnqueueScene03();
        ShowNextLine();
    }

    private void CacheMarcLayouts()
    {
        GameObject[] marcPortraits = { marcNeutral, marcLaugh, marcChide };
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
        GameObject[] marcPortraits = { marcNeutral, marcLaugh, marcChide };
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

    private void SetCristelToCenter(bool isCenter)
    {
        GameObject[] cristelPortraits = { cristelFrown, cristelNeutral, cristelSmile };
        foreach (var portrait in cristelPortraits)
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

    public void OnChoice7Selected(int index)
    {
        if (choicePanelMarcReality != null) choicePanelMarcReality.SetActive(false);

        usingTempQueue = true;
        tempQueue.Clear();

        if (index == 0)
        {
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_GLASS]"));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[CRISTEL_CENTER]"));
            tempQueue.Enqueue(new DialogueLine(
                "Cristel",
                "Yes, none of this is real. You’re inside a game.",
                cristelNeutral
            ));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[CRISTEL_NORMAL]"));

            tempQueue.Enqueue(new DialogueLine(
                "Darlene",
                "Wha– who are you? Kuya Marc? What is happening?"
                
            ));

            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_GLASS]"));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[MARC_CENTER]"));
            tempQueue.Enqueue(new DialogueLine(
                "Marc",
                "Looks like they want you to realize it all along..",
                marcNeutral
            
            ));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));

        }
        else if (index == 1)
        {
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_GLASS]"));
            tempQueue.Enqueue(new DialogueLine(

                "Darlene",
                "Do what? Who are you?! What is happening?"
            ));

            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[MARC_CENTER]"));
            tempQueue.Enqueue(new DialogueLine(
                "Marc",
                "It’s okay, first impressions are always awkward.",
                marcLaugh
            ));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));

        }
        else
        {
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_GLASS]"));
            tempQueue.Enqueue(new DialogueLine(
                "Darlene",
                "Kuya? Who are you talking to?",
                darleneNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Marc",
                "I see, so you don’t want to show yourself yet huh.",
                marcNeutral
            ));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        }

        // Return Marc's portrait layout to standard sides before scene change transitions

        tempQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE7_END]"));
        tempQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));

        ShowNextLine();
    }

    void Update()
    {
        skipMode = Input.GetKey(KeyCode.LeftControl);

        // Safely check if any of our choice panels are currently blocking inputs
        bool isAnyChoiceActive = (choicePanelComfort != null && choicePanelComfort.activeSelf) ||
                                 (choicePanelMarc != null && choicePanelMarc.activeSelf) ||
                                 (choicePanelMarcReality != null && choicePanelMarcReality.activeSelf);

        if (skipMode && !isTyping && !isAnyChoiceActive)
        {
            ShowNextLine();
        }
    }

    void EnqueueScene03()
    {
        // --- ACT I: ESCAPE BACK TO COMLAB ---
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Finally! Nakalaya din tayo!", darleneNeutral));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_DOOR]"));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Tignan nyo, pinto ba ng comlab natin yan?", cristelSmile));
        dialogueQueue.Enqueue(new DialogueLine("", "Raven checks her phone."));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Oo nga! Wait! Hindi na 5:17! Pero 3:45pm na, late na tayo sa class ni sir.", ravenNeutral));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_OPENDOOR]"));
        dialogueQueue.Enqueue(new DialogueLine("", "A classmate opens the door."));
        dialogueQueue.Enqueue(new DialogueLine("Classmate", "Oy! Late nanaman kayo!"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_CLASSROOM]"));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Uy pre! Namiss kita grabe!", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Wow.. everyone’s here…", darleneNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Darlene’s eyes gaze on Kuh who was greeted by some.."));
        dialogueQueue.Enqueue(new DialogueLine("", "Cristel being asked where they went..."));
        dialogueQueue.Enqueue(new DialogueLine("", "and Raven ducking towards her desk."));
        dialogueQueue.Enqueue(new DialogueLine("", "Marc, however, his back towards everyone, he’s looking out the window."));
        dialogueQueue.Enqueue(new DialogueLine("", "Darlene didn't pay him much mind."));
        dialogueQueue.Enqueue(new DialogueLine("", "Maybe he just needs space. Like Cristel. Everything is fine now. Somehow they escaped."));
        dialogueQueue.Enqueue(new DialogueLine("", "Like Cristel..."));
        dialogueQueue.Enqueue(new DialogueLine("", "Everything is fine now. Somehow they escaped."));
        dialogueQueue.Enqueue(new DialogueLine("", "But something still doesn't sit right. Darlene shakes her head. She sits near her desk. Everyone looked fine. But she worries about Marc."));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "I wonder if he really is okay…", darleneSad));

        // --- ACT II: ATTEMPT TO REPAIR CRISTEL'S HEART ---
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_HALLWAY]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));

        dialogueQueue.Enqueue(new DialogueLine("", "The afternoon progresses. Darlene corners Cristel in the quiet hallway near the lockers to finally clear the heavy clouds between them."));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Salamat nga pala kanina, Dar. At sa lahat. Sa paghahanap sa akin...", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Malamang hahanapin ka namin. Pamilya na tayo rito.", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Actually... hindi ko na talaga alam minsan kung paano magpapatuloy.", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "May mga araw na nararamdaman kong pabigat lang ako... na mas okay siguro pag wala na lang ako.", cristelFrown));

        // Choice 7: Comforting Cristel
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE_COMFORT]"));

        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Siguro..", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Cristel, naniniwala akong hanggang dulo magkakasama pa rin tayong lahat. Wag kayong mawalan ng hope.", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Basta promise me, you’ll stick with us til the end. Okay?", darleneNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Darlene extends her pinky finger. For a moment, Cristel stares at it before slowly raising her own."));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Promise, Dar.", cristelSmile));

        // --- ACT III: CLASSROOM ILLUSION / MARC'S PROVOCATION ---
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_CLASSROOM]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));

        dialogueQueue.Enqueue(new DialogueLine("", "They return to the classroom. The sunset leaks through the dust-mote windows, painting everything in an elegant, almost unreal, golden aura."));
        dialogueQueue.Enqueue(new DialogueLine("", "Marc slowly drags a chair over and sits right next to Darlene's desk."));

        dialogueQueue.Enqueue(new DialogueLine("Marc", "Dar.", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Kuya?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Di ka naman siguro nauto na eto talaga yung nangyari, noh dar?", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "What are you talking about, Kuya?", darleneSad));

        // Shift focus strictly to Player / Fourth Wall Break!
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_GLASS]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[PLAY_GLASS_BREAK]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[MARC_CENTER]"));
        dialogueQueue.Enqueue(new DialogueLine("", "Marc turns towards you.", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "You should be asking **them** not me.", marcNeutral));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_GLASS]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE_7]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_GLASS]"));
    }

    public void OnComfortSelected(int index)
    {
        if (choicePanelComfort != null) choicePanelComfort.SetActive(false);
        usingTempQueue = true;
        tempQueue.Clear();

        if (index == 0) // Choice 7A: Comfort Warmly
        {
            AddTrust("Cristel", 15);
            tempQueue.Enqueue(new DialogueLine("Cristel", "Thank you, dar...", cristelSmile));
        }
        else // Choice 7B: Remind Value
        {
            AddTrust("Cristel", 10);
            tempQueue.Enqueue(new DialogueLine("Cristel", "Naappreciate ko yun, dar...", cristelSmile));
        }
        ShowNextLine();
    }


    private GameObject darleneGlareOrSad()
    {
        return darleneSad != null ? darleneSad : darleneNeutral;
    }

    public void OnNextClick()
    {
        bool isAnyChoiceActive = (choicePanelComfort != null && choicePanelComfort.activeSelf) ||
                                 (choicePanelMarc != null && choicePanelMarc.activeSelf) ||
                                 (choicePanelMarcReality != null && choicePanelMarcReality.activeSelf);

        if (isAnyChoiceActive)
        {
            return; // 🚨 BLOCK ALL DIALOGUE INPUT
        }
        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (dialogueQueue.Count == 0 && tempQueue.Count == 0) return;

        bool isAnyChoiceActive = (choicePanelComfort != null && choicePanelComfort.activeSelf) ||
                                 (choicePanelMarc != null && choicePanelMarc.activeSelf) ||
                                 (choicePanelMarcReality != null && choicePanelMarcReality.activeSelf);

        if (isAnyChoiceActive)
        {
            return; // Stop VN while choosing
        }

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
            case "[CHOICE_COMFORT]":
                if (choicePanelComfort != null) choicePanelComfort.SetActive(true);
                if (nextButton != null) nextButton.SetActive(false);
                return true;
            case "[CHOICE_MARC_WARN]":
                if (choicePanelMarc != null) choicePanelMarc.SetActive(true);
                if (nextButton != null) nextButton.SetActive(false);
                return true;
            case "[SFX_INTERCOM]":
                if (sfxSource && intercomCrackle) sfxSource.PlayOneShot(intercomCrackle);
                return false;
            case "[SFX_GLITCH]":
                if (sfxSource && glitchSFX) sfxSource.PlayOneShot(glitchSFX);
                return false;
            case "[FADE_OUT]":
                StartFade(1.0f);
                return true;
            case "[FADE_IN]":
                StartFade(0.0f);
                return true;
            case "[MARC_CENTER]":
                SetMarcToCenter(true);
                return false;
            case "[MARC_NORMAL]":
                SetMarcToCenter(false);
                return false;
            case "[CRISTEL_CENTER]":
                SetCristelToCenter(true);
                return false;
            case "[CRISTEL_NORMAL]":
                SetMarcToCenter(false);
                return false;
            case "[CHOICE_7]":
                
                if (choicePanelMarcReality != null) choicePanelMarcReality.SetActive(true);
                if (nextButton != null) nextButton.SetActive(false);
                return true;
            case "[CHOICE7_END]":
                SceneManager.LoadScene("Scene06");
                return true;
            case "[PLAY_GLASS_BREAK]":
                
                if (glassBreakSoundObject != null) glassBreakSoundObject.SetActive(true);
                return false;
            case "[BG_GLASS]": StartBGTransition(glassBreakOverlay); return false;
            case "[BG_COMPLAB]": StartBGTransition(complabBG); return false;
            case "[BG_CLASSROOM]": StartBGTransition(classroomBG); return false;
            case "[BG_HALLWAY]": StartBGTransition(hallwayBG); return false;
            case "[BG_BLACK]": StartBGTransition(blackBG); return false;
            case "[BG_DOOR]": StartBGTransition(doorBG); return false;
            case "[BG_OPENDOOR]": StartBGTransition(openDoorBG); return false;
            case "[GOTO_SCENE06]":
                SceneManager.LoadScene("Scene06");
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
        if (complabBG) complabBG.SetActive(false);
        if (classroomBG) classroomBG.SetActive(false);
        if (hallwayBG) hallwayBG.SetActive(false);
        if (blackBG) blackBG.SetActive(false);
        currentBG = null;
    }

    void HideAllPortraits()
    {
        GameObject[] ports = { cristelNeutral, cristelFrown, cristelSmile, marcLaugh, marcNeutral, marcChide, kuhNeutral, ravenNeutral, darleneNeutral, darleneSad };
        foreach (var p in ports) if (p) p.SetActive(false);
    }
}