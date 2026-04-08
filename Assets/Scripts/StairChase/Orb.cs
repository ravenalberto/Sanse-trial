using UnityEngine;

public class Orb : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("Hit: " + other.name);

		if (other.CompareTag("Player"))
		{
			Debug.Log("PLAYER CONFIRMED");

			var gm = FindFirstObjectByType<StaircaseGameManager>();

			if (gm != null)
				gm.OrbCollected();

			Destroy(gameObject);
		}
	}
}