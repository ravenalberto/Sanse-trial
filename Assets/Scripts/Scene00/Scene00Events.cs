using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




public class Scene00VN : MonoBehaviour
{

    private int currentLineIndex = 0; // Tracks dialogue progression


    public static bool isKuhOutsideSlots = false;
    private Dictionary<string, int> trustScores = new Dictionary<string, int>();

    public void AddTrust(string characterName, int amount)
    {
        if (!trustScores.ContainsKey(characterName))
        {
            trustScores[characterName] = 0;
        }
        trustScores[characterName] += amount;
        Debug.Log($"{characterName} trust changed by {amount}. Total: {trustScores[characterName]}");
    }


    private Coroutine typingCoroutine;

    private float skipTimer = 0f;
    public float skipDelay = 0.05f;

    private bool isLooping = false;


    public float textSpeed = 0.02f;
    public bool skipMode = false;


    private DialogueLine currentLine;

    private Queue<DialogueLine> tempQueue = new Queue<DialogueLine>();
    private bool usingTempQueue = false;

    [Header("Choices")]
    public GameObject foodChoicePanel;
    public GameObject dialogueChoicePanel;


    // FOOD (image buttons)
    public GameObject foodButtonA;
    public GameObject foodButtonB;
    public GameObject foodButtonC;


    // Notebook (image buttons)
    public GameObject closedNotebook;
    public GameObject openNotebook;
    // DIALOGUE (text buttons)

    public GameObject choiceTurnA;
    public GameObject choiceTurnB;

    public GameObject choiceTruthA;
    public GameObject choiceTruthB;

    public GameObject choiceNBA;
    public GameObject choiceNBB;



    private int choiceResult = 0;

    [Header("UI")]
    public GameObject textBox;
    public TMP_Text charNameText;
    public TMP_Text dialogueText;
    public GameObject nextButton;


    [Header("Backgrounds")]
    public GameObject BathroomBG;
    public GameObject ZoomComputerBG;
    public GameObject KeyboardBG;
    public GameObject SideviewcomputerBG;
    public GameObject hallwayBG;
    public GameObject complabBG;
    public GameObject uncleJohnsBG;
    public GameObject insideuncleJohnsBG;
    public GameObject schoolBG;
    public GameObject currentBG;
    public GameObject BGTransitionPanel; // For fade effect
    public GameObject FadeOutBG;

    [Header("Pause & HUD UI References")]
    [Tooltip("Drag your top-right HUD Play/Pause Button GameObject here.")]
    public GameObject hudPauseButtonObject;



    [Header("Portraits")]
    public GameObject cristelNeutral;
    public GameObject cristelFrown;
    public GameObject marcLaugh;
    public GameObject kuhNeutral;
    public GameObject kuhScared;
    public GameObject ravenNeutral;
    public GameObject darleneNeutral;
    public GameObject strangerNeutral;
    public GameObject strangerSmile;
    public GameObject darleneGlare;
    public GameObject marcChide;
    public GameObject marcNeutral;
    public GameObject cristelSmile; 

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip bgFadeSFX;   // whoosh sound
    public AudioClip glitchSFX;   // optional

    private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();
    private bool isTyping = false;
    private Coroutine bgFadeCoroutine;

    void Start()
    {
        // Set initial BG
        currentBG = schoolBG;
        schoolBG.SetActive(true);

        EnqueueDialogue();
        // --- SAVE PROGRESS RESTORER ---
        // Checks if the player loaded a save file corresponding to this exact scene
        // --- SAVE PROGRESS RESTORER ---
        if (PlayerPrefs.HasKey("SavedLineIndex") && PlayerPrefs.GetString("SavedScene") == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            int targetIndex = PlayerPrefs.GetInt("SavedLineIndex", 0);

            // Fast-forward (discard) lines from the queue until we reach the target line
            int discardedCount = 0;
            while (dialogueQueue.Count > 0 && discardedCount < targetIndex - 1)
            {
                DialogueLine line = dialogueQueue.Dequeue();

                // --- THE VISUAL RESTORATION FIX ---
                // If we skip past a background change command, run it instantly so the visuals catch up!
                if (line.speaker == "SYSTEM" && line.text.StartsWith("[BG_"))
                {
                    HandleSystemCommand(line.text);
                }

                discardedCount++;
            }

            // Set the active tracker index to match our loaded checkpoint
            currentLineIndex = discardedCount;

            // Clean up PlayerPrefs trackers so regular reloads don't loop
            PlayerPrefs.DeleteKey("SavedLineIndex");
            PlayerPrefs.DeleteKey("SavedScene");
            PlayerPrefs.Save();

            Debug.Log($"<color=green>Save restorer:</color> Skipped {discardedCount} lines. Backgrounds restored. Continuing at line {currentLineIndex + 1}.");
        }
        else
        {
            currentLineIndex = 0; // Fresh scene start
        }

        ShowNextLine();
    }

