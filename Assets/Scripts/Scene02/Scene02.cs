using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene02 : MonoBehaviour
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
    public GameObject choicePanelRaven; // For Choice 6
    public GameObject choicePanelJump;  // For the JUMP choice sequence
    public CanvasGroup fadeCanvasGroup;

    [Header("Backgrounds")]
    public GameObject currentBG;
    public GameObject staircaseSunsetBG;
    public GameObject highSchoolBG;
    public GameObject canteenBG;
    public GameObject rooftopBG;
    public GameObject blackBG;

    [Header("Portraits")]
    public GameObject cristelNeutral; public GameObject cristelFrown; public GameObject cristelSmile;
    public GameObject marcLaugh; public GameObject marcNeutral; public GameObject marcChide;
    public GameObject kuhNeutral; public GameObject kuhScared;
    public GameObject ravenNeutral;
    public GameObject darleneNeutral; public GameObject darleneSad;

    [Header("Distorted Portraits")]
    public GameObject shadowedDarlene;
    public GameObject shadowedRaven;
    public GameObject shadowedMarc;
    public GameObject shadowedCristel;
    public GameObject highschoolCristel;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioClip intercomCrackle;
    public AudioClip angelusPrayer;
    public AudioClip staticCut;
    public AudioClip glitchSFX;
    public AudioClip bgFadeSFX;
    public AudioClip heavyBreathing;

    void Start()
    {
        if (staircaseSunsetBG != null)
        {
            currentBG = staircaseSunsetBG;
            staircaseSunsetBG.SetActive(true);
        }

        if (textBox != null) textBox.SetActive(false);
        if (choicePanelRaven != null) choicePanelRaven.SetActive(false);
        if (choicePanelJump != null) choicePanelJump.SetActive(false);
        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 0;

        EnqueueScene02();
        ShowNextLine();
    }

    void Update()
    {
        skipMode = Input.GetKey(KeyCode.LeftControl);
        if (skipMode && !isTyping && !choicePanelRaven.activeSelf && !choicePanelJump.activeSelf) ShowNextLine();
    }

    void EnqueueScene02()
    {
        // --- START OF SCENE 02 ---
        dialogueQueue.Enqueue(new DialogueLine("Marc", "…Ano tapos kana?", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Ako?", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "hinde joke lang pala, gusto mo party hat?", marcLaugh));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Since ikaw unang nakapansin..", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Ano sa tingin mo ven?", cristelNeutral));

        // Choice 6 Trigger
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE_RAVEN_6]"));

        dialogueQueue.Enqueue(new DialogueLine("", "No one else replied."));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[SFX_INTERCOM_START]"));
        dialogueQueue.Enqueue(new DialogueLine("INTERCOM", "Angelus Domini nuntiavit Mariae…"));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "…Again?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[SFX_STATIC_CUT]"));
        dialogueQueue.Enqueue(new DialogueLine("INTERCOM", "…Proceed."));

        dialogueQueue.Enqueue(new DialogueLine("Marc", "Ang bossy naman ng prayer na to.", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Kuya..", darleneNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Suddenly, Kuh stops on her tracks."));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Guys naririnig nyo ba yon?", kuhScared));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Hinde, naiwan ko vape ko eh wala ako sa sarili ko.", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Hindi– parang–", kuhScared));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Parang..?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Si Cristel ba yon?!?!", kuhScared));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "ha? Andito ako!", cristelNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Cristel is standing right beside us. So why does Raven hear her voice upstairs too?"));
        dialogueQueue.Enqueue(new DialogueLine("", "No. Not hear. Remember."));
        dialogueQueue.Enqueue(new DialogueLine("", "But before anything else, Kuh was already running up ahead."));

        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Kuh?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Kuh wait–", darleneSad));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "What the helly?", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "ven tignan mo tong pader may swastika", marcNeutral));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));

        // --- CHAPTER 3: KUH POV ---
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[CHAPTER_3_START]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));

        dialogueQueue.Enqueue(new DialogueLine("", "Kuh doesn’t really know what’s happening. She just hears her name being called."));
        dialogueQueue.Enqueue(new DialogueLine("Voice", "Kuh?"));
        dialogueQueue.Enqueue(new DialogueLine("Voice", "Kuh jane?"));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Si Cristel ba yon?!?!", kuhScared));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Kuh? Hello?", cristelNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "It is Cristel! But wait, Kuh was sure she was just with them."));
        dialogueQueue.Enqueue(new DialogueLine("", "Suddenly, her legs started moving on their own. Towards the voice."));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "!!?", kuhScared));
        dialogueQueue.Enqueue(new DialogueLine("", "Kuh doesn’t realize, she was already running without her permission."));

        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Kuh!", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Aray, Kuh!", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Tel? Tel! Asan ka?!", kuhScared));

        dialogueQueue.Enqueue(new DialogueLine("", "The staircase keeps going. Not looping. Not repeating. Just—continuing longer than it should."));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "What.. What the…", kuhScared));

        dialogueQueue.Enqueue(new DialogueLine("", "Then there was a figure just a few steps ahead. Their face was obscured with shadow."));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "!!?", kuhScared));
        dialogueQueue.Enqueue(new DialogueLine("Person", "Hindi ka namin maintindihan."));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Ha? Parang ikaw hindi ko maintindihan eh!", kuhScared));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_HIGHSCHOOL]"));

        dialogueQueue.Enqueue(new DialogueLine("", "Suddenly, Kuh is in a different place. Teenagers in uniforms, an unfamiliar school."));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Where am I?", kuhScared));
        dialogueQueue.Enqueue(new DialogueLine("Person", "Lagi ka nalang papansin, noh?"));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Ang kapal mo naman po kung ganon.", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Person", "Hindi mo ba narerealize walang gustong makitropa sayo? Iba kasi ang trip mo eh."));
        dialogueQueue.Enqueue(new DialogueLine("Highschool Cristel", "Gusto ko lang naman kayo maging kaibigan..", highschoolCristel));
        dialogueQueue.Enqueue(new DialogueLine("Person", "Kaso, nakakahiya kang kasama. Pano yun?"));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Eh kung suntukin kaya kita—", kuhNeutral));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[GLITCH]"));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Oo okay lang naman ako, thank you sa pagtatanong.", cristelSmile));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "You’re welcome.", marcNeutral)); // Glitching as Marc
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "??", kuhScared));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_BLACK]"));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "what is happening..?!", kuhScared));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_CANTEEN]"));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "This is our college! Am i finally back?! But where are they?", kuhScared));
        dialogueQueue.Enqueue(new DialogueLine("Voice", "Cristel!"));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Kakain kami sa uncle johns, sama ikaw?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Sunod nalang ako may inaantay kasi ako.", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Okay! Sunod ka ah!", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Sinong inaantay neto?", kuhNeutral));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_STAIRCASE]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));

        // --- DISTORTED FRIENDS ---
        dialogueQueue.Enqueue(new DialogueLine("Shadowed Darlene", "Bakit mo naman kami iniwan?", shadowedDarlene));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "OMAYGAD!?", kuhScared));
        dialogueQueue.Enqueue(new DialogueLine("Shadowed Raven", "Naiintindihan mo ba talaga kami, Kuh?", shadowedRaven));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "SINO BA KAYO? MUKHA KAYONG MGA AI", kuhScared));
        dialogueQueue.Enqueue(new DialogueLine("Shadowed Marc", "Hehe, kami ba talaga ang AI o ikaw?", shadowedMarc));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "LAHAT KAYO", kuhScared));
        dialogueQueue.Enqueue(new DialogueLine("Shadowed Cristel", "Kuh.. wag mo naman ako hayaan mawala..", shadowedCristel));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "ok Ggs gooodgame, ggwap, nice g, well played", kuhScared));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[GOTO_PACMAN]"));

        // --- ROOFTOP SECTION (POST MINIGAME) ---
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_ROOFTOP]"));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Why am I here now?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Where is Cristel?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[GLITCH]"));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Matatapos na din lahat…", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "HUH?!!", kuhScared));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Teka lang–", kuhScared));
        dialogueQueue.Enqueue(new DialogueLine("", "Kuh tries to stop herself from walking but she couldn't."));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Makakalaya na rin ako…", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "No no no no no…", kuhScared));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE_JUMP]"));

        dialogueQueue.Enqueue(new DialogueLine("Kuh", "AHH!!", kuhScared));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_STAIRCASE]"));
        dialogueQueue.Enqueue(new DialogueLine("", "Kuh grabs the railing tightly."));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Kuh!", darleneSad));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Kuh! Its ok we’re here what happened?", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Why did you run?", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Hayyy, grabe, hinihingal ako wala ung vape ko.", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Cristel.", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Bakit Kuh?", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Usap tayo.", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Yes! Finally! Bravo! May nag step up din!", marcLaugh));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[SCENE_END]"));
    }

    public void OnChoice6Selected(int index)
    {
        choicePanelRaven.SetActive(false);
        usingTempQueue = true;
        tempQueue.Clear();

        switch (index)
        {
            case 0: // 6A
                tempQueue.Enqueue(new DialogueLine("Marc", "Syempre ako nalang lagi may kasalanan diba?", marcChide));
                break;
            case 1: // 6B
                tempQueue.Enqueue(new DialogueLine("Marc", "Ayos, sana nagblockblast nalang din ako.", marcNeutral));
                break;
            case 2: // 6C
                tempQueue.Enqueue(new DialogueLine("Marc", "…Sana makita nyo rin yung perspective ko kung ganyan.", marcNeutral));
                break;
            case 3: // 6D
                tempQueue.Enqueue(new DialogueLine("Marc", "…I agree.", marcNeutral));
                break;
        }
        ShowNextLine();
    }

    public void OnJumpSelected()
    {
        choicePanelJump.SetActive(false);
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
            case "[CHOICE_RAVEN_6]":
                choicePanelRaven.SetActive(true);
                nextButton.SetActive(false);
                return true;
            case "[CHOICE_JUMP]":
                choicePanelJump.SetActive(true);
                nextButton.SetActive(false);
                return true;
            case "[SFX_INTERCOM_START]":
                if (sfxSource && intercomCrackle) sfxSource.PlayOneShot(intercomCrackle);
                if (musicSource && angelusPrayer) musicSource.PlayOneShot(angelusPrayer);
                return false;
            case "[SFX_STATIC_CUT]":
                if (sfxSource && staticCut) sfxSource.PlayOneShot(staticCut);
                return false;
            case "[GLITCH]":
                if (sfxSource && glitchSFX) sfxSource.PlayOneShot(glitchSFX);
                return false;
            case "[FADE_OUT]":
                StartFade(1.0f);
                return true;
            case "[FADE_IN]":
                StartFade(0.0f);
                return true;
            case "[BG_HIGHSCHOOL]": StartBGTransition(highSchoolBG); return false;
            case "[BG_CANTEEN]": StartBGTransition(canteenBG); return false;
            case "[BG_ROOFTOP]": StartBGTransition(rooftopBG); return false;
            case "[BG_STAIRCASE]": StartBGTransition(staircaseSunsetBG); return false;
            case "[BG_BLACK]": StartBGTransition(blackBG); return false;
            case "[GOTO_PACMAN]":
                SceneManager.LoadScene("PacmanChaseScene");
                return true;
            case "[SCENE_END]":
                SceneManager.LoadScene("Scene03");
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
        if (bgFadeSFX) sfxSource.PlayOneShot(bgFadeSFX);
        if (currentBG) currentBG.SetActive(false);
        newBG.SetActive(true);
        currentBG = newBG;
        yield return null;
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

    void HideAllPortraits()
    {
        GameObject[] ports = { cristelNeutral, cristelFrown, cristelSmile, marcLaugh, marcNeutral, marcChide, kuhNeutral, kuhScared, ravenNeutral, darleneNeutral, darleneSad, shadowedDarlene, shadowedRaven, shadowedMarc, shadowedCristel, highschoolCristel };
        foreach (var p in ports) if (p) p.SetActive(false);
    }
}