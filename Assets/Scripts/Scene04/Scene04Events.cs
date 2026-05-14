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

    [Header("UI Components")]
    public GameObject textBox; // The main panel containing the text
    public TMP_Text charNameText;
    public TMP_Text dialogueText;
    public GameObject nextButton;

    [Header("Backgrounds")]
    public GameObject hallwayBG;
    public GameObject classroomBG;
    public GameObject fadeOverlay;

    [Header("Portraits")]
    public GameObject cristelNeutral; public GameObject cristelFrown; public GameObject cristelSmile;
    public GameObject marcLaugh; public GameObject marcNeutral; public GameObject marcChide;
    public GameObject kuhNeutral; public GameObject ravenNeutral;
    public GameObject darleneNeutral; public GameObject darleneSad;

    void Start()
    {
        if (hallwayBG != null) hallwayBG.SetActive(true);
        // Ensure textBox starts off or on correctly
        if (textBox != null) textBox.SetActive(false);

        EnqueueChapter4();
        ShowNextLine();
    }

    void Update()
    {
        skipMode = Input.GetKey(KeyCode.LeftControl);
        if (skipMode && !isTyping) ShowNextLine();
    }

    void EnqueueChapter4()
    {
        // ... (Lines remain exactly the same as your provided code) ...
        dialogueQueue.Enqueue(new DialogueLine("", "By some miracle, and the quiet prayers I held in my heart, we reached the top floor hallway."));
        dialogueQueue.Enqueue(new DialogueLine("", "Everyone was looking at Cristel now."));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "May hindi ka sinasabi sa amin. Hindi kita inaaway, pero… parang umiikot lahat pabalik sayo.", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Mali ba ako?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "…What exactly are we missing?", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("", "Cristel looks away.", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "I didn’t think it was important eh..", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "You can’t be thinking that especially ganto sitwasyon natin. Kanina…", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "(Sighs) Nung umakyat ako sa hagdan, narinig ko boses nyo lahat. Actually... boses nyong lahat.", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "So tama nga ang hinala ko. Stuck tayo sa reality na to.", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Hindi naman siguro stuck dito forever… diba?", darleneSad));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Naniniwala ka ba sa forever?", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Cristel, ano ba talaga meron?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("", "(Cristel takes in a shaky breath.)", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Pagod na kasi ako…", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("", "..."));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Hindi ko na talaga alam pano magpatuloy minsan.", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Cristel..", darleneSad));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "May mga araw na iniisip ko…", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "…mas okay siguro pag wala nalang ako.", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("", "I let out a heavy sigh, but it wasn't out of frustration. It was out of love. I stepped forward and hugged her tight."));
        dialogueQueue.Enqueue(new DialogueLine("", "Kuh followed soon after, and Raven placed a gentle hand on her shoulder."));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Cristel, sorry... hindi namin alam ganyan na pala pinagdadaanan mo..", darleneSad));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "Sorry kung nagkulang kami sa'yo, Tel..", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Pero nandito ka ngayon. Kasama mo kami.", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "Alam mo naman andito lang kami lagi diba?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Siguro nga…", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Cristel… Naniniwala akong hanggang dulo, magkakasama parin tayo. So please… Wag tayong bibitaw.", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Promise?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "(Tumango dahan-dahan)", cristelSmile));
        dialogueQueue.Enqueue(new DialogueLine("", "We stayed like that for a while. A promise sealed in a sunset hallway."));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Ahem... (looks away awkwardly)", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Kuya, ikaw mag sorry ka rin! Ikaw talaga laging nang-aano kay Tetel.", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Ako? Galit agad?", marcLaugh));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Dar, okay lang.", cristelSmile));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_OUT]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_CLASSROOM]"));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[FADE_IN]"));
        dialogueQueue.Enqueue(new DialogueLine("", "The sunset was gone. The air felt normal again—dry and smelling of chalk. We were back in the classroom."));
        dialogueQueue.Enqueue(new DialogueLine("", "Everything felt... fine. Almost too fine."));
        dialogueQueue.Enqueue(new DialogueLine("", "Marc pulled a chair and sat right beside me."));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Dar.", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "Kuya?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "Di ka naman siguro nauto na eto talaga yung nangyari, noh Dar?", marcNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "What are you talking about, Kuya?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[SCENE_END]"));
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

        DialogueLine line = usingTempQueue ? tempQueue.Dequeue() : dialogueQueue.Dequeue();

        if (line.speaker == "SYSTEM")
        {
            if (!HandleSystemCommand(line.text)) ShowNextLine();
            return;
        }

        currentLine = line;

        // FIX: Ensure the textbox is visible when showing dialogue
        if (textBox != null) textBox.SetActive(true);

        HideAllPortraits();
        if (line.portrait != null) line.portrait.SetActive(true);
        charNameText.text = line.speaker;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(line.text));
    }

    private bool isTyping = false;
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
            case "[FADE_OUT]":
                fadeOverlay.SetActive(true);
                if (textBox != null) textBox.SetActive(false); // Hide text during fade
                return false;
            case "[FADE_IN]":
                fadeOverlay.SetActive(false);
                return false;
            case "[BG_CLASSROOM]":
                hallwayBG.SetActive(false);
                classroomBG.SetActive(true);
                return false;
            case "[SCENE_END]":
                SceneManager.LoadScene("Final_Scene");
                return true;
            default: return false;
        }
    }

    void HideAllPortraits()
    {
        cristelNeutral.SetActive(false); cristelFrown.SetActive(false); cristelSmile.SetActive(false);
        marcLaugh.SetActive(false); marcNeutral.SetActive(false); marcChide.SetActive(false);
        kuhNeutral.SetActive(false); ravenNeutral.SetActive(false);
        darleneNeutral.SetActive(false); darleneSad.SetActive(false);
    }
}