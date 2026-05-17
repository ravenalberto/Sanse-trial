using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene01 : MonoBehaviour
{
    // Static state tracker to maintain progress across scene loads
    public static bool cameFromBlockPuzzle = false;
    private static string lastActiveScene = "";

    // AUTOMATIC SCENE TRACKER: Automatically sets cameFromBlockPuzzle to true when returning from the minigame
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void TrackSceneChanges()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private static void OnActiveSceneChanged(Scene current, Scene next)
    {
        lastActiveScene = current.name;
        if (next.name == "Scene01" && lastActiveScene == "BlockPuzzleScene")
        {
            cameFromBlockPuzzle = true;
            Debug.Log("<color=green>Scene01 System:</color> Automatically detected transition back from 'BlockPuzzleScene'. cameFromBlockPuzzle has been set to true!");
        }
    }

    private Dictionary<string, int> trustScores = new Dictionary<string, int>();

    public void AddTrust(string characterName, int amount)
    {
        if (!trustScores.ContainsKey(characterName)) trustScores[characterName] = 0;
        trustScores[characterName] += amount;
        Debug.Log($"<color=cyan>Trust Update:</color> {characterName} is now at {trustScores[characterName]}");
    }

    private Coroutine typingCoroutine;
    private Coroutine fadeCoroutine;
    private Coroutine bgFadeCoroutine;
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

    [Header("Choice Panels")]
    public GameObject choicePanel;      // For Choice 5 (Gates)
    public GameObject choicePanel6;     // For Choice 6 (Post-Puzzle)
    public Button choiceButtonB;
    public GameObject gcBackgroundUI;

    [Header("Notification UI Design")]
    public GameObject notificationA;
    public GameObject notificationB;
    [Tooltip("Design for Choice 6A: Marc's fault (Trust down)")]
    public GameObject notification6A;
    [Tooltip("Design for Choice 6B: I don't know")]
    public GameObject notification6B;
    [Tooltip("Design for Choice 6C: It's not that simple (Trust up)")]
    public GameObject notification6C;
    [Tooltip("Design for Choice 6D: We all let it happen (Trust up)")]
    public GameObject notification6D;
    public float notificationDisplayTime = 3.0f;

    [Header("Backgrounds")]
    public GameObject schoolGatesBG;
    public GameObject hallwayBG;
    public GameObject classroomBG;
    public GameObject staircaseSunsetBG;
    public GameObject phoneBG;
    public GameObject phonemessageBG;
    private GameObject currentBG;
    public CanvasGroup fadeCanvasGroup;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioClip angelusSFX;
    public AudioClip intercomSFX;
    public AudioClip glitchSFX;
    public AudioClip bellSFX;
    public AudioClip stretchSFX;
    public AudioClip bgFadeSFX;
    public AudioClip staticCrackleSFX; // Intercom Static Sound
    public AudioClip staticCutSFX;     // Static abrupt cut sound
    public AudioClip notificationSFX;  // Sound effect for the notifications

    [Header("Portraits")]
    public GameObject cristelNeutral; public GameObject cristelFrown; public GameObject cristelSmile;
    public GameObject marcLaugh; public GameObject marcNeutral; public GameObject marcChide;
    public GameObject kuhNeutral; public GameObject ravenNeutral;
    public GameObject darleneNeutral; public GameObject darleneSad;

    private int glitchedClickCount = 0;

    void Start()
    {
        // Initial setup and resets
        DisableAllBGs();
        if (textBox != null) textBox.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
        if (choicePanel6 != null) choicePanel6.SetActive(false);
        if (gcBackgroundUI != null) gcBackgroundUI.SetActive(false);

        // Ensure all choice notifications start hidden
        if (notificationA != null) notificationA.SetActive(false);
        if (notificationB != null) notificationB.SetActive(false);
        if (notification6A != null) notification6A.SetActive(false);
        if (notification6B != null) notification6B.SetActive(false);
        if (notification6C != null) notification6C.SetActive(false);
        if (notification6D != null) notification6D.SetActive(false);

        if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 0;

        // Check if we are returning from the minigame
        if (cameFromBlockPuzzle)
        {
            cameFromBlockPuzzle = false; // reset
            EnqueueScene01PostPuzzle();
        }
        else
        {
            EnqueueScene01();
        }

        ShowNextLine();
    }

    void Update()
    {
        skipMode = Input.GetKey(KeyCode.LeftControl);
        if (skipMode && !isTyping && !choicePanel.activeSelf && !choicePanel6.activeSelf) ShowNextLine();
    }

    void EnqueueScene01()
    {
        // --- ANGELUS START ---
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[SFX_ANGELUS]"));
        dialogueQueue.Enqueue(new DialogueLine("Intercom", "Angelus Domini nuntiavit Mariae…"));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "…Ang late na.", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "…Pero ang aga naman for prayer?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Intercom", "…et concepit de Spiritu—"));
        dialogueQueue.Enqueue(new DialogueLine("Intercom", "Students."));
        dialogueQueue.Enqueue(new DialogueLine("Intercom", "…5:17 PM."));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_GATES]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));

        // --- CHAPTER 2: RAVEN POV ---
        dialogueQueue.Enqueue(new DialogueLine("", "Raven was on her way to school after playing a video game."));
        dialogueQueue.Enqueue(new DialogueLine("", "As she walks towards the campus gates she notices a familiar figure standing under the shed."));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Cristel?", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("", "Cristel stood, looking at her phone, barely registering Raven."));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Yo cristel?", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("", "She calls again this time, she was closer. Cristel jumps."));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Oh! I didn't see you there!", cristelSmile));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "No worries im on my way anyways–", ravenNeutral));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[SFX_GLITCH]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[SHOW_GC]"));
        dialogueQueue.Enqueue(new DialogueLine("GC", "“Guys asan na yung iba?”\n“Andito na si sir oh.”"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[HIDE_GC]"));

        dialogueQueue.Enqueue(new DialogueLine("Raven", "“Crap, they’re looking for us. Should we go?”", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Its fine, mauna kana muna. Sunod ako.", cristelSmile));
        dialogueQueue.Enqueue(new DialogueLine("", "Cristel smiles. But Raven notices something strange. Not sadness. Not fear. Delay."));
        dialogueQueue.Enqueue(new DialogueLine("", "Like Cristel has to remember how to react first."));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Sure ka?", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Oo, may inaantay kasi ako.", cristelSmile));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE_5]"));

        dialogueQueue.Enqueue(new DialogueLine("Raven", "Sunod ka nalang.", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Sige ven. Mamaya nalang", cristelSmile));
        dialogueQueue.Enqueue(new DialogueLine("", "She smiles, but it doesn’t feel real. Raven shakes it off and heads to class."));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_HALLWAY]"));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Uy ven, san na si tetel?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Susunod daw sya eh", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "as if. Lagi naman yan ganyan susunod pero hindi talaga. Boka yan. For sure may tinatago na naman yan satin.", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Kuya naman! Tama na, ring ka hanggang dito!", darleneNeutral));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_CLASSROOM]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));

        // --- CLASSROOM SECTION ---
        dialogueQueue.Enqueue(new DialogueLine("", "Cristel wasn’t acting alright. It’s currently 5:16pm and many of our classmates are already heading home."));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Do you think we should check on her?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Baka need nya muna mapag isa..?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Wag na, yaan nyo nayan wala na yang pake satin.", marcNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Raven shakes her head and decides to focus on finishing up the document."));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[SFX_GLITCH]"));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Finally. That felt like days! I’m finally done."));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "What’s that? Did you say something?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Huh? Me? No?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "I think we should go home.", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "What time is it? Cristel hasn’t gone back yet", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Cristel hasn’t come back?", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Hasn’t it been an hour already? 5:17 PM? Is the clock broken?"));
        dialogueQueue.Enqueue(new DialogueLine("", "She finished an entire document. That should’ve taken at least an hour."));

        dialogueQueue.Enqueue(new DialogueLine("Kuh", "I’ll go see Cristel.", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("", "Marc sat silently playing a gambling game on the pc. Darlene sighs."));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Kuya talaga. Puntahan muna namin si Cristel ah.", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Sige, baka mabawi pa yung kulam nya satin.", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Need lang naman natin sya intindihin Kuya–", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "“Oh talaga? So tayo nalang lagi iintindi sa kanya? Pano tayo? Pano ako? Lagi nalang ganyan?”", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "“Kuya naman please…”", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "“Ayaw mo maniwala sakin kausapin mo sya–”", marcNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "(The door opens.)"));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "“Ano nanaman nangyayari dito jusko”", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Why do i feel like… i’ve seen this before…", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "“Oh ayan speak of the devil, buti dumating pa yan”", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "“Bakit ano bang meron.”", cristelFrown));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));

        // --- OBSERVATIONS ---
        dialogueQueue.Enqueue(new DialogueLine("", "After that message appeared on the board, Raven had been observing every single one of them."));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "(Darlene seemed to be the least aware of what happened. But she tries.)", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "(Kuh acted slightly off. Is this really her?)", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "(Marc keeps targeting Cristel. Too specifically.)", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "(Cristel Smiles. But Cristel is not okay.)", cristelSmile));

        dialogueQueue.Enqueue(new DialogueLine("", "She finds herself walking with her friends towards the staircase."));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_STAIRCASE]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));

        // --- STAIRCASE SUNSET ---
        dialogueQueue.Enqueue(new DialogueLine("", "They shouldn’t be going. Impossible things are easier to dismiss until they keep happening."));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Come on… it’s probably just another clue.", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Since when do we follow instructions from a broken intercom?", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "We followed worse. You know? Like Cristel.", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Ano nanaman ba ginawa ko sayo?", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Ano bang hindi mo ginawa?", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Can we not joke right now?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Sinong nagsabing nagjojoke ako?", marcNeutral));

        dialogueQueue.Enqueue(new DialogueLine("Cristel", "It’s just another puzzle. Like earlier.", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Walang nagtanong sayo.", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "I mean may point si cristel. Kita mo naman yung kanina.", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "So kampi ka na rin sa kanya. Nice. Kala ko panaman matino ka.", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Wow.", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Guys ano ba naman, titigil ba kayo?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "hindi ko kasi alam bakit kanina pa ako inaano neto.", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Kuya, Cristel, please naman. Lahat naman tayo gusto na makaalis dito at makauwi..", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "yun pala so bakit tayo pupunta sa rooftop? Magpaparty muna? Sabihin nyo lang, bibili na ako ng alfonso.", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Ugh.. This is too much.."));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_PHONE]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));
        dialogueQueue.Enqueue(new DialogueLine("", "Raven pulls out her phone."));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("", "Click and drop the randomized block shapes onto the grid to complete full rows or columns, which blasts them off the board."));
        dialogueQueue.Enqueue(new DialogueLine("", "If the board fills up and none of your available block shapes can fit into the remaining open spaces,"));
        dialogueQueue.Enqueue(new DialogueLine("", "it's an instant Game Over—even if there is still time left on the clock."));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[GOTO_MAZE]"));
    }

    void EnqueueScene01PostPuzzle()
    {
        dialogueQueue.Clear();
        tempQueue.Clear();

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_STAIRCASE]"));
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
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Guys naririnig nyo ba yon?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Hinde, naiwan ko vape ko eh wala ako sa sarili ko.", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Hindi– parang–", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Parang..?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Si Cristel ba yon?!?!", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "ha? Andito ako!", cristelNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Cristel is standing right beside us. So why does Raven hear her voice upstairs too? No.Not hear. Remember."));
        dialogueQueue.Enqueue(new DialogueLine("", "But before anything else, Kuh was already running up ahead."));

        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Kuh?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Kuh wait–", darleneSad));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "What the helly?", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "ven tignan mo tong pader may swastika", marcNeutral));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[GOTO_SCENE02]"));
    }

    public void OnChoiceSelected(int index)
    {
        if (index == 1)
        {
            glitchedClickCount++;
            if (sfxSource != null && glitchSFX != null) sfxSource.PlayOneShot(glitchSFX);

            if (glitchedClickCount == 1) dialogueText.text = "HUh? Why can’t I click it?";
            else if (glitchedClickCount == 2) dialogueText.text = "…No.";
            else if (glitchedClickCount >= 3) dialogueText.text = "Why am I allowed to?";

            return;
        }

        choicePanel.SetActive(false);
        glitchedClickCount = 0;
        TriggerNotification(notificationA);
        ShowNextLine();
    }

    public void OnChoice6Selected(int index)
    {
        choicePanel6.SetActive(false);
        usingTempQueue = true;
        tempQueue.Clear();

        switch (index)
        {
            case 0: // Choice 6A
                AddTrust("Marc", -10);
                TriggerNotification(notification6A);
                tempQueue.Enqueue(new DialogueLine("Marc", "Syempre ako nalang lagi may kasalanan diba?", marcChide));
                break;
            case 1: // Choice 6B
                AddTrust("Marc", 0);
                TriggerNotification(notification6B);
                tempQueue.Enqueue(new DialogueLine("Marc", "Ayos, sana nagblockblast nalang din ako.", marcNeutral));
                break;
            case 2: // Choice 6C
                AddTrust("Marc", 10);
                TriggerNotification(notification6C);
                tempQueue.Enqueue(new DialogueLine("Marc", "…Sana makita nyo rin yung perspective ko kung ganyan.", marcNeutral));
                break;
            case 3: // Choice 6D
                AddTrust("Marc", 15);
                TriggerNotification(notification6D);
                tempQueue.Enqueue(new DialogueLine("Marc", "…I agree.", marcNeutral));
                break;
        }

        ShowNextLine();
    }

    // --- NOTIFICATION HANDLER SYSTEM ---
    private void TriggerNotification(GameObject notificationObj)
    {
        if (notificationObj != null)
        {
            StartCoroutine(ShowNotificationRoutine(notificationObj));
        }
    }

    private IEnumerator ShowNotificationRoutine(GameObject notificationObj)
    {
        notificationObj.SetActive(true);
        if (sfxSource != null && notificationSFX != null) sfxSource.PlayOneShot(notificationSFX);
        yield return new WaitForSeconds(notificationDisplayTime);
        notificationObj.SetActive(false);
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
            if (tempQueue.Count == 0) usingTempQueue = false;
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

    bool HandleSystemCommand(string command)
    {
        switch (command)
        {
            case "[CHOICE_5]":
                choicePanel.SetActive(true);
                nextButton.SetActive(false);
                return true;
            case "[CHOICE_RAVEN_6]":
                choicePanel6.SetActive(true);
                nextButton.SetActive(false);
                return true;
            case "[SFX_ANGELUS]":
                if (musicSource != null && angelusSFX != null) musicSource.PlayOneShot(angelusSFX);
                return false;
            case "[SFX_GLITCH]":
                if (sfxSource != null && glitchSFX != null) sfxSource.PlayOneShot(glitchSFX);
                return false;
            case "[SFX_STRETCH]":
                if (sfxSource != null && stretchSFX != null) sfxSource.PlayOneShot(stretchSFX);
                return false;
            case "[SFX_INTERCOM_START]":
                if (sfxSource != null && staticCrackleSFX != null) sfxSource.PlayOneShot(staticCrackleSFX);
                if (musicSource != null && angelusSFX != null) musicSource.PlayOneShot(angelusSFX);
                return false;
            case "[SFX_STATIC_CUT]":
                if (sfxSource != null && staticCutSFX != null) sfxSource.PlayOneShot(staticCutSFX);
                return false;
            case "[FADE_OUT]":
                StartFade(1.0f, 0.8f);
                if (textBox != null) textBox.SetActive(false);
                return true;
            case "[FADE_IN]":
                StartFade(0.0f, 0.8f);
                return true;
            case "[SHOW_GC]":
                if (gcBackgroundUI != null) gcBackgroundUI.SetActive(true);
                return false;
            case "[HIDE_GC]":
                if (gcBackgroundUI != null) gcBackgroundUI.SetActive(false);
                return false;
            case "[BG_GATES]":
                StartBGTransition(schoolGatesBG); return false;
            case "[BG_HALLWAY]":
                StartBGTransition(hallwayBG); return false;
            case "[BG_CLASSROOM]":
                StartBGTransition(classroomBG); return false;
            case "[BG_STAIRCASE]":
                StartBGTransition(staircaseSunsetBG); return false;
            case "[BG_PHONE]":
                StartBGTransition(phoneBG); return false;
            case "[BG_PHONEMESSAGE]":
                StartBGTransition(phonemessageBG); return false;
            case "[GOTO_MAZE]":
                SceneManager.LoadScene("BlockPuzzleScene");
                return true;
            case "[GOTO_SCENE02]":
                SceneManager.LoadScene("Scene02");
                return true;
            default: return false;
        }
    }

    // --- BACKGROUND TRANSITION LOGIC ---
    void StartBGTransition(GameObject newBG)
    {
        if (newBG == currentBG) return;

        if (bgFadeCoroutine != null)
            StopCoroutine(bgFadeCoroutine);

        bgFadeCoroutine = StartCoroutine(FadeBackground(newBG));
    }

    IEnumerator FadeBackground(GameObject newBG)
    {
        float duration = 0.5f;

        if (bgFadeSFX != null && sfxSource != null)
            sfxSource.PlayOneShot(bgFadeSFX);

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
        else
        {
            newBG.SetActive(true);
        }

        currentBG = newBG;
    }

    void StartFade(float targetAlpha, float duration)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha, duration));
    }

    IEnumerator FadeRoutine(float targetAlpha, float duration)
    {
        if (fadeCanvasGroup == null)
        {
            yield return new WaitForSeconds(duration);
            ShowNextLine();
            yield break;
        }

        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
        ShowNextLine();
    }

    void DisableAllBGs()
    {
        if (schoolGatesBG) schoolGatesBG.SetActive(false);
        if (hallwayBG) hallwayBG.SetActive(false);
        if (classroomBG) classroomBG.SetActive(false);
        if (staircaseSunsetBG) staircaseSunsetBG.SetActive(false);
        if (phoneBG) phoneBG.SetActive(false);
        if (phonemessageBG) phonemessageBG.SetActive(false);
        currentBG = null;
    }

    void HideAllPortraits()
    {
        if (cristelNeutral) cristelNeutral.SetActive(false); if (cristelFrown) cristelFrown.SetActive(false); if (cristelSmile) cristelSmile.SetActive(false);
        if (marcLaugh) marcLaugh.SetActive(false); if (marcNeutral) marcNeutral.SetActive(false); if (marcChide) marcChide.SetActive(false);
        if (kuhNeutral) kuhNeutral.SetActive(false); if (ravenNeutral) ravenNeutral.SetActive(false);
        if (darleneNeutral) darleneNeutral.SetActive(false); if (darleneSad) darleneSad.SetActive(false);
    }
}