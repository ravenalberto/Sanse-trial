using UnityEngine;

public class Interactable : MonoBehaviour
{
	public string characterName;
	[TextArea] public string dialogueLine;

	private bool playerInRange = false;

	public GameObject promptUI;

	void Update()
	{
		if (playerInRange && Input.GetKeyDown(KeyCode.E))
		{
			Talk();
		}
	}

	public SimpleDialogue dialogueSystem;

	void Talk()
	{
		if (dialogueSystem == null)
		{
			Debug.LogError("Dialogue system not assigned!");
			return;
		}

		dialogueSystem.ShowDialogue(characterName, dialogueLine);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			playerInRange = true;
			if (promptUI != null) promptUI.SetActive(true);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			playerInRange = false;
			if (promptUI != null) promptUI.SetActive(false);
		}
	}
}