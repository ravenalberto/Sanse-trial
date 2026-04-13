using UnityEngine;

public class PlayerNodeMovement : MonoBehaviour
{
	public NodeController currentNode;
	public float moveSpeed = 3f;

	private NodeController targetNode;

	private Vector2 currentDirection;
	private Vector2 desiredDirection;

	int pelletsEaten = 0;

	Animator anim;

	void Start()
	{
		anim = GetComponent<Animator>();

		targetNode = currentNode;
		transform.position = currentNode.transform.position;

		currentDirection = Vector2.right;
		desiredDirection = Vector2.right;

		MoveForward();
		UpdateAnimation(); // 👈 start anim immediately
	}

	void Update()
	{
		GetInput();
		MoveToNode();
		EatPellet(); // 👈 THIS FIXES EVERYTHING
	}

	void GetInput()
	{
		if (Input.GetKey(KeyCode.W)) desiredDirection = Vector2.up;
		if (Input.GetKey(KeyCode.S)) desiredDirection = Vector2.down;
		if (Input.GetKey(KeyCode.A)) desiredDirection = Vector2.left;
		if (Input.GetKey(KeyCode.D)) desiredDirection = Vector2.right;
	}

	void MoveToNode()
	{
		if (targetNode != null)
		{
			transform.position = Vector3.MoveTowards(
				transform.position,
				targetNode.transform.position,
				moveSpeed * Time.deltaTime
			);

			if (Vector3.Distance(transform.position, targetNode.transform.position) < 0.01f)
			{
				currentNode = targetNode;

				TryChangeDirection();
				MoveForward();
				UpdateAnimation(); // 👈 update when direction changes
			}

			// 🍒 Eat pellet if exists
			Pellet pellet = currentNode.GetComponentInChildren<Pellet>();

			if (pellet != null && pellet.gameObject.activeSelf)
			{
				pellet.Eat();
			pelletsEaten++;
				Debug.Log("Pellets: " + pelletsEaten);
			}
		}
	}

	void EatPellet()
	{
		Pellet pellet = currentNode.GetComponentInChildren<Pellet>();

		if (pellet != null && pellet.gameObject.activeSelf)
		{
			pellet.Eat();
		}
	}

	void TryChangeDirection()
	{
		NodeController nextNode = GetNodeFromDirection(desiredDirection);

		if (nextNode != null)
		{
			currentDirection = desiredDirection;
			targetNode = nextNode;
		}
	}

	void MoveForward()
	{
		NodeController nextNode = GetNodeFromDirection(currentDirection);

		if (nextNode != null)
		{
			targetNode = nextNode;
		}
	}

	void UpdateAnimation()
	{
		if (currentDirection == Vector2.up)
			anim.Play("kuh_up");
		else if (currentDirection == Vector2.down)
			anim.Play("kuh_down");
		else if (currentDirection == Vector2.left)
			anim.Play("kuh_left");
		else if (currentDirection == Vector2.right)
			anim.Play("kuh_right");
		else
			anim.Play("kuh_idle");
	}

	NodeController GetNodeFromDirection(Vector2 dir)
	{
		if (dir == Vector2.up && currentNode.nodeUp != null)
			return currentNode.nodeUp.GetComponent<NodeController>();

		if (dir == Vector2.down && currentNode.nodeDown != null)
			return currentNode.nodeDown.GetComponent<NodeController>();

		if (dir == Vector2.left && currentNode.nodeLeft != null)
			return currentNode.nodeLeft.GetComponent<NodeController>();

		if (dir == Vector2.right && currentNode.nodeRight != null)
			return currentNode.nodeRight.GetComponent<NodeController>();

		return null;
	}
}