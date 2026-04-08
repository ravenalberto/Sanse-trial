using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChairPuzzleManager : MonoBehaviour
{
	public GridSlot[] slots;

	public GridSlot slot1, slot2, slot3, slot4;

	public GameObject resultPanel;
	public TMPro.TMP_Text resultText;

	public void CheckSolution()
	{
		if (slots == null || slots.Length == 0)
		{
			Debug.LogError("Slots array not assigned!");
			return;
		}

		// DEBUG
		Debug.Log("Slot1: " + (slot1.currentChair != null ? slot1.currentChair.chairName : "EMPTY"));
		Debug.Log("Slot2: " + (slot2.currentChair != null ? slot2.currentChair.chairName : "EMPTY"));
		Debug.Log("Slot3: " + (slot3.currentChair != null ? slot3.currentChair.chairName : "EMPTY"));
		Debug.Log("Slot4: " + (slot4.currentChair != null ? slot4.currentChair.chairName : "EMPTY"));

		// --- CHECK POSITIONS ---
		bool marcCorrect = slot1.currentChair != null && slot1.currentChair.chairName == "Marc";
		bool ravenCorrect = slot2.currentChair != null && slot2.currentChair.chairName == "Raven";

		bool slot3Valid = slot3.currentChair != null &&
			(slot3.currentChair.chairName == "Empty1" || slot3.currentChair.chairName == "Empty2");

		bool slot4Valid = slot4.currentChair != null &&
			(slot4.currentChair.chairName == "Empty1" || slot4.currentChair.chairName == "Empty2");

		bool correctStructure = marcCorrect && ravenCorrect;

		// --- CHECK KUH ---
		bool kuhInGrid = false;
		foreach (GridSlot slot in slots)
		{
			if (slot.currentChair != null && slot.currentChair.chairName == "Kuh")
			{
				kuhInGrid = true;
			}
		}

		// --- FINAL RESULT ---
		resultPanel.SetActive(true);

		// Check ONLY core structure first
		if (marcCorrect && ravenCorrect)
		{
			if (kuhInGrid)
			{
				resultText.text = "Kuh disapproves (-Trust)";
				resultText.color = Color.red;
				PuzzleState.scene2Result = "Disapprove";

			}
			else if (slot3Valid && slot4Valid)
			{
				resultText.text = "Kuh approves (+Trust)";
				resultText.color = Color.green;
				PuzzleState.scene2Result = "Approve";
			}
			else
			{
				resultText.text = "Game Over";
				resultText.color = Color.red;
				PuzzleState.scene2Result = "GameOver";
			}
		}
		else
		{
			resultText.text = "Game Over";
			resultText.color = Color.red;
			PuzzleState.scene2Result = "GameOver";
		}

		StartCoroutine(HidePopup());
		
	}

	

	IEnumerator HidePopup()
	{
		yield return new WaitForSeconds(2f);
		resultPanel.SetActive(false);

		SceneManager.LoadScene("ClassroomScene01");
	}
}