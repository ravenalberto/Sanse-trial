using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{
	public bool canMoveLeft = false;
	public bool canMoveRight = false;
	public bool canMoveUp = false;
	public bool canMoveDown = false;

	public GameObject nodeLeft;
	public GameObject nodeRight;
	public GameObject nodeUp;
	public GameObject nodeDown;

	float rayDistance = 0.6f; // adjust if needed

	void Start()
	{
		CheckDirection(Vector2.left, out canMoveLeft, out nodeLeft);
		CheckDirection(Vector2.right, out canMoveRight, out nodeRight);
		CheckDirection(Vector2.up, out canMoveUp, out nodeUp);
		CheckDirection(Vector2.down, out canMoveDown, out nodeDown);
	}

	void CheckDirection(Vector2 direction, out bool canMove, out GameObject node)
	{
		canMove = false;
		node = null;

		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, rayDistance);

		for (int i = 0; i < hits.Length; i++)
		{
			if (hits[i].collider.gameObject == gameObject)
				continue;

			if (hits[i].collider.CompareTag("Node"))
			{
				canMove = true;
				node = hits[i].collider.gameObject;
			}
		}
	}
}