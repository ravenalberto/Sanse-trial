using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class SimpleDialogue : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject vnUI;            // The parent container (VN_ui)
    public GameObject textBox;         // Your TextBox prefab
    public GameObject nextButton;      // Your NextButton prefab
    public TMP_Text nameText;          // Text inside TextBox
    public TMP_Text dialogueText;      // Text inside TextBox

    [Header("Settings")]
    public float typingSpeed = 0.03f;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentFullLine;

    void Start()
    {
        if (vnUI != null) vnUI.SetActive(false);
        if (nextButton != null) nextButton.SetActive(false);
    }


	public PlayerMovement playerMovement;
	public CameraFollow cameraFollow;
	public void ShowDialogue(string speaker, string line)
    {
		playerMovement.canMove = false;
		cameraFollow.canLook = false;

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;




		if (vnUI == null) return;

        vnUI.SetActive(true);
        textBox.SetActive(true);
        nextButton.SetActive(false); // Hide next button while typing

        nameText.text = speaker;
        currentFullLine = line;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(line));
    }

    private IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        nextButton.SetActive(true); // Show the NextButton once finished
    }

	// Call this from the NextButton's OnClick event
	public void OnNextButtonClicked()
	{
		if (isTyping)
		{
			// Skip typing and show full text
			StopCoroutine(typingCoroutine);
			dialogueText.text = currentFullLine;
			isTyping = false;
			nextButton.SetActive(true);
		}
		else
		{
			// Close UI
			vnUI.SetActive(false);

			// ENABLE PLAYER AGAIN
			playerMovement.canMove = true;
			cameraFollow.canLook = true;
		}
	}
}