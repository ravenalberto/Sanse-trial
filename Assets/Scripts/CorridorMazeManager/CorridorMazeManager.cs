using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CorridorMazeManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public GameObject transitionPanel;
    public GameObject[] chanceIndicators; // Assign 3 UI dots/icons here

    [Header("Maze Configuration")]
    private int currentStage = 0;
    private int chances = 3;

    // Pattern: R -> R -> L -> R -> R -> L -> R
    private readonly bool[] correctSequence = { true, true, false, true, true, false, true };

    [Header("Content")]
    private readonly string[] environmentalClues = {
        "The shadows of the lockers lean heavily toward the right wall.",
        "A cold, unnerving breeze blows from the right side of the hallway.",
        "Raven's phone glitches; a static-filled compass briefly points Left.",
        "The floor tiles are cracked, forming a jagged arrow pointing Right.",
        "The flickering light overhead pulses rhythmically toward the Right.",
        "A faint, distorted whisper from the left says: 'Don't follow them.'",
        "The light from the end of the hall reflects off the brass on the Right."
    };

    private readonly string[] characterDialogue = {
        "Cristel: It's definitely right! I can feel it.\nRaven: The air feels stagnant that way...",
        "Marc: Right again. It's a straight line, right?\nRaven: Straight lines are just illusions in this place.",
        "Cristel: Let's keep going right!\n<color=#A5A5A5>Raven: WAIT! Look at the shadows... they switched. It's Left!</color>",
        "Marc: I'm starting to lose track. Right again?\nRaven: My phone is vibrating. The signal is stronger to the Right.",
        "Cristel: Right! I can almost smell the outside air!\n<color=#A5A5A5>Raven: No, that's just the smell of rust. But... Right feels correct.</color>",
        "Marc: We've been here before. It's right, surely?\n<color=#A5A5A5>Raven: No. The pattern broke. It's Left. You HAVE to trust me.</color>",
        "Cristel: One last turn! To the right, everyone!\nRaven: This is it. The end of the loop. The rooftop is right there."
    };

    void Start()
    {
        UpdateDisplay("");
        if (transitionPanel != null) transitionPanel.SetActive(false);
    }

    public void ChooseLeft() { HandleChoice(false); }
    public void ChooseRight() { HandleChoice(true); }

    private void HandleChoice(bool playerChoseRight)
    {
        if (playerChoseRight == correctSequence[currentStage])
        {
            currentStage++;
            if (currentStage >= correctSequence.Length) FinishMaze();
            else StartCoroutine(ProcessTurn(false, false));
        }
        else
        {
            chances--;
            if (chances > 0)
            {
                // Wrong but has lives left
                StartCoroutine(ProcessTurn(false, true));
            }
            else
            {
                // Out of lives - RESET
                currentStage = 0;
                chances = 3;
                StartCoroutine(ProcessTurn(true, true));
            }
        }
    }

    private void UpdateDisplay(string header)
    {
        // Update UI Dots
        for (int i = 0; i < chanceIndicators.Length; i++)
        {
            if (chanceIndicators[i] != null)
                chanceIndicators[i].SetActive(i < chances);
        }

        if (currentStage < correctSequence.Length && dialogueText != null)
        {
            string clue = $"<color=#FFD700>[Clue: {environmentalClues[currentStage]}]</color>";
            dialogueText.text = header + clue + "\n\n" + characterDialogue[currentStage];
        }
    }

    IEnumerator ProcessTurn(bool wasFullReset, bool wasWrong)
    {
        if (transitionPanel != null) transitionPanel.SetActive(true);

        // If wrong, you can trigger a camera shake or sound effect here
        if (wasWrong)
        {
            // Camera.main.GetComponent<ScreenShake>()?.Trigger(); 
        }

        yield return new WaitForSeconds(0.8f);

        string msg = "";
        if (wasFullReset) msg = "<color=#FF0000>[REALITY COLLAPSE]</color>\nThe dimensions fold. You are back at the start.\n\n";
        else if (wasWrong) msg = "<color=#FF6600>[DISTORTION]</color>\nA wave of nausea hits. Something broke, but you are still here.\n\n";

        UpdateDisplay(msg);

        if (transitionPanel != null) transitionPanel.SetActive(false);
    }

    void FinishMaze()
    {
        dialogueText.text = "<color=#6366F1>[SUCCESS]</color>\n\nThe staircase to the rooftop finally manifests before you.";
        Invoke("GoToChapter5", 3.5f);
    }

    void GoToChapter5() { SceneManager.LoadScene("Marc_TestScene"); }
}