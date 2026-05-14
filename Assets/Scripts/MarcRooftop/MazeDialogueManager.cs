using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeDialogueManager : MonoBehaviour
{
    public SimpleDialogue dialogueSystem;
    public GameObject dialogueChoicePanel; // Your Choice Prefab


     private int doorsOpened = 0;
    private bool isDialogueActive = false;
    private bool isEventActive = false;

    private Dictionary<int, List<DialogueLine>> storyMilestones = new Dictionary<int, List<DialogueLine>>();

    void Start()
    {
        SetupMazeConversations();
    }

    void SetupMazeConversations()
    {
        // Milestone: First Door
        storyMilestones[1] = new List<DialogueLine> {
            new DialogueLine("Darlene", "Eto na... Unang pinto. Please, sana hindi 'to loop."),
            new DialogueLine("Marc", "Relax, Dar. Basta magkakasama tayo, walang maliligaw."),
            new DialogueLine("Raven", "Statistically speaking, chances of looping increase the deeper we go. Focus lang.")
        };

        // Milestone: Fifth Door (A Choice Point)
        storyMilestones[5] = new List<DialogueLine> {
            new DialogueLine("Tetel", "Wait... parang narinig ko na yung tunog na 'to kanina."),
            new DialogueLine("Darlene", "(Nararamdaman ko rin. Parang pinaglalaruan tayo ng hallway.)"),
            new DialogueLine("SYSTEM", "[CHOICE_DIRECTION]")
        };
    }


    public void OnDoorOpened()
    {
        if (isEventActive) return;
        doorsOpened++;

        if (doorsOpened == 1)
        {
            dialogueSystem.ShowDialogue("Darlene", "Bukas na yung pinto... pero parang mas dumidilim sa loob.");
        }
        else if (doorsOpened == 5)
        {
            TriggerChoiceEvent();
        }
        else
        {
            // Random ambient dialogue for other doors
            string[] clues = { "May narinig ba kayo?", "Marc, 'wag ka masyadong malayo.", "Raven, what's the logic here?" };
            dialogueSystem.ShowDialogue("Darlene", clues[Random.Range(0, clues.Length)]);
        }
    }

    void TriggerChoiceEvent()
    {
        isEventActive = true;
        dialogueSystem.ShowDialogue("Raven", "Wait. Dalawa yung daan. Darlene, ikaw ang mag decide.");

        // We wait for the dialogue to finish before showing choices
        StartCoroutine(ShowChoicesAfterDialogue());
    }

    IEnumerator ShowChoicesAfterDialogue()
    {
        // Wait until the dialogue UI is ready for input (Next button visible)
        yield return new WaitUntil(() => dialogueSystem.nextButton.activeSelf);

        // Hide next button and show choices instead
        dialogueSystem.nextButton.SetActive(false);
        dialogueChoicePanel.SetActive(true);
    }
    public void OnChoiceSelected(int choiceIndex)
    {
        dialogueChoicePanel.SetActive(false);
        isEventActive = false;

        if (choiceIndex == 0) // e.g., Choice A
            dialogueSystem.ShowDialogue("Marc", "Sige, tiwala kami sayo Dar. Kanan tayo.");
        else
            dialogueSystem.ShowDialogue("Raven", "Logical. Mas malamig yung hangin sa kaliwa.");
    }




IEnumerator PlayConversation(List<DialogueLine> lines)
    {
        isDialogueActive = true;
        foreach (var line in lines)
        {
            if (line.speaker == "SYSTEM" && line.text == "[CHOICE_DIRECTION]")
            {
                ShowMazeChoices();
                yield break;
            }

            dialogueSystem.ShowDialogue(line.speaker, line.text);
            yield return new WaitUntil(() => !dialogueSystem.vnUI.activeSelf);
        }
        isDialogueActive = false;
    }

    string GetRandomClue()
    {
        string[] clues = {
            "Malamig yung hangin dito... galing ba 'to sa labas?",
            "Marc, 'wag ka ngang malayo sa amin!",
            "Raven, tignan mo 'tong bitak sa pader. Parang bago.",
            "I need to find that rooftop. Malapit na tayo, nararamdaman ko."
        };
        return clues[Random.Range(0, clues.Length)];
    }

    void ShowMazeChoices()
    {
        dialogueChoicePanel.SetActive(true);
        // Choice logic handled by button clicks
    }
}