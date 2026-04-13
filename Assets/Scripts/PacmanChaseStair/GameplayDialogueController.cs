using System.Collections;
using TMPro;
using UnityEngine;

public class GameplayDialogueController : MonoBehaviour
{
	public GameObject vnUI;
	public GameObject nextButton;
	public TMP_Text dialogueText;
	public TMP_Text nameText;
	public GameObject textBox;

	int currentOrb = -1;
	bool waitingForNext = false;

	int dialogueStep = 0; // ✅ NEW

	void Start()
	{
		vnUI.SetActive(false);
		textBox.SetActive(false);
		nextButton.SetActive(false);
	}

	public GameObject charKuh;
	public GameObject charMarc;
	public GameObject charCristel;
	public GameObject charDarlene;
	public GameObject charRaven;

	void HideAllCharacters()
	{
		charKuh.SetActive(false);
		charMarc.SetActive(false);
		charCristel.SetActive(false);
		charDarlene.SetActive(false);
		charRaven.SetActive(false);
	}

	public void StartMemoryDialogue(int orbID)
	{
		currentOrb = orbID;

		dialogueStep = 0; // ✅ RESET STEP
		waitingForNext = true;

		vnUI.SetActive(true);
		textBox.SetActive(true);
		nextButton.SetActive(true);

		Time.timeScale = 0f;

		ShowLine(); // ✅ IMPORTANT
	}

	void ShowLine()
	{
		HideAllCharacters();

		// 🔥 ORB 0
		if (currentOrb == 0)
		{
			if (dialogueStep == 0)
			{
				nameText.text = "Kuh";
				dialogueText.text = "…Marc.";
				charKuh.SetActive(true);
			}
		}

		// 🔥 ORB 1
		else if (currentOrb == 1)
		{
			if (dialogueStep == 0)
			{
				nameText.text = "Kuh";
				dialogueText.text = "Cristel… I remember.";
				charKuh.SetActive(true);
			}
		}

		// 🔥 ORB 2
		else if (currentOrb == 2)
		{
			if (dialogueStep == 0)
			{
				nameText.text = "Kuh";
				dialogueText.text = "Darlene…";
				charKuh.SetActive(true);
			}
		}

		// 🔥 ORB 3
		else if (currentOrb == 3)
		{
			if (dialogueStep == 0)
			{
				nameText.text = "Kuh";
				dialogueText.text = "Raven…";
				charKuh.SetActive(true);
			}
		}

		waitingForNext = true; // ✅ allow next click
	}

	public void OnSkipPressed()
	{
		Debug.Log("SKIP PRESSED");

		waitingForNext = false;
		ResumeGameplay();
	}

	public void OnNextPressed()
	{
		if (!waitingForNext) return;

		waitingForNext = false;

		dialogueStep++; // ✅ MOVE TO NEXT LINE

		// 👇 if no more lines → exit
		if (dialogueStep >= 1) // currently 1 line per orb
		{
			ResumeGameplay();
		}
		else
		{
			ShowLine();
		}
	}

	void ResumeGameplay()
	{
		vnUI.SetActive(false);
		Time.timeScale = 1f;

		if (MemoryManager.Instance != null)
		{
			MemoryManager.Instance.CollectOrb(currentOrb);
		}
	}
}