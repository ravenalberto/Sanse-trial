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

	public string characterID;

	void Update()
	{
		// 🎬 FINAL MARC
		if (isFinalMarc)
		{
			Debug.Log("FINAL MARC CHECK");

			if (InteractionProgress.Instance == null)
			{
				Debug.Log("INTERACTION PROGRESS NULL");
				return;
			}

			Debug.Log(
				"COUNT: " +
				InteractionProgress.Instance.interactionsFinished
			);

			if (
				InteractionProgress.Instance.interactionsFinished >= 4 &&
				Input.GetKeyDown(KeyCode.E)
			)
			{
				Debug.Log("MARC TALK TRIGGERED");

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
		Debug.Log("TALK STARTED");
		Debug.Log("alreadyInteracted: " + alreadyInteracted);

		if (alreadyInteracted) return;

		alreadyInteracted = true;

		dialogueSystem.ShowDialogue(
	speakerNames,
	dialogueLines,
	gameObject,
	characterID
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