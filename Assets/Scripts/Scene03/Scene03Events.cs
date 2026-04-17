using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Scene03Events : MonoBehaviour
{
    [Header("Characters")]
    public GameObject charKuh;
    public GameObject charCristel;
    public GameObject charDarlene;
    public GameObject charMarc;
    public GameObject charRaven;

    [Header("UI References")]
    public GameObject textBox;
    public GameObject dialogueUI;
    public GameObject blockPuzzleUI;
    public GameObject vnUI;
    public GameObject choiceUI;
    public GameObject mainTextObject;
    public GameObject nextButton;
    public GameObject charName;
    public GameObject fadeOut;
    public GameObject fadeScreenIn;
    public GameObject flashOverlay;

    [Header("Backgrounds")]
    public GameObject bgRooftop;
    public GameObject bgClassroomFlashback;

    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioClip flashbackBGM;
    public AudioClip rooftopWindBGM;
    public AudioClip intercomCrackleSFX;
    public AudioClip clockTickSFX;
    public AudioClip flashSFX;
    public AudioClip phoneSlideSFX;

    [Header("Puzzle Reference")]
    public BlockPuzzleManager puzzleManager;

    [Header("Logic States")]
    int memoryCount = 0;
    public int marcTrust = 0;
    public int marcStayPoints = 0;

    [Header("Settings")]
    [SerializeField] string textToSpeak;
    [SerializeField] int currentTextLength;
    [SerializeField] int textLength;
    [SerializeField] int eventPos = 0;
    private bool isTyping = false;

    [Header("Sprites")]
    [SerializeField] Sprite cristelNeutral;
    [SerializeField] Sprite cristelFrown;
    [SerializeField] Sprite kuhNeutral;
    [SerializeField] Sprite marcNeutral;
    [SerializeField] Sprite marcSmile;
    [SerializeField] Sprite marcAngry;
    [SerializeField] Sprite ravenNeutral;
    [SerializeField] Sprite darleneNeutral;
    [SerializeField] Sprite darleneShock;

    void Start()
    {
        // Initial setup to ensure no overlaps
        if (bgRooftop != null) bgRooftop.SetActive(false);
        if (bgClassroomFlashback != null) bgClassroomFlashback.SetActive(false);
        if (flashOverlay != null) flashOverlay.SetActive(false);
        if (fadeOut != null) fadeOut.SetActive(false);

        if (PuzzleState.scene3Result != "")
        {
            StartCoroutine(HandlePuzzleResult());
        }
        else
        {
            StartCoroutine(FlashbackStarter());
        }
    }

    void Update()
    {
        textLength = TextCreator.charCount;
        if (textLength >= currentTextLength && isTyping)
        {
            isTyping = false;
            nextButton.SetActive(true);
        }
    }

    IEnumerator FlashbackStarter()
    {
        fadeScreenIn.SetActive(true);
        StartCoroutine(FadeIn());

        // Set Flashback Background and hide Rooftop
        if (bgRooftop != null) bgRooftop.SetActive(false);
        if (bgClassroomFlashback != null) bgClassroomFlashback.SetActive(true);

        if (bgmSource != null && flashbackBGM != null)
        {
            bgmSource.clip = flashbackBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        yield return new WaitForSeconds(1.5f);

        StartDialogue("", "[Two Months Before the Incident]");
        eventPos = -10; // New starting point for longer flashback
        yield return null;
    }

    // --- EXTENDED FLASHBACK STEPS ---

    IEnumerator FlashbackStep2()
    {
        nextButton.SetActive(false);
        HideAllCharacters();
        charRaven.SetActive(true); // Raven remains constant
        charMarc.SetActive(true);
        SetExpression(charMarc, marcSmile);
        StartDialogue("Marc", "I’m telling you, if we use this layout for the presentation, we’re getting an easy flat 1.0. High risk, high reward.");
        eventPos = -9;
        yield return null;
    }

    IEnumerator FlashbackStep3()
    {
        nextButton.SetActive(false);
        HideAllCharacters();
        charRaven.SetActive(true);
        StartDialogue("Raven", "It's statistically inefficient, Marc. The professor prefers structured data over 'aesthetic flair'. Let Darlene handle the visuals.");
        eventPos = -8;
        yield return null;
    }

    IEnumerator FlashbackStep4()
    {
        nextButton.SetActive(false);
        HideAllCharacters();
        charRaven.SetActive(true);
        charCristel.SetActive(true);
        SetExpression(charCristel, cristelNeutral);
        StartDialogue("Cristel", "Raven's right, Marc. Last time you handled the layout, we almost forgot the bibliography.");
        eventPos = -7;
        yield return null;
    }

    IEnumerator FlashbackStep5()
    {
        nextButton.SetActive(false);
        HideAllCharacters();
        charRaven.SetActive(true);
        charDarlene.SetActive(true);
        SetExpression(charDarlene, darleneNeutral);
        StartDialogue("Darlene", "It’s okay, guys! I can merge both ideas. I'll use Marc's colors but keep Raven's formatting. Tetel, do you have the intro ready?");
        eventPos = -6;
        yield return null;
    }

    IEnumerator FlashbackStep6()
    {
        nextButton.SetActive(false);
        HideAllCharacters();
        charRaven.SetActive(true);
        charKuh.SetActive(true);
        SetExpression(charKuh, kuhNeutral);
        StartDialogue("Kuh", "Hala, project na naman? Kahit ano basta may food break after nito! Sobrang gutom na ako!");
        eventPos = -5;
        yield return null;
    }

    IEnumerator FlashbackStep7()
    {
        nextButton.SetActive(false);
        HideAllCharacters();
        charRaven.SetActive(true);
        charMarc.SetActive(true);
        SetExpression(charMarc, marcSmile);
        StartDialogue("Marc", "Kuh, kakain lang tayo 30 minutes ago. Paano naging break 'yun? Speedrun eating ba goal mo?");
        eventPos = -4;
        yield return null;
    }

    IEnumerator FlashbackStep8()
    {
        nextButton.SetActive(false);
        HideAllCharacters();
        charRaven.SetActive(true);
        StartDialogue("Raven", "If we maintain this pace of conversation, we will exceed the deadline by exactly 14 hours. Focus.");
        eventPos = -3;
        yield return null;
    }

    IEnumerator FlashbackStep9()
    {
        nextButton.SetActive(false);
        HideAllCharacters();
        charRaven.SetActive(true);
        charCristel.SetActive(true);
        StartDialogue("Cristel", "Okay, okay. Chill lang, Raven. Malapit na tayong matapos. We can submit this before the afternoon prayer later.");
        eventPos = -2;
        yield return null;
    }

    IEnumerator FlashbackStep10()
    {
        nextButton.SetActive(false);
        HideAllCharacters();
        charRaven.SetActive(true);
        charDarlene.SetActive(true);
        StartDialogue("Darlene", "Yeah, then we can finally have that food trip Kuh keeps mentioning. Promise!");
        eventPos = -1;
        yield return null;
    }

    IEnumerator EndFlashback()
    {
        nextButton.SetActive(false);

        if (sfxSource != null && flashSFX != null) sfxSource.PlayOneShot(flashSFX);
        if (flashOverlay != null)
        {
            flashOverlay.SetActive(true);
            yield return new WaitForSeconds(0.5f);
        }

        HideAllCharacters();
        // Switch backgrounds cleanly
        if (bgClassroomFlashback != null) bgClassroomFlashback.SetActive(false);
        if (bgRooftop != null) bgRooftop.SetActive(true);

        if (bgmSource != null) bgmSource.Stop();
        if (flashOverlay != null) flashOverlay.SetActive(false);

        StartCoroutine(EventStarter());
    }

    IEnumerator EventStarter()
    {
        yield return new WaitForSeconds(0.5f);
        StartDialogue("", "CHAPTER 2\n\nPatterns That Shouldn’t Exist");
        yield return WaitForText();
        yield return new WaitForSeconds(1f);

        // Fade through black before the rooftop tension starts
        fadeOut.SetActive(true);
        CanvasGroup fadeCanvas = fadeOut.GetComponent<CanvasGroup>();
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            if (fadeCanvas != null) fadeCanvas.alpha = t;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        fadeOut.SetActive(false);

        if (bgmSource != null && rooftopWindBGM != null)
        {
            bgmSource.clip = rooftopWindBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        StartCoroutine(EventOne());
    }

    // --- REALITY STEPS ---

    IEnumerator EventOne()
    {
        nextButton.SetActive(false);
        HideAllCharacters();
        charRaven.SetActive(true);
        SetExpression(charRaven, ravenNeutral);
        StartDialogue("", "We shouldn’t be going.");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 2;
    }

    IEnumerator EventTwo()
    {
        nextButton.SetActive(false);
        StartDialogue("", "That’s the first thing I notice.\n\nNot fear.\n\nJust… wrong direction.");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 3;
    }

    IEnumerator EventThree()
    {
        nextButton.SetActive(false);
        charCristel.SetActive(true);
        SetExpression(charCristel, cristelNeutral);
        StartDialogue("Cristel", "Come on… it’s probably just another clue.");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 4;
    }

    IEnumerator EventFour()
    {
        nextButton.SetActive(false);
        charRaven.SetActive(true);
        SetExpression(charRaven, ravenNeutral);
        StartDialogue("Raven", "Since when do we follow instructions from a broken intercom?");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 5;
    }

    IEnumerator EventFive()
    {
        nextButton.SetActive(false);
        charCristel.SetActive(false);
        charMarc.SetActive(true);
        SetExpression(charMarc, marcSmile);
        StartDialogue("Marc", "We followed worse.");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 6;
    }

    IEnumerator EventSix()
    {
        nextButton.SetActive(false);
        charRaven.SetActive(true);
        SetExpression(charRaven, ravenNeutral);
        StartDialogue("Raven", "Yeah, like your ideas.");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 7;
    }

    IEnumerator EventSeven()
    {
        nextButton.SetActive(false);
        StartDialogue("Marc", "Wow. Betrayal.");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 8;
    }

    IEnumerator EventEight()
    {
        nextButton.SetActive(false);
        charMarc.SetActive(false);
        charDarlene.SetActive(true);
        SetExpression(charDarlene, darleneNeutral);
        StartDialogue("Darlene", "Can we not joke right now?");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 9;
    }

    IEnumerator EventNine()
    {
        nextButton.SetActive(false);
        StartDialogue("Raven", "…Why the rooftop?");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 10;
    }

    IEnumerator EventTen()
    {
        nextButton.SetActive(false);
        charDarlene.SetActive(false);
        charMarc.SetActive(true);
        SetExpression(charMarc, marcAngry);
        StartDialogue("Marc", "…Why not?");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 11;
    }

    IEnumerator EventEleven()
    {
        nextButton.SetActive(false);
        charMarc.SetActive(false);
        charDarlene.SetActive(true);
        SetExpression(charDarlene, darleneNeutral);
        StartDialogue("Darlene", "Don’t.");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 12;
    }

    IEnumerator EventTwelve()
    {
        nextButton.SetActive(false);
        charDarlene.SetActive(false);
        charCristel.SetActive(true);
        SetExpression(charCristel, cristelNeutral);
        StartDialogue("Cristel", "It’s just another puzzle. Like earlier.");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 13;
    }

    IEnumerator EventThirteen()
    {
        nextButton.SetActive(false);
        charRaven.SetActive(true);
        StartDialogue("Raven", "…You keep saying that.");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 14;
    }

    IEnumerator EventFourteen()
    {
        nextButton.SetActive(false);
        StartDialogue("Cristel", "Because it is.");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 15;
    }

    IEnumerator EventFifteen()
    {
        nextButton.SetActive(false);
        charRaven.SetActive(true);
        StartDialogue("Raven", "Or because you don’t want it to be something else?");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 16;
    }

    IEnumerator EventSixteen()
    {
        nextButton.SetActive(false);
        HideAllCharacters();
        StartDialogue("", "[Silence lingers.]");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 17;
    }

    IEnumerator EventSeventeen()
    {
        nextButton.SetActive(false);
        StartDialogue("", "Too many variables.\n\nToo many assumptions.\n\nToo many… versions.");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 18;
    }

    IEnumerator EventEightteen()
    {
        nextButton.SetActive(false);
        if (sfxSource != null && phoneSlideSFX != null) sfxSource.PlayOneShot(phoneSlideSFX);
        StartDialogue("", "I pull out my phone");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 19;
    }

    IEnumerator EventNineteen()
    {
        nextButton.SetActive(false);
        charCristel.SetActive(true);
        SetExpression(charCristel, cristelNeutral);
        StartDialogue("Cristel", "…Really?");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 20;
    }

    IEnumerator EventTwenty()
    {
        nextButton.SetActive(false);
        charRaven.SetActive(true);
        SetExpression(charRaven, ravenNeutral);
        StartDialogue("Raven", "I think better when things are structured.");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 21;
    }

    IEnumerator EventTwentyOne()
    {
        nextButton.SetActive(false);
        fadeOut.SetActive(true);
        CanvasGroup fadeCanvas = fadeOut.GetComponent<CanvasGroup>();
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            if (fadeCanvas != null) fadeCanvas.alpha = t;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        PlayerPrefs.SetInt("Scene03_Event", 22);
        SceneManager.LoadScene("BlockPuzzleScene");
    }

    IEnumerator EventTwentyTwo()
    {
        nextButton.SetActive(false);
        charMarc.SetActive(true);
        SetExpression(charMarc, marcAngry);
        StartDialogue("Marc", "…You done?");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 23;
    }

    IEnumerator EventTwentyThree()
    {
        nextButton.SetActive(false);
        charMarc.SetActive(false);
        charDarlene.SetActive(true);
        SetExpression(charDarlene, darleneNeutral);
        StartDialogue("Darlene", "We’re asking you.");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 24;
    }

    IEnumerator EventTwentyFour()
    {
        nextButton.SetActive(false);
        charDarlene.SetActive(false);
        charCristel.SetActive(true);
        SetExpression(charCristel, cristelNeutral);
        StartDialogue("Cristel", "What do you think?");
        yield return WaitForText();
        nextButton.SetActive(false);
        charCristel.SetActive(false);
        choiceUI.SetActive(true);
    }

    IEnumerator AfterChoice()
    {
        choiceUI.SetActive(false);
        nextButton.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        charMarc.SetActive(true);

        if (marcTrust <= -1 && marcStayPoints <= -1)
        {
            SetExpression(charMarc, marcAngry);
            StartDialogue("Marc", "…Right. Of course it is.");
        }
        else if (marcTrust < 0)
        {
            SetExpression(charMarc, marcNeutral);
            StartDialogue("Marc", "…That’s helpful.");
        }
        else if (marcTrust >= 2)
        {
            SetExpression(charMarc, marcNeutral);
            StartDialogue("Marc", "…Yeah. I guess we all did.");
        }
        else
        {
            SetExpression(charMarc, marcNeutral);
            StartDialogue("Marc", "…It never is.");
        }

        yield return WaitForText();
        yield return new WaitForSeconds(0.3f);
        nextButton.SetActive(true);
        eventPos = 25;
    }

    IEnumerator EventTwentyFive()
    {
        nextButton.SetActive(false);
        charMarc.SetActive(false);
        StartDialogue("", "[Silence hangs for a second too long]");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 26;
    }

    IEnumerator EventTwentySix()
    {
        nextButton.SetActive(false);
        if (sfxSource != null && intercomCrackleSFX != null) sfxSource.PlayOneShot(intercomCrackleSFX);
        StartDialogue("", "[SFX: intercom crackle]");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 27;
    }

    IEnumerator EventTwentySeven()
    {
        nextButton.SetActive(false);
        StartDialogue("INTERCOM", "Angelus Domini nuntiavit Mariae…");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 28;
    }

    IEnumerator EventTwentyEight()
    {
        nextButton.SetActive(false);
        charKuh.SetActive(true);
        SetExpression(charKuh, kuhNeutral);
        StartDialogue("Kuh", "…Again?");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 29;
    }

    IEnumerator EventTwentyNine()
    {
        nextButton.SetActive(false);
        StartDialogue("INTERCOM", "…et concepit de—");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 30;
    }

    IEnumerator EventThirty()
    {
        nextButton.SetActive(false);
        if (sfxSource != null && intercomCrackleSFX != null) sfxSource.PlayOneShot(intercomCrackleSFX);
        StartDialogue("", "[STATIC CUT]");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 31;
    }

    IEnumerator EventThirtyOne()
    {
        nextButton.SetActive(false);
        StartDialogue("INTERCOM", "…Proceed.");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 32;
    }

    IEnumerator EventThirtyTwo()
    {
        nextButton.SetActive(false);
        if (sfxSource != null && clockTickSFX != null)
        {
            sfxSource.clip = clockTickSFX;
            sfxSource.loop = true;
            sfxSource.Play();
        }
        StartDialogue("", "[Clock ticking returns, louder than before]");
        yield return WaitForText();
        nextButton.SetActive(true);
        eventPos = 33;
    }

    IEnumerator Event_TransitionToGameplay()
    {
        nextButton.SetActive(false);
        charKuh.SetActive(true);
        SetExpression(charKuh, kuhNeutral);
        StartDialogue("", "…That sound again.");
        yield return WaitForText();
        StartDialogue("", "Why does it feel closer this time?");
        yield return WaitForText();
        StartDialogue("", "…Like it’s calling.");
        yield return WaitForText();
        StartDialogue("Kuh", "Hello?");
        yield return WaitForText();
        StartDialogue("Kuh", "…Guys?");
        yield return WaitForText();
        StartDialogue("", "…Someone’s up there.");
        yield return WaitForText();
        StartDialogue("", "…They’re not answering.");
        yield return WaitForText();
        StartDialogue("Kuh", "…Wait.");
        yield return WaitForText();
        StartDialogue("", "…They’re behind me.");
        yield return WaitForText();
        StartDialogue("", "…Right?");
        yield return WaitForText();
        StartDialogue("Kuh", "…I’m right here.");
        yield return WaitForText();
        StartDialogue("", "…They’ll catch up.");
        yield return WaitForText();
        StartDialogue("", "…It’s fine.");
        yield return WaitForText();
        StartDialogue("Kuh", "…Why does it feel like I’ve been here before?");
        yield return WaitForText();

        fadeOut.SetActive(true);
        CanvasGroup fadeCanvas = fadeOut.GetComponent<CanvasGroup>();
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            if (fadeCanvas != null) fadeCanvas.alpha = t;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("PacmanChaseStair2");
    }

    // --- Utilities ---

    IEnumerator FadeIn()
    {
        CanvasGroup fadeCanvas = fadeScreenIn.GetComponent<CanvasGroup>();
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime;
            if (fadeCanvas != null) fadeCanvas.alpha = t;
            yield return null;
        }
        fadeScreenIn.SetActive(false);
    }

    IEnumerator HandlePuzzleResult()
    {
        yield return new WaitForSeconds(1f);
        string result = PuzzleState.scene3Result;
        PuzzleState.scene3Result = "";
        vnUI.SetActive(true);
        nextButton.SetActive(true);
        eventPos = 22;
    }

    IEnumerator WaitForText()
    {
        float timer = 0f;
        while (textLength < currentTextLength)
        {
            timer += Time.deltaTime;
            if (timer > 5f) break;
            yield return null;
        }
        yield return new WaitForSeconds(0.05f);
    }

    void SetExpression(GameObject character, Sprite expression)
    {
        if (character == null || expression == null) return;
        var img = character.GetComponent<UnityEngine.UI.Image>();
        if (img != null) img.sprite = expression;
    }

    void StartDialogue(string speaker, string line)
    {
        mainTextObject.SetActive(true);
        charName.GetComponent<TMPro.TMP_Text>().text = speaker;
        TextCreator.runTextPrint = false;
        textToSpeak = line;
        TextCreator.fullText = textToSpeak;
        TextCreator.charCount = 0;
        currentTextLength = textToSpeak.Length;
        textLength = 0;
        TextCreator.runTextPrint = true;
        isTyping = true;
    }

    void HideAllCharacters()
    {
        charKuh.SetActive(false);
        charCristel.SetActive(false);
        charDarlene.SetActive(false);
        charMarc.SetActive(false);
        charRaven.SetActive(false);
    }

    public void ChoiceA() { marcTrust -= 1; marcStayPoints -= 1; StartCoroutine(AfterChoice()); }
    public void ChoiceB() { marcTrust -= 1; StartCoroutine(AfterChoice()); }
    public void ChoiceC() { marcTrust += 1; marcStayPoints += 1; StartCoroutine(AfterChoice()); }
    public void ChoiceD() { marcTrust += 2; StartCoroutine(AfterChoice()); }

    public void NextButton()
    {
        if (isTyping) return;

        // Expanded Flashback
        if (eventPos == -10) StartCoroutine(FlashbackStep2());
        else if (eventPos == -9) StartCoroutine(FlashbackStep3());
        else if (eventPos == -8) StartCoroutine(FlashbackStep4());
        else if (eventPos == -7) StartCoroutine(FlashbackStep5());
        else if (eventPos == -6) StartCoroutine(FlashbackStep6());
        else if (eventPos == -5) StartCoroutine(FlashbackStep7());
        else if (eventPos == -4) StartCoroutine(FlashbackStep8());
        else if (eventPos == -3) StartCoroutine(FlashbackStep9());
        else if (eventPos == -2) StartCoroutine(FlashbackStep10());
        else if (eventPos == -1) StartCoroutine(EndFlashback());

        // Main Reality
        else if (eventPos == 1) StartCoroutine(EventOne());
        else if (eventPos == 2) StartCoroutine(EventTwo());
        else if (eventPos == 3) StartCoroutine(EventThree());
        else if (eventPos == 4) StartCoroutine(EventFour());
        else if (eventPos == 5) StartCoroutine(EventFive());
        else if (eventPos == 6) StartCoroutine(EventSix());
        else if (eventPos == 7) StartCoroutine(EventSeven());
        else if (eventPos == 8) StartCoroutine(EventEight());
        else if (eventPos == 9) StartCoroutine(EventNine());
        else if (eventPos == 10) StartCoroutine(EventTen());
        else if (eventPos == 11) StartCoroutine(EventEleven());
        else if (eventPos == 12) StartCoroutine(EventTwelve());
        else if (eventPos == 13) StartCoroutine(EventThirteen());
        else if (eventPos == 14) StartCoroutine(EventFourteen());
        else if (eventPos == 15) StartCoroutine(EventFifteen());
        else if (eventPos == 16) StartCoroutine(EventSixteen());
        else if (eventPos == 17) StartCoroutine(EventSeventeen());
        else if (eventPos == 18) StartCoroutine(EventEightteen());
        else if (eventPos == 19) StartCoroutine(EventNineteen());
        else if (eventPos == 20) StartCoroutine(EventTwenty());
        else if (eventPos == 21) StartCoroutine(EventTwentyOne());
        else if (eventPos == 22) StartCoroutine(EventTwentyTwo());
        else if (eventPos == 23) StartCoroutine(EventTwentyThree());
        else if (eventPos == 24) StartCoroutine(EventTwentyFour());
        else if (eventPos == 25) StartCoroutine(EventTwentyFive());
        else if (eventPos == 26) StartCoroutine(EventTwentySix());
        else if (eventPos == 27) StartCoroutine(EventTwentySeven());
        else if (eventPos == 28) StartCoroutine(EventTwentyEight());
        else if (eventPos == 29) StartCoroutine(EventTwentyNine());
        else if (eventPos == 30) StartCoroutine(EventThirty());
        else if (eventPos == 31) StartCoroutine(EventThirtyOne());
        else if (eventPos == 32) StartCoroutine(EventThirtyTwo());
        else if (eventPos == 33) StartCoroutine(Event_TransitionToGameplay());
    }
}