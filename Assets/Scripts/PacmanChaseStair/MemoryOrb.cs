using UnityEngine;

public class MemoryOrb : MonoBehaviour
{
	public int orbID;

	public void Activate()
	{
		Debug.Log("Memory Orb Activated!");

		GameplayDialogueController controller =
			FindObjectOfType<GameplayDialogueController>();

		if (controller == null)
		{
			Debug.Log("❌ CONTROLLER NOT FOUND");
		}
		else
		{
			Debug.Log("✅ CONTROLLER FOUND");
			controller.StartMemoryDialogue(orbID);
		}

		gameObject.SetActive(false);
	}


}