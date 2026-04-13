using UnityEngine;

public class Pellet : MonoBehaviour
{
	public void Eat()
	{
		gameObject.SetActive(false);
	}
}