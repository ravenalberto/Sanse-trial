using UnityEngine;

public class Interactable : MonoBehaviour
{

	bool alreadyInteracted = false;


	public string characterName;
	public string[] speakerNames;

	[TextArea]
	public string[] dialogueLines;

	[Header("Choices")]
	public bool useChoices = false;

	public string redChoiceText;
	public string blueChoiceText;

	private bool playerInRange = false;

	public GameObject promptUI;

	public bool isFinalMarc = false;

	void Update()
	{
		// 🎬 FINAL MARC
		if (isFinalMarc)
		{
			if (
				InteractionProgress.Instance.interactionsFinished >= 4 &&
				Input.GetKeyDown(KeyCode.E)
			)
			{
				Talk();
			}

			return;
		}

		// NORMAL CHARACTERS
		if (playerInRange && Input.GetKeyDown(KeyCode.E))
		{
			Talk();
		}
	}

	public SimpleDialogue dialogueSystem;

	void Talk()
	{
		if (alreadyInteracted) return;

		alreadyInteracted = true;

		dialogueSystem.ShowDialogue(
	speakerNames,
	dialogueLines,
	gameObject
);

		if (useChoices)
		{
			dialogueSystem.SetupChoices(
				redChoiceText,
				blueChoiceText
			);
		}

		//gameObject.SetActive(false);
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