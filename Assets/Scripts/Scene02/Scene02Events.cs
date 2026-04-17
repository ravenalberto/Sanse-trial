using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Scene02Events : MonoBehaviour
{
    [Header("Characters")]
    public GameObject charKuh;
    public GameObject charCristel;
    public GameObject charDarlene;
    public GameObject charMarc;
    public GameObject charRaven;

    [Header("UI References")]
    public GameObject textBox;
    public GameObject vnUI;
    public GameObject mainTextObject;
    public GameObject nextButton;
    public GameObject charName;
    public GameObject fadeOut;
    public GameObject fadeScreenIn;
    public GameObject glitchOverlay; // For visual tension

    [Header("Audio")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioClip roomTensionBGM;
    public AudioClip intercomClickSFX;
    public AudioClip angelusPrayerDistorted;
    public AudioClip doorUnlockSFX;
    public AudioClip glitchSFX;
    public AudioClip slideSFX;

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
    [SerializeField] Sprite ravenNeutral;
    [SerializeField] Sprite darleneNeutral;
    [SerializeField] Sprite darleneShock;

    void Start()
    {
        fadeScreenIn.SetActive(true);
        if (glitchOverlay != null) glitchOverlay.SetActive(false);

        // Start Room BGM immediately
        if (bgmSource != null && roomTensionBGM != null)
        {
            bgmSource.clip = roomTensionBGM;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        StartCoroutine(FadeIn());

        if (PuzzleState.scene2Result != "")
        {
            StartCoroutine(HandlePuzzleResult());
        }
        else
        {
            StartCoroutine(EventStarter());
        }
    }

    IEnumerator FadeIn()
    {
        CanvasGroup fadeCanvas = fadeScreenIn.GetComponent<CanvasGroup>();
        if (fadeCanvas != null)
        {
            float t = 1;
            while (t > 0)
            {
                t -= Time.deltaTime;
                fadeCanvas.alpha = t;
                yield return null;
            }
        }
        fadeScreenIn.SetActive(false);
    }

    void Update()
    {
        textLength = TextCreator.charCount;

        // Check if text is finished to enable next button
        if (textLength >= currentTextLength && isTyping)
        {
            isTyping = false;
            nextButton.SetActive(true);
        }
    }

    IEnumerator HandlePuzzleResult()
    {
        yield return new WaitForSeconds(1f);
        string result = PuzzleState.scene2Result;
        PuzzleState.scene2Result = "";
        vnUI.SetActive(true);

        if (result == "Approve") eventPos = 20;
        else if (result == "Disapprove") eventPos = 30;
        else eventPos = 40;

        NextButton(); // Auto-trigger dialogue for result
    }

    IEnumerator SlideCharacter(GameObject character, Vector3 start, Vector3 end, float duration)
    {
        RectTransform rect = character.GetComponent<RectTransform>();
        float time = 0;
        rect.anchoredPosition = start;

        if (sfxSource != null && slideSFX != null) sfxSource.PlayOneShot(slideSFX);

        while (time < duration)
        {
            rect.anchoredPosition = Vector3.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = end;
    }

    IEnumerator EventStarter()
    {
        yield return new WaitForSeconds(1);
        charDarlene.SetActive(true);
        SetExpression(charDarlene, darleneShock);
        StartDialogue("Darlene", "Cristel!");
        eventPos = 1;
    }

    IEnumerator EventOne()
    {
        nextButton.SetActive(false);
        charCristel.SetActive(true);
        SetExpression(charCristel, cristelNeutral);
        StartDialogue("Cristel", "Oh thank god—akala ko kami lang ni Kuh…");
        eventPos = 2;
        yield return null;
    }

    IEnumerator EventTwo()
    {
        nextButton.SetActive(false);
        charDarlene.SetActive(false);
        charMarc.SetActive(true);
        SetExpression(charMarc, marcSmile);
        RectTransform marcRect = charMarc.GetComponent<RectTransform>();
        Vector2 endPos = marcRect.anchoredPosition;
        yield return StartCoroutine(SlideCharacter(charMarc, endPos + new Vector2(800, 0), endPos, 0.5f));
        StartDialogue("Marc", "Look who finally decided to respawn.");
        eventPos = 3;
    }

    IEnumerator EventThree()
    {
        nextButton.SetActive(false);
        StartDialogue("Cristel", "Not funny.");
        eventPos = 4;
        yield return null;
    }

    IEnumerator EventFour()
    {
        nextButton.SetActive(false);
        charMarc.SetActive(false);
        charRaven.SetActive(true);
        SetExpression(charRaven, ravenNeutral);
        RectTransform ravenRect = charRaven.GetComponent<RectTransform>();
        Vector2 endPos = ravenRect.anchoredPosition;
        yield return StartCoroutine(SlideCharacter(charRaven, endPos + new Vector2(800, 0), endPos, 0.5f));
        StartDialogue("Raven", "You took longer than usual.");
        eventPos = 5;
    }

    IEnumerator EventFive()
    {
        nextButton.SetActive(false);
        StartDialogue("Cristel", "Usual?");
        eventPos = 6;
        yield return null;
    }

    IEnumerator EventSix()
    {
        nextButton.SetActive(false);
        charRaven.SetActive(false);
        charCristel.SetActive(false);
        StartDialogue("", "The classroom feels… rearranged.\n\nLike someone tried to remember it instead of actually remembering.");
        eventPos = 7;
        yield return null;
    }

    IEnumerator EventSeven()
    {
        nextButton.SetActive(false);
        charCristel.SetActive(true);
        StartDialogue("Cristel", "…Wait. Iniwan ba natin ‘to ng ganito?");
        eventPos = 8;
        yield return null;
    }

    IEnumerator EventEight()
    {
        nextButton.SetActive(false);
        charMarc.SetActive(true);
        SetExpression(charMarc, marcNeutral);
        StartDialogue("Marc", "I don’t remember putting name tags on chairs.");
        eventPos = 9;
        yield return null;
    }

    IEnumerator EventNine()
    {
        nextButton.SetActive(false);
        charMarc.SetActive(false);
        charDarlene.SetActive(true);
        SetExpression(charDarlene, darleneNeutral);
        StartDialogue("Darlene", "Maybe may purpose? Parang clue?");
        eventPos = 10;
        yield return null;
    }

    IEnumerator EventTen()
    {
        nextButton.SetActive(false);
        charDarlene.SetActive(false);
        charRaven.SetActive(true);
        StartDialogue("Raven", "…There’s a message.");
        eventPos = 11;
        yield return null;
    }

    IEnumerator EventEleven()
    {
        nextButton.SetActive(false);
        StartDialogue("Raven", "Raven is always with Darlene.");
        eventPos = 12;
        yield return null;
    }

    IEnumerator EventTwelve()
    {
        nextButton.SetActive(false);
        StartDialogue("Raven", "Marc is always next to Cristel.");
        eventPos = 13;
        yield return null;
    }

    IEnumerator EventThirteen()
    {
        nextButton.SetActive(false);
        StartDialogue("Raven", "Kuh is everywhere.");
        eventPos = 14;
        yield return null;
    }

    IEnumerator EventFourteen()
    {
        nextButton.SetActive(false);
        charRaven.SetActive(false);
        charMarc.SetActive(true);
        StartDialogue("Marc", "Wow. Ang linaw. Zero explanation.");
        eventPos = 15;
        yield return null;
    }

    IEnumerator EventFifteen()
    {
        nextButton.SetActive(false);
        StartDialogue("Cristel", "No… it does.");
        eventPos = 16;
        yield return null;
    }

    IEnumerator EventSixteen()
    {
        nextButton.SetActive(false);
        charCristel.SetActive(false);
        charMarc.SetActive(false);
        StartDialogue("", "This is easy.\n\nI know them.\n\nI know where everyone belongs.\n\nRight?");
        eventPos = 17;
        yield return null;
    }

    IEnumerator EventSeventeen()
    {
        nextButton.SetActive(false);
        CanvasGroup fadeCanvas = fadeOut.GetComponent<CanvasGroup>();
        fadeOut.SetActive(true);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            if (fadeCanvas != null) fadeCanvas.alpha = t;
            yield return null;
        }
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene("ChairPuzzleScene");
    }

    // --- APPROVE BRANCH ---
    IEnumerator Event20()
    {
        charCristel.SetActive(true);
        SetExpression(charCristel, cristelNeutral);
        StartDialogue("Cristel", "Oh…\n\n…right.");
        eventPos = 21;
        yield return null;
    }

    IEnumerator Event21()
    {
        nextButton.SetActive(false);
        StartDialogue("Cristel", "Kuh never sat still.");
        eventPos = 22;
        yield return null;
    }

    IEnumerator Event22()
    {
        charRaven.SetActive(true);
        SetExpression(charRaven, ravenNeutral);
        nextButton.SetActive(false);
        StartDialogue("Raven", "…You remembered.");
        eventPos = 23;
        yield return null;
    }

    IEnumerator Event23()
    {
        charRaven.SetActive(false);
        charCristel.SetActive(false);
        nextButton.SetActive(false);
        StartDialogue("", "For a second,\nthe room feels… real again.");
        eventPos = 50;
        yield return null;
    }

    // --- DISAPPROVE BRANCH ---
    IEnumerator Event30()
    {
        charKuh.SetActive(true);
        SetExpression(charKuh, kuhNeutral);
        nextButton.SetActive(false);
        StartDialogue("Kuh", "…Funny.");
        eventPos = 31;
        yield return null;
    }

    IEnumerator Event31()
    {
        nextButton.SetActive(false);
        StartDialogue("Kuh", "I don’t remember sitting there.");
        eventPos = 32;
        yield return null;
    }

    IEnumerator Event32()
    {
        nextButton.SetActive(false);
        StartDialogue("Kuh", "Or… anywhere, really.");
        eventPos = 33;
        yield return null;
    }

    IEnumerator Event33()
    {
        charKuh.SetActive(false);
        nextButton.SetActive(false);
        StartDialogue("", "The silence stretches.\n\nSomething is off.");
        eventPos = 50;
        yield return null;
    }

    // --- ERROR/GAME OVER BRANCH ---
    IEnumerator Event40()
    {
        nextButton.SetActive(false);
        StartDialogue("", "…No.\n\nThis isn’t right.");
        eventPos = 41;
        yield return null;
    }

    IEnumerator Event41()
    {
        nextButton.SetActive(false);
        StartDialogue("", "None of this is right.");
        eventPos = 42;
        yield return null;
    }

    IEnumerator Event42()
    {
        nextButton.SetActive(false);
        StartDialogue("", "Let’s try again.");
        yield return WaitForText();
        SceneManager.LoadScene("ChairPuzzleScene");
    }

    // --- INTERCOM FINALE ---
    IEnumerator Event50()
    {
        nextButton.SetActive(false);
        if (sfxSource != null && intercomClickSFX != null) sfxSource.PlayOneShot(intercomClickSFX);
        StartDialogue("", "Then the intercom clicks.");
        eventPos = 51;
        yield return null;
    }

    IEnumerator Event51()
    {
        nextButton.SetActive(false);
        StartDialogue("", "For a moment—\nit feels normal.");
        eventPos = 52;
        yield return null;
    }

    IEnumerator Event52()
    {
        nextButton.SetActive(false);
        if (sfxSource != null && angelusPrayerDistorted != null)
        {
            sfxSource.clip = angelusPrayerDistorted;
            sfxSource.Play();
        }
        StartDialogue("Intercom", "Angelus Domini nuntiavit Mariae…");
        eventPos = 53;
        yield return null;
    }

    IEnumerator Event53()
    {
        nextButton.SetActive(false);
        charDarlene.SetActive(true);
        SetExpression(charDarlene, darleneNeutral);
        StartDialogue("Darlene", "…Ang late na.");
        eventPos = 54;
        yield return null;
    }

    IEnumerator Event54()
    {
        nextButton.SetActive(false);
        charDarlene.SetActive(false);
        charKuh.SetActive(true);
        SetExpression(charKuh, kuhNeutral);
        StartDialogue("Kuh", "…Pero ang aga naman for prayer?");
        eventPos = 55;
        yield return null;
    }

    IEnumerator Event55()
    {
        charKuh.SetActive(false);
        nextButton.SetActive(false);
        StartCoroutine(GlitchEffect());
        StartDialogue("Intercom", "…et concepit de Spiritu—");
        eventPos = 56;
        yield return null;
    }

    IEnumerator Event56()
    {
        nextButton.SetActive(false);
        StartDialogue("Intercom", "Students.");
        eventPos = 57;
        yield return null;
    }

    IEnumerator Event57()
    {
        nextButton.SetActive(false);
        StartDialogue("Intercom", "…report to the rooftop.");
        eventPos = 58;
        yield return null;
    }

    IEnumerator Event58()
    {
        nextButton.SetActive(false);
        StartDialogue("Intercom", "…5:17 PM.");
        eventPos = 59;
        yield return null;
    }

    IEnumerator Event59()
    {
        nextButton.SetActive(false);
        charCristel.SetActive(true);
        StartDialogue("Cristel", "…Again?");
        eventPos = 60;
        yield return null;
    }

    IEnumerator Event60()
    {
        charCristel.SetActive(false);
        nextButton.SetActive(false);
        if (sfxSource != null && doorUnlockSFX != null) sfxSource.PlayOneShot(doorUnlockSFX);
        StartDialogue("", "The door unlocks.\n\nBut something else doesn’t.");
        yield return WaitForText();
        fadeOut.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(3);
    }

    // --- UTILS ---
    void StartDialogue(string speaker, string line)
    {
        mainTextObject.SetActive(true);
        charName.GetComponent<TMPro.TMP_Text>().text = speaker;
        textToSpeak = line;
        TextCreator.fullText = textToSpeak;
        TextCreator.charCount = 0;
        currentTextLength = textToSpeak.Length;
        textLength = 0;
        TextCreator.runTextPrint = true;
        isTyping = true;
    }

    IEnumerator GlitchEffect()
    {
        if (sfxSource != null && glitchSFX != null) sfxSource.PlayOneShot(glitchSFX);
        if (glitchOverlay != null)
        {
            glitchOverlay.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            glitchOverlay.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            glitchOverlay.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            glitchOverlay.SetActive(false);
        }
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

    public void NextButton()
    {
        if (isTyping) return; // Prevent skipping while typing

        if (eventPos == 1) StartCoroutine(EventOne());
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

        else if (eventPos == 20) StartCoroutine(Event20());
        else if (eventPos == 21) StartCoroutine(Event21());
        else if (eventPos == 22) StartCoroutine(Event22());
        else if (eventPos == 23) StartCoroutine(Event23());

        else if (eventPos == 30) StartCoroutine(Event30());
        else if (eventPos == 31) StartCoroutine(Event31());
        else if (eventPos == 32) StartCoroutine(Event32());
        else if (eventPos == 33) StartCoroutine(Event33());

        else if (eventPos == 40) StartCoroutine(Event40());
        else if (eventPos == 41) StartCoroutine(Event41());
        else if (eventPos == 42) StartCoroutine(Event42());

        else if (eventPos == 50) StartCoroutine(Event50());
        else if (eventPos == 51) StartCoroutine(Event51());
        else if (eventPos == 52) StartCoroutine(Event52());
        else if (eventPos == 53) StartCoroutine(Event53());
        else if (eventPos == 54) StartCoroutine(Event54());
        else if (eventPos == 55) StartCoroutine(Event55());
        else if (eventPos == 56) StartCoroutine(Event56());
        else if (eventPos == 57) StartCoroutine(Event57());
        else if (eventPos == 58) StartCoroutine(Event58());
        else if (eventPos == 59) StartCoroutine(Event59());
        else if (eventPos == 60) StartCoroutine(Event60());
    }
}