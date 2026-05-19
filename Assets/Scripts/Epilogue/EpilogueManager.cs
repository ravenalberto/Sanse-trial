using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EpilogueManager : MonoBehaviour
{

    private int currentLineIndex = 0; // Tracks dialogue progression
    public class DialogueLine
    {
        public string speaker;
        public string text;

        public GameObject portrait;
        public GameObject background;

        public DialogueLine(
            string s,
            string t,
            GameObject p,
            GameObject b
        )
        {
            speaker = s;
            text = t;
            portrait = p;
            background = b;
        }
    }

    List<DialogueLine> dialogue = new List<DialogueLine>();

    [Header("UI")]
    public GameObject textBox;
    public GameObject hudPauseButtonObject;

    public TMP_Text charNameText;
    public TMP_Text dialogueText;

    public GameObject nextButton;

    [Header("Typing")]
    public float textSpeed = 0.03f;

    private bool isTyping = false;
    private Coroutine typingCoroutine;

    int currentLine = 0;

    [Header("Portraits")]
    public GameObject ravenNeutral;
    public GameObject marcNeutral;
    public GameObject cristelNeutral;
    public GameObject darleneNeutral;
    public GameObject kuhNeutral;

    [Header("Backgrounds")]
    public GameObject rooftopBG;
    public GameObject classroomBG;
    public GameObject blackBG;

    void Start()
    {
        if (textBox != null) textBox.SetActive(true);
        if (nextButton != null) nextButton.SetActive(true);

        BuildDialogue();

        ShowLine();
    }

    void BuildDialogue()
    {
        // --- 🛡️ CRASH PREVENTION SYSTEM (FALLBACK VALUES) ---
        bool ravenRed = false;
        bool darleneRed = false;
        bool kuhRed = false;
        bool cristelRed = false;

        if (ChoiceManager.Instance != null)
        {
            ravenRed = ChoiceManager.Instance.ravenRed;
            darleneRed = ChoiceManager.Instance.darleneRed;
            kuhRed = ChoiceManager.Instance.kuhRed;
            cristelRed = ChoiceManager.Instance.cristelRed;
        }
        else
        {
            Debug.LogWarning("<color=yellow>[Epilogue Debugger]:</color> ChoiceManager.Instance is null (usually because you ran this scene directly). Falling back to PlayerPrefs so your game doesn't crash!");
            // Safely grab from PlayerPrefs if they were backed up, otherwise default to false
            ravenRed = PlayerPrefs.GetInt("Choice_RavenRed", 0) == 1;
            darleneRed = PlayerPrefs.GetInt("Choice_DarleneRed", 0) == 1;
            kuhRed = PlayerPrefs.GetInt("Choice_KuhRed", 0) == 1;
            cristelRed = PlayerPrefs.GetInt("Choice_CristelRed", 0) == 1;
        }

        // OPENING

        dialogue.Add(new DialogueLine(
            "Narration",
            "The bell rings while the sun is still setting, but the clock ticks its final second to 5:18.",
            null,
            rooftopBG
        ));

        // RAVEN FIRST HALF

        if (!ravenRed)
        {
            dialogue.Add(new DialogueLine(
                "Raven",
                "Guys… i think this time it’s for real.",
                ravenNeutral,
                rooftopBG
            ));
        }

        // DARLENE FIRST HALF

        if (!darleneRed)
        {
            dialogue.Add(new DialogueLine(
                "Darlene",
                "Is it finally real this time? Really?",
                darleneNeutral,
                rooftopBG
            ));
        }

        // KUH FIRST HALF

        if (!kuhRed)
        {
            dialogue.Add(new DialogueLine(
                "Kuh",
                "I think.. We escaped..",
                kuhNeutral,
                rooftopBG
            ));
        }

        // CRISTEL BLUE ONLY

        if (!cristelRed)
        {
            dialogue.Add(new DialogueLine(
                "Narration",
                "Cristel’s seat remains, her name etched on wood in memory.",
                null,
                classroomBG
            ));
        }

        // MARC

        dialogue.Add(new DialogueLine(
            "Marc",
            "Ayos din choice mo ah.",
            marcNeutral,
            rooftopBG
        ));

        dialogue.Add(new DialogueLine(
            "Narration",
            "The students disperse from the school, each going their separate ways.. The air feels more real this time.",
            null,
            rooftopBG
        ));

        dialogue.Add(new DialogueLine(
            "Narration",
            "Or is it?",
            null,
            blackBG
        ));

        // SECOND HALF — REDPILL ONLY

        if (ravenRed)
        {
            dialogue.Add(new DialogueLine(
                "Raven",
                "So.. you're the one who had been controlling my choices all along.",
                ravenNeutral,
                blackBG
            ));

            dialogue.Add(new DialogueLine(
                "Raven",
                "Can't say it's nice to meet you but, thanks for letting me out.",
                ravenNeutral,
                blackBG
            ));
        }

        if (darleneRed)
        {
            dialogue.Add(new DialogueLine(
                "Darlene",
                "I don't know who you are. I also don't know where I am.",
                darleneNeutral,
                blackBG
            ));

            dialogue.Add(new DialogueLine(
                "Darlene",
                "Pero… sana sa reality na to, buhay si Cristel..",
                darleneNeutral,
                blackBG
            ));
        }

        if (kuhRed)
        {
            dialogue.Add(new DialogueLine(
                "Kuh",
                "Soafer lala ha! Ano to? Nababaliw kana ba?",
                kuhNeutral,
                blackBG
            ));

            dialogue.Add(new DialogueLine(
                "Kuh",
                "Anyways! Kung nasan man ako! Sana hindi na ako piliting tumalon sa building!",
                kuhNeutral,
                blackBG
            ));
        }

        if (cristelRed)
        {
            dialogue.Add(new DialogueLine(
                "Cristel",
                "I trusted Marc… Looks like he's right.",
                cristelNeutral,
                blackBG
            ));

            dialogue.Add(new DialogueLine(
                "Cristel",
                "Feel ko, sa reality na to, buhay pa ako. Diba?",
                cristelNeutral,
                blackBG
            ));
        }

        // FINAL LINE

        dialogue.Add(new DialogueLine(
            "Marc",
            "Oh diba? Lupit! Sabi sayo ako main character eh! Sana nag enjoy ka sa game. Btw pasabi kay sir sana 100 yung grade.",
            marcNeutral,
            blackBG
        ));
    }

    void ShowLine()
    {
        if (currentLine >= dialogue.Count)
        {
            SceneManager.LoadScene("MAINMENU");
            return;
        }

        DialogueLine line = dialogue[currentLine];

        HideAllPortraits();
        HideAllBackgrounds();

        if (line.portrait != null)
            line.portrait.SetActive(true);

        if (line.background != null)
            line.background.SetActive(true);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    IEnumerator TypeLine(DialogueLine line)
    {
        isTyping = true;

        if (nextButton != null) nextButton.SetActive(false);

        charNameText.text = line.speaker;

        dialogueText.text = "";

        foreach (char c in line.text)
        {
            dialogueText.text += c;

            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;

        if (nextButton != null) nextButton.SetActive(true);
    }

    public void OnNextPressed()
    {
        
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);

            dialogueText.text = dialogue[currentLine].text;

            isTyping = false;

            if (nextButton != null) nextButton.SetActive(true);

            return;
        }

        currentLine++;

        ShowLine();
    }

    void HideAllPortraits()
    {
        if (ravenNeutral != null)
            ravenNeutral.SetActive(false);

        if (marcNeutral != null)
            marcNeutral.SetActive(false);

        if (cristelNeutral != null)
            cristelNeutral.SetActive(false);

        if (darleneNeutral != null)
            darleneNeutral.SetActive(false);

        if (kuhNeutral != null)
            kuhNeutral.SetActive(false);
    }

    void HideAllBackgrounds()
    {
        if (rooftopBG != null)
            rooftopBG.SetActive(false);

        if (classroomBG != null)
            classroomBG.SetActive(false);

        if (blackBG != null)
            blackBG.SetActive(false);
    }
}