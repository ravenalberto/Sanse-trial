using UnityEngine;

public class PlayerNodeMovement : MonoBehaviour
{
	public GridNode currentNode;
	public float moveSpeed = 5f;

	private GridNode targetNode;

	void Start()
	{
		targetNode = currentNode;
		transform.position = currentNode.transform.position;
	}

	void Update()
	{
		HandleInput();
		MoveToNode();
	}

	void HandleInput()
	{
		if (targetNode != currentNode) return;

		if (Input.GetKey(KeyCode.W) && currentNode.up != null)
			targetNode = currentNode.up;

		if (Input.GetKey(KeyCode.S) && currentNode.down != null)
			targetNode = currentNode.down;

		if (Input.GetKey(KeyCode.A) && currentNode.left != null)
			targetNode = currentNode.left;

		if (Input.GetKey(KeyCode.D) && currentNode.right != null)
			targetNode = currentNode.right;
	}

	void MoveToNode()
	{
		if (targetNode == null) return;

		transform.position = Vector3.MoveTowards(
			transform.position,
			targetNode.transform.position,
			moveSpeed * Time.deltaTime
		);

		if (Vector3.Distance(transform.position, targetNode.transform.position) < 0.05f)
		{
			currentNode = targetNode;
		}
	}
}