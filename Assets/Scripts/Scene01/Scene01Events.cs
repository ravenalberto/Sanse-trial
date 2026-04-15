using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Scene01Events : MonoBehaviour
{
    [Header("UI References")]
    public GameObject fadeScreenIn;
    public GameObject fadeOut;
    public GameObject textBox;
    public GameObject mainTextObject;
    public GameObject nextButton;
    public TMP_Text charNameText;

    [Header("Characters")]
    public GameObject charKuh;
    public GameObject charCristel;
    public GameObject charDarlene;
    public GameObject charMarc;
    public GameObject charRaven;

    [Header("Settings")]
    [SerializeField] private int eventPos = 0;
    private int currentTextLength;
    private bool isTyping = false;

    // A simple class to hold dialogue data
    [System.Serializable]
    public class DialogueLine
    {
        public string name;
        public string text;
        public bool showKuh;
        public bool showCristel;
        public bool triggerFadeOut; // Used for scene transitions
    }

    public List<DialogueLine> dialogueList = new List<DialogueLine>();

    void Start()
    {
        // Initialize Scene
        mainTextObject.SetActive(false);
        nextButton.SetActive(false);
        textBox.SetActive(false);

        // Build the dialogue list based on your script
        SetupDialogue();

        StartCoroutine(OpeningSequence());
    }

    void Update()
    {
        // Syncing with your existing TextCreator logic
        if (TextCreator.charCount >= currentTextLength && isTyping)
        {
            isTyping = false;
            nextButton.SetActive(true);
        }
    }

    void SetupDialogue()
    {
        // LOBBY
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "Psst—Tetel!", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Cristel", text = "Ah! Kuh, andiyan ka pala!", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Cristel", text = "San ka na naman galing? Nasa room na sila.", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "Dapat nandun na nga eh… pero walang sumasagot sa taas.", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Cristel", text = "Ha? Baka tulog lang sila?", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "Hindi eh… kumatok ako. Ilang beses. …Tahimik talaga. As in wala.", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "Tsaka may narinig ako… parang may naglalakad sa hallway.", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "…pero pag tingin ko—wala naman.", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Cristel", text = "Uy stop… ang creepy mo na.", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "Seryoso ako.", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Cristel", text = "Wait… bakit parang ang dilim bigla?", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "…Napansin mo rin?", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Cristel", text = "Wait—narinig mo ‘yon?", showKuh = true, showCristel = true });
        dialogueList.Add(new DialogueLine { name = "Kuh", text = "Tetel… bilis. Tara na.", showKuh = true, showCristel = true, triggerFadeOut = true });

        // Add more for Scene 2 and Scene 3 here...
    }

    IEnumerator OpeningSequence()
    {
        yield return new WaitForSeconds(2);
        fadeScreenIn.SetActive(false);
        charCristel.SetActive(true);
        yield return new WaitForSeconds(1);

        // First Line (Automatic)
        PlayDialogue(0);
    }

    public void NextButton()
    {
        eventPos++;
        if (eventPos < dialogueList.Count)
        {
            if (dialogueList[eventPos].triggerFadeOut)
            {
                StartCoroutine(TransitionToNextScene());
            }
            else
            {
                PlayDialogue(eventPos);
            }
        }
        else
        {
            // End of dialogue logic
            SceneManager.LoadScene(1);
        }
    }

    void PlayDialogue(int index)
    {
        nextButton.SetActive(false);
        textBox.SetActive(true);
        mainTextObject.SetActive(true);

        DialogueLine line = dialogueList[index];

        charNameText.text = line.name;
        charKuh.SetActive(line.showKuh);
        charCristel.SetActive(line.showCristel);

        // Send to your TextCreator script
        TextCreator.fullText = line.text;
        TextCreator.charCount = 0;
        TextCreator.runTextPrint = true;

        currentTextLength = line.text.Length;
        isTyping = true;
    }

    IEnumerator TransitionToNextScene()
    {
        nextButton.SetActive(false);
        fadeOut.SetActive(true);
        yield return new WaitForSeconds(2);
        // If you want to load a new Unity Scene:
        SceneManager.LoadScene(1);
    }
}