    // =========================
    // 🔥 DIALOGUE
    // =========================
    void EnqueueDialogue()
    {
        // 🏫 SCHOOL
        dialogueQueue.Enqueue(new DialogueLine("", "The bell rings."));
        dialogueQueue.Enqueue(new DialogueLine("", "The Angelus follows—"));
        dialogueQueue.Enqueue(new DialogueLine("", "a few seconds too late."));
        dialogueQueue.Enqueue(new DialogueLine("", "Cristel doesn’t notice."));
        dialogueQueue.Enqueue(new DialogueLine("", "Or maybe…"));
        dialogueQueue.Enqueue(new DialogueLine("", "she just chooses not to."));

        dialogueQueue.Enqueue(new DialogueLine("", "She walks beside her friends."));
        dialogueQueue.Enqueue(new DialogueLine("", "The usual circle."));


        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_HALLWAY]"));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "so guys, anong game want nyo?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "siguro barilan, yung may zombies.", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "haha i like that. Parang resident evil.", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "oo maganda yun!", marcLaugh));

        dialogueQueue.Enqueue(new DialogueLine("Raven", "kaso parang mahirap sya i code.", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "what if yung may story? Ano tawag sa mga ganun?", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "visual novel?", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "ay alam mo yung summertime saga?", marcLaugh));

        dialogueQueue.Enqueue(new DialogueLine("", "Raven and Kuh laugh awkwardly."));

        // 🏪 OUTSIDE STORE
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_UNCLE]"));

        dialogueQueue.Enqueue(new DialogueLine("", "They arrive at the nearby convenience store."));
        dialogueQueue.Enqueue(new DialogueLine("", "Their usual spot."));
        dialogueQueue.Enqueue(new DialogueLine("", "The air conditioning feels nicer against the heat."));

        dialogueQueue.Enqueue(new DialogueLine("Darlene", "ano yun?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "basta visual novel din yun.", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "ako yung main character", marcLaugh));

        dialogueQueue.Enqueue(new DialogueLine("Cristel", "lagi ka nalang feeling main character nakakabwisit na", cristelFrown));

        // 🏪 INSIDE STORE
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_INSIDE]"));

        dialogueQueue.Enqueue(new DialogueLine("Darlene", "kuya ano ba kasi yon—", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "guys baka gusto nyong bumili", kuhNeutral));

        dialogueQueue.Enqueue(new DialogueLine("Stranger", "ano ba naman to nakaharang sa daan."));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "hala sorry po", darleneNeutral));

        // 👁 HORROR BUILDUP
        dialogueQueue.Enqueue(new DialogueLine("", "Something feels… off."));
        dialogueQueue.Enqueue(new DialogueLine("", "Not loud."));
        dialogueQueue.Enqueue(new DialogueLine("", "Not obvious."));
        dialogueQueue.Enqueue(new DialogueLine("", "Just—wrong."));
        dialogueQueue.Enqueue(new DialogueLine("", "Someone stands outside the store.", strangerNeutral));
        dialogueQueue.Enqueue(new DialogueLine("", "Talking.", strangerNeutral));
        dialogueQueue.Enqueue(new DialogueLine("", "But there’s no one in front of them.", strangerNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Cristel turns to see…", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Wait… I know them..", cristelFrown));

        dialogueQueue.Enqueue(new DialogueLine("Kuh", "huh sino?", kuhNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Marc, Darlene, and Raven are still arguing."));

        dialogueQueue.Enqueue(new DialogueLine("Darlene", "hehe ok sige basta visual novel nalang.", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "tapos lagyan ko nalang minigames.", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "gusto ko pogi ako jan", marcLaugh));

        dialogueQueue.Enqueue(new DialogueLine("", "They sit near the window."));
        dialogueQueue.Enqueue(new DialogueLine("", "Cristel keeps looking outside."));
        dialogueQueue.Enqueue(new DialogueLine("", "Something still doesn’t sit right."));

        dialogueQueue.Enqueue(new DialogueLine("Kuh", "cristel? ok ka lang? parang nakakita ka ng multo.", kuhNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Cristel turns to Kuh, startled.", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "a-ah.. wala baka guni guni lang", cristelNeutral));

        dialogueQueue.Enqueue(new DialogueLine("Raven", "ang alin?", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "wala ka binili?", darleneNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Cristel looks at everyone already eating."));

        dialogueQueue.Enqueue(new DialogueLine("Cristel", "oh right sorry i forgot!", cristelNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "She heads to the shelves."));
        dialogueQueue.Enqueue(new DialogueLine("", "But her mind is somewhere else."));
        dialogueQueue.Enqueue(new DialogueLine("", "Still outside."));
        dialogueQueue.Enqueue(new DialogueLine("", "Still watching."));

        dialogueQueue.Enqueue(new DialogueLine("Marc", "ANG TAGAL NAMAN NG ISA JAN", marcLaugh));

        dialogueQueue.Enqueue(new DialogueLine("", "Cristel snaps out of it.", cristelFrown));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "SORRY HA", cristelFrown));

        dialogueQueue.Enqueue(new DialogueLine("Darlene", "omaygulay kalmahan nyo lang pinagtitinginan na kayo oh.", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "umay", kuhNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Kuh turns away."));
        dialogueQueue.Enqueue(new DialogueLine("", "Raven scrolls through her phone."));

        // 🍱 CHOICE 1
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE_FOOD]"));

        // AFTER CHOICE
        dialogueQueue.Enqueue(new DialogueLine("", "Finally they had finished eating."));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "tapos bukas mahaba vacant natin right?", darleneNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Raven", "comshop na", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "uuwi ako tas di na ako babalik", marcLaugh));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "tel, what if volleyball tayo sa vacant", kuhNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "As they prepare to head out, Cristel looks outside."));
        dialogueQueue.Enqueue(new DialogueLine("", "The figure is gone."));

        dialogueQueue.Enqueue(new DialogueLine("", "Cristel smiles."));
        dialogueQueue.Enqueue(new DialogueLine("Cristel", "taraahhh", cristelNeutral));

        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_UNCLE]"));
        dialogueQueue.Enqueue(new DialogueLine("", "They walk toward the campus."));

        dialogueQueue.Enqueue(new DialogueLine("Raven", "anjan na ba daw si sir?", ravenNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "wait chat ko sila", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("Marc", "dar pahipak", marcLaugh));
        dialogueQueue.Enqueue(new DialogueLine("Darlene", "kuya namannnn", darleneNeutral));

        dialogueQueue.Enqueue(new DialogueLine("", "Cristel can’t focus."));
        dialogueQueue.Enqueue(new DialogueLine("", "A familiar voice calls out."));
        dialogueQueue.Enqueue(new DialogueLine("Voice", "Cristel!"));

        dialogueQueue.Enqueue(new DialogueLine("", "She stops."));
        dialogueQueue.Enqueue(new DialogueLine("", "Turns around."));
        dialogueQueue.Enqueue(new DialogueLine("", "A familiar face waves."));
        dialogueQueue.Enqueue(new DialogueLine("", "An old friend."));

        dialogueQueue.Enqueue(new DialogueLine("Cristel", "Oh hey!", cristelNeutral));
        dialogueQueue.Enqueue(new DialogueLine("", "Something feels off."));
        dialogueQueue.Enqueue(new DialogueLine("Kuh", "tetel?", kuhNeutral));
        dialogueQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE_TURN]"));

        //COMLAB

    }

    // =========================
    // 🎮 NEXT BUTTON
    // =========================
    public void OnNextClick()
    {
        if (PauseMenu.IsPaused) return;
        if (foodChoicePanel.activeSelf || dialogueChoicePanel.activeSelf) return;


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

    void Update()
    {

        // 1. BLOCK SKIPPING IF PAUSED

        textSpeed = PauseMenu.GetTextDelay();
        if (PauseMenu.IsPaused) return;

        // 2. HIDE PAUSE BUTTON DURING CHOICES (To prevent pausing during decisions)
        if (hudPauseButtonObject != null)
        {
            bool isChoiceActive = foodChoicePanel.activeSelf || dialogueChoicePanel.activeSelf;
            hudPauseButtonObject.SetActive(!isChoiceActive);
        }

        skipMode = Input.GetKey(KeyCode.LeftControl);

        if (skipMode && !isTyping)
        {
            // STOP at choices
            if (!foodChoicePanel.activeSelf &&
                !dialogueChoicePanel.activeSelf)
            {
                ShowNextLine();
            }
        }
    }


    void SkipToNextChoice()
    {
        while (dialogueQueue.Count > 0)
        {
            DialogueLine next = dialogueQueue.Peek();

            if (next.speaker == "SYSTEM" &&
                (next.text.Contains("CHOICE")))
            {
                ShowNextLine();
                return;
            }

            dialogueQueue.Dequeue();
        }
    }

    void ShowNextLine()
    {
        // Changed condition: treat scene as done only when BOTH queues are empty.
        // This prevents early "Scene Done" when one queue (usually tempQueue)
        // still contains lines your logic intends to run.
        if (dialogueQueue.Count == 0 && tempQueue.Count == 0)
        {
            Debug.Log("Scene Done");
            return;
        }

        DialogueLine line;

        if (usingTempQueue && tempQueue.Count > 0)
        {
            line = tempQueue.Dequeue();

            if (tempQueue.Count == 0 && !(line.speaker == "SYSTEM"))
            {
                usingTempQueue = false;
            } // go back to main queue after
        }
        else
        {
            // If tempQueue is empty but dialogueQueue still has lines, continue.
            // (dialogueQueue.Dequeue() is safe because we check above that not both are empty)
            line = dialogueQueue.Dequeue();
        }

        // 🎬 SYSTEM COMMANDS
        if (line.speaker == "SYSTEM")
        {
            bool shouldPause = HandleSystemCommand(line.text);

            if (!shouldPause)
                ShowNextLine();

            return;
        }

        // --- 💾 SYNCHRONIZE ACTIVE DIALOGUE TRACKERS FOR THE PAUSE SYSTEM ---
        // This tracks which line we are on for saving, and takes a text preview for the slot labels!
        currentLineIndex++;
        PauseMenu.ActiveLineIndex = currentLineIndex;
        PauseMenu.ActiveDialogueText = line.text;

        // 🎭 PORTRAITS
        HideAllPortraits();
        if (line.portrait != null)
            line.portrait.SetActive(true);

        currentLine = line;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Runs your existing coroutine exactly as it is (no parameter changes!)
        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    void ProcessLine(DialogueLine line)
    {
        if (line == null)
            return;

        // SYSTEM handling
        if (line.speaker == "SYSTEM")
        {
            bool shouldPause = HandleSystemCommand(line.text);
            if (!shouldPause)
                ShowNextLine();
            return;
        }

        // portraits
        HideAllPortraits();
        if (line.portrait != null)
            line.portrait.SetActive(true);

        currentLine = line;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line));
    }

    public void OnNotebookClicked()
    {
        // 1. Visual Swap: Hide the closed book, show the open/bloody one
        closedNotebook.SetActive(false);
        openNotebook.SetActive(true);

        // 2. Play animation for the blood dripping
        Animator openAnim = openNotebook.GetComponent<Animator>();
        if (openAnim != null) openAnim.SetTrigger("OpenBlood");

        // 3. Resume the Dialogue
        if (nextButton != null) nextButton.SetActive(true);

        // Ensure we consume tempQueue entries (defensive).
        // Some flows put the notebook lines into tempQueue (SCENE_COMPLAB).
        usingTempQueue = true;

        // If there's something queued in tempQueue, process the next item immediately.
        if (tempQueue.Count > 0)
        {
            DialogueLine line = tempQueue.Dequeue();

            if (tempQueue.Count == 0 && !(line.speaker == "SYSTEM"))
                usingTempQueue = false;

            ProcessLine(line);
            return;
        }

        // fallback
        ShowNextLine();
    }



    // =========================
    // 🔊 SYSTEM COMMANDS
    // =========================
    bool HandleSystemCommand(string command)
    {
        if (command == "[BG_UNCLE]")
        {
            StartBGTransition(uncleJohnsBG);
            return false;
        }
        else if (command == "[BG_INSIDE]")
        {
            StartBGTransition(insideuncleJohnsBG);
            return false;
        }
        else if (command == "[BG_HALLWAY]")
        {
            StartBGTransition(hallwayBG);
            return false;
        }
        else if (command == "[BG_COMPUTER]")
        {
            StartBGTransition(ZoomComputerBG);
            return false;
        }
        else if (command == "[BG_SIDECOMPUTER]")
        {
            StartBGTransition(SideviewcomputerBG);
            return false;
        }
        else if (command == "[BG_KEYBOARD]")
        {
            StartBGTransition(KeyboardBG);
            return false;
        }

        else if (command == "[GLITCH]")
        {
            if (glitchSFX != null)
                sfxSource.PlayOneShot(glitchSFX);

            return false;
        }
        else if (command == "[CHOICE_FOOD]")
        {
            nextButton.SetActive(false);
            foodChoicePanel.SetActive(true);
            return true;
        }

        else if (command == "[CHOICE_TURN]")
        {
            HideAllChoices(); // Clear old buttons first
            nextButton.SetActive(false);
            dialogueChoicePanel.SetActive(true);
            choiceTurnA.SetActive(true);
            choiceTurnB.SetActive(true);
            return true;



        }

        else if (command == "[CHOICE_TRUTH]")
        {
            HideAllChoices(); // Clear old buttons first
            nextButton.SetActive(false);
            dialogueChoicePanel.SetActive(true);
            choiceTruthA.SetActive(true);
            choiceTruthB.SetActive(true);
            return true;
        }

        else if (command == "[LOOP_CONTINUE]")
        {
            tempQueue.Enqueue(new DialogueLine(
                "",
                "Finally they had finished eating."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Darlene",
                "tapos bukas mahaba vacant natin right?",
                darleneNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Raven",
                "comshop na",
                ravenNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Marc",
                "uuwi ako tas di na ako babalik",
                marcLaugh
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Kuh",
                "tel, what if volleyball tayo sa vacant",
                kuhNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Cristel",
                "ahh. sige…",
                cristelNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "As they were preparing to head out, Cristel looks outside."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "The figure was gone."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Again."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Cristel",
                "No… It's happening again…",
                cristelFrown
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "They walk toward the campus."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Voice",
                "cristel!"
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Closer."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Closer."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Cristel",
                "Sorry. Let's go.",
                cristelFrown
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Kuh",
                "What's that about?",
                kuhNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "SYSTEM",
                "[SCENE_COMPLAB]"
            ));

            return false;
        }

        else if (command == "[LOOP_START]")
        {
            tempQueue.Enqueue(new DialogueLine(
                "",
                "Cristel could see Dar looking at her empty table."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Darlene",
                "wala ka binili?",
                darleneNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Cristel looks at everyone who already bought lunch to eat."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "This feels like a deja vu..."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "...No."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Not deja vu."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "This already happened."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Exactly like this."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Cristel",
                "oh right sorry i forgot!",
                cristelNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "She heads to the shelf to pick what to buy."
            ));

            isLooping = true;

            tempQueue.Enqueue(new DialogueLine(
                "SYSTEM",
                "[CHOICE_FOOD]"
            ));

            return false;
        }

        else if (command == "[SCENE_COMPLAB]")
        {
            usingTempQueue = true;

            usingTempQueue = true;
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_COMPLAB]"));
            tempQueue.Enqueue(new DialogueLine("", "The classroom is busy as usual."));
            tempQueue.Enqueue(new DialogueLine("", "Cristel sits next to Darlene's desk."));
            tempQueue.Enqueue(new DialogueLine("Cristel", "Darlene! Can I borrow your notes?", cristelNeutral));
            tempQueue.Enqueue(new DialogueLine("Darlene", "Sure!", darleneNeutral));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[CHOICE_NOTEBOOK]")); // SHOW CLOSED
            tempQueue.Enqueue(new DialogueLine("Cristel", "thanks", cristelNeutral));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[OPEN_NOTEBOOK]")); // SWAP TO BLOODY
            tempQueue.Enqueue(new DialogueLine("", "As she opens it—there was red all over. Flesh dripping."));
            tempQueue.Enqueue(new DialogueLine("Cristel", "AH!", cristelFrown));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[HIDE_NOTEBOOK]"));
            tempQueue.Enqueue(new DialogueLine("Raven", "You good?", ravenNeutral));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[SHOW_NOTEBOOK_RESULT_CHOICES]"));
            return false;
        }

        else if (command == "[BG_COMPLAB]")
        {
            StartBGTransition(complabBG);
            return false;
        }

        else if (command == "[BG_SCREEN_GOES_BLACK]")
        {
            StartBGTransition(hallwayBG);
            return false;
        }

        else if (command == "[CHOICE_NOTEBOOK]")
        {
            closedNotebook.SetActive(true);
            openNotebook.SetActive(false);
            return false;
        }

        else if (command == "[OPEN_NOTEBOOK]")
        {
            closedNotebook.SetActive(false);
            openNotebook.SetActive(true);
            Animator anim = openNotebook.GetComponent<Animator>();
            if (anim != null) anim.SetTrigger("OpenBlood");
            return false;
        }

        else if (command == "[HIDE_NOTEBOOK]")
        {
            closedNotebook.SetActive(false);
            openNotebook.SetActive(false);
            return false;
        }

        else if (command == "[SHOW_NOTEBOOK_RESULT_CHOICES]")
        {
            nextButton.SetActive(false);
            dialogueChoicePanel.SetActive(true);
            choiceNBA.SetActive(true);
            choiceNBB.SetActive(true);
            return true; // PAUSE
        }



        else if (command == "[GOTO_COMPUTER_TASK]")
        {
            tempQueue.Enqueue(new DialogueLine("Classmate", "Dar halika dito saglit"));
            tempQueue.Enqueue(new DialogueLine("Cristel", "Sige na go na Dar okay lang ako. Pramis.", cristelNeutral));
            tempQueue.Enqueue(new DialogueLine("Darlene", "Okay sige, andito lang kami ha?", darleneNeutral));
            tempQueue.Enqueue(new DialogueLine("Cristel", "Opo thank you po.", cristelNeutral));

            tempQueue.Enqueue(new DialogueLine("Cristel", "I should probably just do something else.."));
            tempQueue.Enqueue(new DialogueLine("", "She turns to look at Raven who is typing some code on our project."));
            tempQueue.Enqueue(new DialogueLine("Cristel", "Hey Raven can I help with anything?", cristelNeutral));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_COMPUTER]"));
            tempQueue.Enqueue(new DialogueLine("Raven", "Huh? Yeah sure I guess you can work on this script..", ravenNeutral));
            tempQueue.Enqueue(new DialogueLine("", "Raven explains some tasks..."));

            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_SIDECOMPUTER]"));

            tempQueue.Enqueue(new DialogueLine("Raven", "Alright, if you have questions I'll be with Dar.", ravenNeutral));
            tempQueue.Enqueue(new DialogueLine("Cristel", "Okay.. thank you raven."));


            tempQueue.Enqueue(new DialogueLine("Cristel", "Hmm what should go here."));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_KEYBOARD]"));
            tempQueue.Enqueue(new DialogueLine("", "But suddenly the keyboard won't type."));
            tempQueue.Enqueue(new DialogueLine("Cristel", "huh..?", cristelFrown));

            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_COMPLAB]"));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[SCREEN_GOES_BLACK]"));
            tempQueue.Enqueue(new DialogueLine("Cristel", "Crap, did it even save? Did I kick the plug?"));
            tempQueue.Enqueue(new DialogueLine("", "Something feels weird. Like someone is watching her through the screen."));
            tempQueue.Enqueue(new DialogueLine("Cristel", "Who.."));

            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[SHOW_BLOODY_RAVEN_JUMPSCARE]"));
            tempQueue.Enqueue(new DialogueLine("Cristel", "Ah!", cristelFrown));
            tempQueue.Enqueue(new DialogueLine("", "She drops the mouse and stands up shaking."));

            tempQueue.Enqueue(new DialogueLine("Kuh", "Tel!! wha happened?!", kuhNeutral));
            tempQueue.Enqueue(new DialogueLine("", "She looks back at the PC which is perfectly closed. Raven is fine."));

            tempQueue.Enqueue(new DialogueLine("Cristel", "I need to go to the bathroom."));
            tempQueue.Enqueue(new DialogueLine("Darlene", "Wait cristel let us go with you!"));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[SCENE_BATHROOM]"));
            return false;
        }



        else if (command == "[SCENE_BATHROOM]")
        {
            tempQueue.Enqueue(new DialogueLine("Cristel", "what the heck is wrong with me am i being haunted by ghosts?"));
            tempQueue.Enqueue(new DialogueLine("Cristel", "He’s right.. Its nothing this is nothing.."));
            tempQueue.Enqueue(new DialogueLine("Cristel", "I’m fine", cristelSmile));

            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[MIRROR_SHIFT_MARC]"));
            tempQueue.Enqueue(new DialogueLine("Marc", "I’m fine~ I’m fine~ You done lying to yourself?", marcNeutral));
            tempQueue.Enqueue(new DialogueLine("Cristel", "WHa–"));

            tempQueue.Enqueue(new DialogueLine("", "The mirror is back to normal."));
            tempQueue.Enqueue(new DialogueLine("Cristel", "I hate this I hate this i hate this"));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[BG_HALLWAY_SUNSET]"));
            tempQueue.Enqueue(new DialogueLine("Kuh", "Cristel", kuhNeutral));
            tempQueue.Enqueue(new DialogueLine("Cristel", "Oh thank God kuh! Where are they?"));

            tempQueue.Enqueue(new DialogueLine("Kuh", "They're inside.", kuhNeutral));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[SCENE_DOOR_OVERHEAR]"));
            return false;
        }

        else if (command == "[SCENE_DOOR_OVERHEAR]")
        {
            tempQueue.Enqueue(new DialogueLine("Darlene", "Need lang naman natin sya intindihin Kuya–"));
            tempQueue.Enqueue(new DialogueLine("Marc", "Oh talaga? So tayo nalang lagi iintindi sa kanya? Pano tayo? Pano ako? Lagi nalang ganyan?"));
            tempQueue.Enqueue(new DialogueLine("Darlene", "Kuya naman please…"));
            tempQueue.Enqueue(new DialogueLine("Marc", "Ayaw mo maniwala sakin kausapin mo sya–"));

            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[DOOR_OPEN_SFX]"));
            tempQueue.Enqueue(new DialogueLine("Kuh", "Ano nanaman nangayyari dito jusko", kuhNeutral));
            tempQueue.Enqueue(new DialogueLine("Marc", "Oh ayan speak of the devil, buti dumating pa yan", marcNeutral));
            tempQueue.Enqueue(new DialogueLine("Cristel", "Bakit ano bang meron.", cristelNeutral));
            tempQueue.Enqueue(new DialogueLine("Darlene", "Cristel, saan ka galing?", darleneNeutral));
            tempQueue.Enqueue(new DialogueLine("Cristel", "Nagbanyo lang saglit. Okay lang ba si Marc."));
            tempQueue.Enqueue(new DialogueLine("Darlene", "Don’t mind him nalang.. May real concern tayo."));

            tempQueue.Enqueue(new DialogueLine("Raven", "Akala ko ung orasan lang yung stuck, hindi talaga nababa yung araw."));
            tempQueue.Enqueue(new DialogueLine("Cristel", "What do you mean?"));
            tempQueue.Enqueue(new DialogueLine("Raven", "Check mo phone. Ano nakalagay na time?"));
            tempQueue.Enqueue(new DialogueLine("Cristel", "5:17pm"));
            tempQueue.Enqueue(new DialogueLine("Raven", "Yah, isang oras ng 5:17."));

            tempQueue.Enqueue(new DialogueLine("Kuh", "Sa totoo lang uwing uwi na ako pero ikaw lang din talaga inaantay namin at isang oras na ang nakalipas."));
            tempQueue.Enqueue(new DialogueLine("Darlene", "hindi mo ba naramdaman? Isang oras ka wala.."));
            tempQueue.Enqueue(new DialogueLine("Cristel", "hindi eh…"));

            tempQueue.Enqueue(new DialogueLine("Raven", "guys... look at the board."));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[SHOW_BOARD_WRITING]"));
            tempQueue.Enqueue(new DialogueLine("Board", "Raven is always with Darlene."));
            tempQueue.Enqueue(new DialogueLine("Board", "Marc is always next to Cristel."));
            tempQueue.Enqueue(new DialogueLine("Board", "Kuh is everywhere."));

            tempQueue.Enqueue(new DialogueLine("Marc", "Wow. Ang linaw. Zero explanation.", marcNeutral));
            tempQueue.Enqueue(new DialogueLine("Darlene", "wow kuya buhay ka pa pala"));
            tempQueue.Enqueue(new DialogueLine("Cristel", "No… it does."));
            tempQueue.Enqueue(new DialogueLine("Cristel", "(This is easy. I know them. I know where everyone belongs. Right?)"));


            tempQueue.Enqueue(new DialogueLine("", "Kindly place the chairs on the white square blanks based on the clue given by dragging and dropping the chairs."));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[START_CHAIR_PUZZLE]"));
            return false;
        }
        else if (command == "[START_CHAIR_PUZZLE]")
        {
            // Transition to the actual puzzle scene
            SceneManager.LoadScene("ChairPuzzleScene");
            return true; // Return true to signal the dialogue system to wait/stop
        }

        else if (command == "[FINISH_CHAIR_PUZZLE]")
        {
            // Logic for Kuh placement based on global variables or puzzle manager
            if (isKuhOutsideSlots)
            {
                AddTrust("Kuh", 10);
                tempQueue.Enqueue(new DialogueLine("Kuh", "Good. You remembered.", kuhNeutral));
            }
            else
            {
                AddTrust("Kuh", -10);
                tempQueue.Enqueue(new DialogueLine("Kuh", "Bakit mo ako sinasama dyan?", kuhNeutral));
            }

            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[SCENE_ANGELUS]"));
            return false;
        }

        else if (command == "[SCENE_ANGELUS]")
        {
            tempQueue.Enqueue(new DialogueLine("Intercom", "Angelus Domini nuntiavit Mariae..."));
            tempQueue.Enqueue(new DialogueLine("Darlene", "...Ang late na.", darleneNeutral));
            tempQueue.Enqueue(new DialogueLine("Kuh", "...Pero ang aga naman for prayer?", kuhNeutral));
            tempQueue.Enqueue(new DialogueLine("Intercom", "...et concepit de Spiritu—"));
            tempQueue.Enqueue(new DialogueLine("Intercom", "Students."));
            tempQueue.Enqueue(new DialogueLine("Intercom", "5:17 PM."));
            tempQueue.Enqueue(new DialogueLine("SYSTEM", "[FADEOUT_END]"));
            return false;
        }

        // Ensure a default return so all code paths return a value.
        return false;
    }



    void HideAllChoices()
    {
        if (dialogueChoicePanel != null) dialogueChoicePanel.SetActive(false);
        if (foodChoicePanel != null) foodChoicePanel.SetActive(false);

        if (choiceTurnA != null) choiceTurnA.SetActive(false);
        if (choiceTurnB != null) choiceTurnB.SetActive(false);
        if (choiceTruthA != null) choiceTruthA.SetActive(false);
        if (choiceTruthB != null) choiceTruthB.SetActive(false);
        if (choiceNBA != null) choiceNBA.SetActive(false);
        if (choiceNBB != null) choiceNBB.SetActive(false);
    }


    void StartBGTransition(GameObject newBG)
    {
        if (newBG == currentBG) return;

        if (bgFadeCoroutine != null)
            StopCoroutine(bgFadeCoroutine);

        bgFadeCoroutine = StartCoroutine(FadeBackground(newBG));
    }



    public void ChoiceNotebookA() // Player chose: There's blood
    {
        dialogueChoicePanel.SetActive(false);
        usingTempQueue = true;
        tempQueue.Clear();
        tempQueue.Enqueue(new DialogueLine("", "Darlene takes the notebook on the floor and flips the page."));
        tempQueue.Enqueue(new DialogueLine("Kuh", "hindi ka naman nilalagnat?", kuhNeutral));
        tempQueue.Enqueue(new DialogueLine("Marc", "Ganyan talaga yan", marcChide));
        tempQueue.Enqueue(new DialogueLine("Darlene", "Kuya..", darleneGlare));
        tempQueue.Enqueue(new DialogueLine("SYSTEM", "[GOTO_COMPUTER_TASK]"));
        ShowNextLine();
    }

    public void ChoiceNotebookB() // Player chose: It's nothing
    {
        dialogueChoicePanel.SetActive(false);
        usingTempQueue = true;
        tempQueue.Clear();
        tempQueue.Enqueue(new DialogueLine("Darlene", "Sure ka? Nanginginig ka eh.", darleneNeutral));
        tempQueue.Enqueue(new DialogueLine("SYSTEM", "[GOTO_COMPUTER_TASK]"));
        ShowNextLine();
    }
    public void ChoiceA()
    {
        foodChoicePanel.SetActive(false);

        usingTempQueue = true;
        tempQueue.Clear();

        // NORMAL TIMELINE
        if (!isLooping)
        {
            tempQueue.Enqueue(new DialogueLine(
                "Cristel",
                "I guess I'm a bit more thirsty than hungry..",
                cristelNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Cristel walks over to the counter to pay."
            ));
        }
        else
        {
            // 👁 LOOP VERSION
            tempQueue.Enqueue(new DialogueLine(
                "",
                "The bottle feels familiar."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Too familiar."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Cristel",
                "...Didn't I already pick this before?",
                cristelFrown
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "The fluorescent lights suddenly feel too bright."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Marc",
                "ANG TAGAL NAMAN NG ISA JAN",
                marcLaugh
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Cristel breaks out of her trance."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "She chose not to reply."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Darlene",
                "omaygulay kalmahan nyo lang pinagtitinginan na kayo oh.",
                darleneNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Kuh",
                "umay",
                kuhNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Kuh already had her face turned away from people."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Raven is just scrolling through her phone."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "SYSTEM",
                "[LOOP_CONTINUE]"
            ));

            // 🔥 EXIT LOOP
            isLooping = false;
        }

        ShowNextLine();
    }

    public void ChoiceB()
    {
        foodChoicePanel.SetActive(false);

        usingTempQueue = true;
        tempQueue.Clear();

        if (!isLooping)
        {
            // NORMAL VERSION
            tempQueue.Enqueue(new DialogueLine(
                "Cristel",
                "Not too hungry, but I’m craving these..",
                cristelNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Cristel walks over to the counter to pay."
            ));
        }
        else
        {
            // LOOP VERSION
            tempQueue.Enqueue(new DialogueLine(
                "",
                "The plastic crinkles in her hand."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "The sound makes Cristel freeze."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Cristel",
                "...I heard this already.",
                cristelFrown
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "For a second, it feels like she already lived this exact moment."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Same shelf."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Same lighting."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Same people."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Marc",
                "ANG TAGAL NAMAN NG ISA JAN",
                marcLaugh
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Cristel breaks out of her trance."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "She chose not to reply."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Darlene",
                "omaygulay kalmahan nyo lang pinagtitinginan na kayo oh.",
                darleneNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Kuh",
                "umay",
                kuhNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Kuh already had her face turned away from people."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Raven is just scrolling through her phone."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "SYSTEM",
                "[LOOP_CONTINUE]"
            ));

            isLooping = false;
        }

        ShowNextLine();
    }

    public void ChoiceC()
    {
        foodChoicePanel.SetActive(false);

        usingTempQueue = true;
        tempQueue.Clear();

        if (!isLooping)
        {
            // NORMAL VERSION
            tempQueue.Enqueue(new DialogueLine(
                "Cristel",
                "Not too hungry, but I’m craving these..",
                cristelNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Cristel walks over to the counter to pay."
            ));
        }

        else
        {
            tempQueue.Enqueue(new DialogueLine(
                "",
                "The smell hits her immediately."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Oil."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Spices."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "And something else."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Cristel",
                "...Why do I remember this?",
                cristelFrown
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Her stomach suddenly twists."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Like her body already knew what was about to happen."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "The cashier smiles."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "For a second, Cristel feels terrified."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Marc",
                "ANG TAGAL NAMAN NG ISA JAN",
                marcLaugh
            ));

            tempQueue.Enqueue(new DialogueLine(
                "",
                "Cristel snaps out of it."
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Darlene",
                "omaygulay kalmahan nyo lang pinagtitinginan na kayo oh.",
                darleneNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "Kuh",
                "umay",
                kuhNeutral
            ));

            tempQueue.Enqueue(new DialogueLine(
                "SYSTEM",
                "[LOOP_CONTINUE]"
            ));

            isLooping = false;
        }

        ShowNextLine();
    }

    public void ChoiceTurnA() // canon
    {
        skipMode = false;
        dialogueChoicePanel.SetActive(false);

        usingTempQueue = true;
        tempQueue.Clear();

        tempQueue.Enqueue(new DialogueLine(
            "",
            "Cristel walks towards them."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Old Friend",
            "hi, kamusta ka?", strangerSmile
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "Their smile seems off. Has she really forgiven them? Or is there still resentment?"
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Cristel",
            "eto, okay lang naman..",
            cristelNeutral
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "Cristel wants to say something else but she couldn't get it out."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Old Friend",
            "mabuti naman. Miss kana namin.", strangerSmile
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "But everything feels quiet."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "Then she hears Kuh."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Kuh",
            "cristel?",
            kuhNeutral
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "She could hear Kuh coming closer."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "And the loud sound of an approaching vehicle."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Old Friend",
            "Sino yun?", strangerNeutral
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Kuh",
            "cristel sino—",
            kuhScared
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "Then there was a loud crash."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "And everyone was screaming."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "Cristel turns around but before she could see the sight, the sound disappears."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "Then—"
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "red."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "SYSTEM",
            "[GLITCH]"
        ));

        tempQueue.Enqueue(new DialogueLine(
            "SYSTEM",
            "[BG_INSIDE]"
        ));
        tempQueue.Enqueue(new DialogueLine(
            "",
            "The world snaps back into place."
        ));


        tempQueue.Enqueue(new DialogueLine(
            "",
            "Too fast."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "Like nothing ever happened."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "Like it was never allowed to happen."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Kuh",
            "cristel? ok ka lang? parang naka kita ka ng multo.",
            kuhNeutral
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "Cristel turns to Kuh, surprised, shaking."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "SYSTEM",
            "[CHOICE_TRUTH]"
        ));

        ShowNextLine();
    }

    public void ChoiceTurnB()
    {
        skipMode = false;
        dialogueChoicePanel.SetActive(false);

        usingTempQueue = true;
        tempQueue.Clear();

        tempQueue.Enqueue(new DialogueLine("", "Cristel freezes."));
        tempQueue.Enqueue(new DialogueLine("", "She doesn't turn back."));
        tempQueue.Enqueue(new DialogueLine("", "She already knows what happens if she does."));

        tempQueue.Enqueue(new DialogueLine("", "Closer."));
        tempQueue.Enqueue(new DialogueLine("", "Closer."));

        tempQueue.Enqueue(new DialogueLine("Cristel", "Sorry.", cristelNeutral));
        tempQueue.Enqueue(new DialogueLine("Kuh", "what's that about?", kuhNeutral));

        // continue main flow directly
        ShowNextLine();
    }


    public void ChoiceTruthA()
    {
        skipMode = false;
        dialogueChoicePanel.SetActive(false);

        usingTempQueue = true;
        tempQueue.Clear();

        tempQueue.Enqueue(new DialogueLine(
            "Kuh",
            "ako??? Nabangga??",
            kuhScared
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "Cristel nods."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Cristel",
            "oo, sorry baka kulang lang ako sa tulog huhu. Kuh mag ingat ka lagi please",
            cristelFrown
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Marc",
            "baka ikaw kailangan mag ingat di ko alam san ka nauntog",
            marcLaugh
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Darlene",
            "kuya namannnn",
            darleneNeutral
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Raven",
            "ayos yan idea for visual novel",
            ravenNeutral
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Darlene",
            "baka need mo magpahinga tel, pero if may pinagdadaanan ka you always know we're here para sayo..",
            darleneNeutral
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Cristel",
            "thank you guys…",
        cristelNeutral
        ));

        tempQueue.Enqueue(new DialogueLine(
            "SYSTEM",
            "[LOOP_START]"
        ));

        ShowNextLine();
    }


    public void ChoiceTruthB()
    {
        isLooping = true;
        skipMode = false;
        dialogueChoicePanel.SetActive(false);

        usingTempQueue = true;
        tempQueue.Clear();

        tempQueue.Enqueue(new DialogueLine(
            "Darlene",
            "kung ano man yan you know you can tell us okay?",
            darleneNeutral
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Marc",
            "ang weird mo today ah",
            marcLaugh
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "Cristel could see Darlene looking at her empty table."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Darlene",
            "wala ka binili?",
            darleneNeutral
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "Cristel looks at everyone who already bought lunch to eat."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "This feels like a deja vu..."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "...No."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "Not deja vu."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "This already happened."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "Exactly like this."
        ));

        tempQueue.Enqueue(new DialogueLine(
            "Cristel",
            "oh right sorry i forgot!",
            cristelNeutral
        ));

        tempQueue.Enqueue(new DialogueLine(
            "",
            "She heads to the shelf to pick what to buy."
        ));

        // 🔁 LOOP
        tempQueue.Enqueue(new DialogueLine(
            "SYSTEM",
            "[CHOICE_FOOD]"
        ));

        ShowNextLine();
    }



    // =========================
    // 🎬 FADE + SOUND
    // =========================
    IEnumerator FadeBackground(GameObject newBG)
    {
        float duration = 0.5f;

        // 🔊 PLAY SOUND HERE
        if (bgFadeSFX != null)
            sfxSource.PlayOneShot(bgFadeSFX);

        // Fade OUT
        if (currentBG != null)
        {
            CanvasGroup oldCG = currentBG.GetComponent<CanvasGroup>();
            float t = 0;

            while (t < duration)
            {
                t += Time.deltaTime;
                oldCG.alpha = 1 - (t / duration);
                yield return null;
            }

            oldCG.alpha = 0;
            currentBG.SetActive(false);
        }

        // Switch
        newBG.SetActive(true);
        CanvasGroup newCG = newBG.GetComponent<CanvasGroup>();
        newCG.alpha = 0;

        // Fade IN
        float t2 = 0;
        while (t2 < duration)
        {
            t2 += Time.deltaTime;
            newCG.alpha = t2 / duration;
            yield return null;
        }

        newCG.alpha = 1;
        currentBG = newBG;
    }

    // =========================
    // 📝 TEXT
    // =========================
    IEnumerator TypeLine(DialogueLine line)
    {
        isTyping = true;
        nextButton.SetActive(false);

        textBox.SetActive(true);
        charNameText.text = line.speaker;
        dialogueText.text = "";

        foreach (char c in line.text)
        {
            dialogueText.text += c;

            yield return new WaitForSeconds(
                skipMode ? 0.0001f : textSpeed
            );
        }

        isTyping = false;
        nextButton.SetActive(true);
    }

    // =========================
    // 🎭 PORTRAITS
    // =========================
    void HideAllPortraits()
    {
        cristelNeutral.SetActive(false);
        cristelFrown.SetActive(false);
        cristelSmile.SetActive(false);
        marcLaugh.SetActive(false);
        marcNeutral.SetActive(false);
        marcChide.SetActive(false);
        kuhNeutral.SetActive(false);
        kuhScared.SetActive(false);
        ravenNeutral.SetActive(false);
        darleneNeutral.SetActive(false);
        darleneGlare.SetActive(false);
        strangerNeutral.SetActive(false);
        strangerSmile.SetActive(false);
    }
}

// =========================
// DATA CLASS
// =========================
[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string text;
    public GameObject portrait;

    public DialogueLine(string speaker, string text, GameObject portrait = null)
    {
        this.speaker = speaker;
        this.text = text;
        this.portrait = portrait;
    }
}