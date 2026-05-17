using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public CanvasGroup fadeCanvasGroup;

    [Header("Backgrounds")]
    public GameObject currentBG;
    public GameObject highSchoolCanteenBG;
    public GameObject highSchoolClassroomBG;
    public GameObject blackBG;
    public GameObject SchoolBG;

    [Header("Portraits")]
    public GameObject marcNeutral;
    public GameObject marcLaugh;
    public GameObject marcChide;
    public GameObject highschoolCristel; // Using SHS Cristel portrait

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioClip bellSFX;
    public AudioClip glitchSFX;
    public AudioClip bgFadeSFX;
    public AudioClip tearSniffleSFX;

    void Start()
    {
        // Start scene at the high school canteen flashback
        if (highSchoolCanteenBG != null)
        {
            currentBG = highSchoolCanteenBG;
            highSchoolCanteenBG.SetActive(true);
        }

        if (textBox != null) textBox.SetActive(false);
        if (choicePanelApproach != null) choicePanelApproach.SetActive(false);
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 0;

        EnqueueScene06();
        ShowNextLine();
    }

    void Update()
    {
        skipMode = Input.GetKey(KeyCode.LeftControl);
        if (skipMode && !isTyping && !choicePanelApproach.activeSelf) ShowNextLine();
    }

    void EnqueueScene06()
    {
        // --- ACT I: THE TOMBOY MORENA GIRL ---
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_SCHOOL]"));
        dialogueQueue.Enqueue(new DialogueLine("", "..."));
        dialogueQueue.Enqueue(new DialogueLine("", "I was just minding my own business back then..."));
        dialogueQueue.Enqueue(new DialogueLine("", "But his eyes always drifted towards a specific short-haired, tomboy morena girl who walked around with a certain maangas vibe."));

        dialogueQueue.Enqueue(new DialogueLine("", "Napapansin ko, she walked behind them a lot. Parang laging buntot sa sarili niyang grupo..." ));
        dialogueQueue.Enqueue(new DialogueLine("", "He occasionally saw her sitting alone or quietly watching people in the school canteen."));
        dialogueQueue.Enqueue(new DialogueLine("", "Maybe I had a small crush on her. Who wouldn't? Kaso... I already knew my other friends liked her too. Kaya nanahimik na lang ako." ));

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
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Ahaha, oo... okay lang ako.", highschoolCristel));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[GOTO_MARC_TEST]"));
    }

    public void OnApproachSelected(int index)
    {
        choicePanelApproach.SetActive(false);
        usingTempQueue = true;
        tempQueue.Clear();

        if (index == 0) // Choice 9A: Playful / Gay Persona Approach
        {
            AddTrust("Cristel", 15);
            
            tempQueue.Enqueue(new DialogueLine("", "He pretended to sound gay, pitching his voice up slightly. He figured it was the best way to make her feel comfortable."));
        }
        else // Choice 9B: Gentle / Direct Approach
        {
            AddTrust("Cristel", 10);
            
            tempQueue.Enqueue(new DialogueLine("", "He spoke softly, hoping his sudden presence wouldn't startle her away."));
        }

        ShowNextLine();
    }

    public void OnNextClick()
    {
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
                choicePanelApproach.SetActive(true);
                nextButton.SetActive(false);
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
            case "[FADE_OUT]":
                StartFade(1.0f);
                return true;
            case "[FADE_IN]":
                StartFade(0.0f);
                return true;
            case "[BG_CANTEEN]": StartBGTransition(highSchoolCanteenBG); return false;
            case "[BG_CLASSROOM]": StartBGTransition(highSchoolClassroomBG); return false;
            case "[BG_BLACK]": StartBGTransition(blackBG); return false;
            case "[BG_SCHOOL]": StartBGTransition(SchoolBG); return false;
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
        nextButton.SetActive(false);
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(skipMode ? 0.001f : textSpeed);
        }
        isTyping = false;
        nextButton.SetActive(true);
    }

    void DisableAllBGs()
    {
        if (highSchoolCanteenBG) highSchoolCanteenBG.SetActive(false);
        if (highSchoolClassroomBG) highSchoolClassroomBG.SetActive(false);
        if (blackBG) blackBG.SetActive(false);
        currentBG = null;
    }

    void HideAllPortraits()
    {
        GameObject[] ports = { marcNeutral, marcLaugh, marcChide, highschoolCristel };
        foreach (var p in ports) if (p) p.SetActive(false);
    }
}