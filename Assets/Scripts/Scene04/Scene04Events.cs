using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scene04VN : MonoBehaviour
{
    private Dictionary<string, int> trustScores = new Dictionary<string, int>();

    public void AddTrust(string characterName, int amount)
    {
        if (!trustScores.ContainsKey(characterName)) trustScores[characterName] = 0;
        trustScores[characterName] += amount;
        Debug.Log($"<color=cyan>Trust Update:</color> {characterName} is now at {trustScores[characterName]}");
    }

    private Coroutine typingCoroutine;
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
    public GameObject choicePanel;

    [Header("Notification UI Design")]
    [Tooltip("Design for Choice A: Cristel will appreciate that")]
    public GameObject notificationA;
    [Tooltip("Design for Choice B: Cristel felt dismissed")]
    public GameObject notificationB;
    public float notificationDisplayTime = 3.0f;

    [Header("Backgrounds")]
    public GameObject hallwayBG;
    public GameObject stretchHallwayBG; // Optional: Separate sprite for the stretched hallway
    public GameObject fadeOverlay;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioClip ambientSunset;
    public AudioClip hugSFX;
    public AudioClip notificationSFX;
    public AudioClip stretchSFX; // SFX para sa pag-stretch ng hallway

    [Header("Portraits")]
    public GameObject cristelNeutral; public GameObject cristelFrown; public GameObject cristelSmile;
    public GameObject marcLaugh; public GameObject marcNeutral; public GameObject marcChide;
    public GameObject kuhNeutral; public GameObject ravenNeutral;
    public GameObject darleneNeutral; public GameObject darleneSad;

    void Start()
    {
        if (hallwayBG != null) hallwayBG.SetActive(true);
        if (textBox != null) textBox.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);

        if (notificationA != null) notificationA.SetActive(false);
        if (notificationB != null) notificationB.SetActive(false);

        if (musicSource != null && ambientSunset != null)
        {
            musicSource.clip = ambientSunset;
            musicSource.Play();
        }

        EnqueueChapter4();
        ShowNextLine();
    }

    void Update()
    {
        skipMode = Input.GetKey(KeyCode.LeftControl);
        if (skipMode && !isTyping && !choicePanel.activeSelf) ShowNextLine();
    }

    void EnqueueChapter4()
    {
        dialogueQueue.Enqueue(new DialogueLine("", "By some miracle, and quiet prayers of Darlene, they’ve reached the top floor hallway."));
        dialogueQueue.Enqueue(new DialogueLine("", "Everyone was looking at Cristel now."));

        dialogueQueue.Enqueue(new DialogueLine("Kuh", "May hindi kang sinasabi samin. Hindi kita inaaway, pero… Parang umiikot lahat pabalik sayo.", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Mali ba ako?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "“…What exactly are we missing?”", ravenNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Cristel looks away.", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "I didn’t think it was important eh..", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "You can’t be thinking that especially ganto sitwasyon natin.", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Kanina… (Sighs) Nung umakyat ako sa hagdan, narinig ko boses mo, actually, boses nyong lahat.", kuhNeutral));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));

        dialogueQueue.Enqueue(new DialogueLine("Raven", "So tama nga ang hinala ko. Stuck tayo sa reality na to.", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Hindi naman siguro stuck dito forever… diba?", darleneSad));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Naniniwala ka ba sa forever?", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Cristel, ano ba talaga meron?", kuhNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Cristel takes in a breath.", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Pagod na kasi ako…", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("", "Silence"));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "“Hindi ko na talaga alam pano magpatuloy minsan.”", cristelFrown));

        // Choice trigger
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE_DARLENE]"));

        // Reaction text depends on choice (handled in OnChoiceSelected)

        dialogueQueue.Enqueue(new DialogueLine("Cristel", "“May mga araw na iniisip ko…”", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "“…mas okay siguro pag wala nalang ako.”", cristelFrown));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[SFX_HUG]"));
        dialogueQueue.Enqueue(new DialogueLine("", "Darlene let out a sigh. She hugs Cristel. Kuh follows. Raven pats Cristel on the back."));

        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Cristel, sorry hindi namin alam ganyan na pala pinagdadaanan mo..", darleneSad));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Sorry kung nagkulang kami sayo tel..", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Pero nandito ka ngayon. Kasama mo kami.", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Alam mo naman andito lang kami lagi diba?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Siguro nga…", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Cristel… Naniniwala akong hanggang dulo, magkakasama parin tayo. So please… Wag tayong bibitaw.", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Promise?", darleneNeutral));

        dialogueQueue.Enqueue(new DialogueLine("Cristel", "(Nods slowly)", cristelSmile));
        dialogueQueue.Enqueue(new DialogueLine("", "Marc coughs in the corner, pretending to look somewhere else."));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Kuya, ikaw mag sorry ka rin. Ikaw talaga laging nang aano kay tetel..", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "ako?", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Oo sino paba", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Bat ako nag sosorry jan? Ano bang ginawa ko?", marcNeutral));

        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Dar, okay lang.", cristelSmile));
        dialogueQueue.Enqueue(new DialogueLine("", "Cristel gives Marc a knowing look before looking away."));

        // TRANSITION TO MAZE
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Uhh.. guys..?", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Bakit ven?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "YUng hallway…", ravenNeutral));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[SFX_STRETCH]"));
        dialogueQueue.Enqueue(new DialogueLine("", "Suddenly, the hallway stretches for miles. Doors and infinite pathways begin to appear."));

        // --- TUTORIAL / INSTRUCTIONS SEQUENCE ---
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("", "In the game, we must be able to escape and find the door."));
        dialogueQueue.Enqueue(new DialogueLine("", "Use WSAD to move and press E to open the door."));
        dialogueQueue.Enqueue(new DialogueLine("", "Follow the blood traces to find the door to the exit."));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[GOTO_MAZE]"));
    }

    public void OnChoiceSelected(int index)
    {
        choicePanel.SetActive(false);
        usingTempQueue = true;
        tempQueue.Clear();

        if (index == 0) // Choice A: Comfort
        {
            AddTrust("Cristel", 15);
            TriggerNotification(notificationA);
            tempQueue.Enqueue(new DialogueLine("Darlene", "Cristel.. andito kami. Hindi ka mag-isa.", darleneSad));
        }
        else // Choice B: Realistic
        {
            AddTrust("Cristel", -5);
            TriggerNotification(notificationB);
            tempQueue.Enqueue(new DialogueLine("Darlene", "Tetel, kailangan nating maging matatag ngayon.", darleneNeutral));
        }

        ShowNextLine();
    }

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
            case "[CHOICE_DARLENE]":
                choicePanel.SetActive(true);
                nextButton.SetActive(false);
                return true;
            case "[SFX_HUG]":
                if (sfxSource != null && hugSFX != null) sfxSource.PlayOneShot(hugSFX);
                return false;
            case "[SFX_STRETCH]":
                if (sfxSource != null && stretchSFX != null) sfxSource.PlayOneShot(stretchSFX);
                return false;
            case "[FADE_OUT]":
                fadeOverlay.SetActive(true);
                if (textBox != null) textBox.SetActive(false);
                return false;
            case "[FADE_IN]":
                fadeOverlay.SetActive(false);
                return false;
            case "[GOTO_MAZE]":
                // Palitan ito ng pangalan ng iyong maze scene
                SceneManager.LoadScene("CorridorMazeGame2dtry");
                return true;
            default: return false;
        }
    }

    void HideAllPortraits()
    {
        if (cristelNeutral) cristelNeutral.SetActive(false); if (cristelFrown) cristelFrown.SetActive(false); if (cristelSmile) cristelSmile.SetActive(false);
        if (marcLaugh) marcLaugh.SetActive(false); if (marcNeutral) marcNeutral.SetActive(false); if (marcChide) marcChide.SetActive(false);
        if (kuhNeutral) kuhNeutral.SetActive(false); if (ravenNeutral) ravenNeutral.SetActive(false);
        if (darleneNeutral) darleneNeutral.SetActive(false); if (darleneSad) darleneSad.SetActive(false);
    }
}