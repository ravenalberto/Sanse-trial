using UnityEngine;

public class StaircaseGameManager : MonoBehaviour
{
	public int totalOrbs;

	void Start()
	{
		totalOrbs = GameObject.FindGameObjectsWithTag("Collectible").Length;
		Debug.Log("Orbs: " + totalOrbs);
	}

	public void OrbCollected()
	{
		totalOrbs--;

		Debug.Log("Remaining: " + totalOrbs);

		if (totalOrbs <= 0)
		{
			Debug.Log("YOU WIN 👁");
		}
	}
}