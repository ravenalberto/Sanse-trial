using UnityEngine;

public class InteractionProgress : MonoBehaviour
{
	public static InteractionProgress Instance;

	public int interactionsFinished = 0;

	void Awake()
	{
		Instance = this;
	}

	public void AddInteraction()
	{
		interactionsFinished++;

		Debug.Log("Interactions Finished: " + interactionsFinished);
	}
}