using UnityEngine;

public class GhostNodeAI : MonoBehaviour
{
	public NodeController currentNode;
	public float moveSpeed = 3f; // 👈 adjust here

	private NodeController targetNode;
	private NodeController previousNode;

	private Transform player;
	private PlayerNodeMovement playerScript;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerScript = player.GetComponent<PlayerNodeMovement>();

		targetNode = currentNode;
		transform.position = currentNode.transform.position;
	}

	void Update()
	{
		MoveToNode();

		if (targetNode == currentNode)
		{
			ChooseNextNode();
		}
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
			previousNode = currentNode;
			currentNode = targetNode;
		}
	}

	void ChooseNextNode()
	{
		NodeController bestNode = null;
		float bestDistance = Mathf.Infinity;

		NodeController[] options = new NodeController[]
		{
			currentNode.nodeUp?.GetComponent<NodeController>(),
			currentNode.nodeDown?.GetComponent<NodeController>(),
			currentNode.nodeLeft?.GetComponent<NodeController>(),
			currentNode.nodeRight?.GetComponent<NodeController>()
		};

		Vector3 targetPos = player.position;

		// 🔥 smarter targeting (uses player's node)
		if (playerScript != null && playerScript.currentNode != null)
		{
			targetPos = playerScript.currentNode.transform.position;
		}

		// First pass: avoid going back
		foreach (NodeController node in options)
		{
			if (node == null) continue;
			if (node == previousNode) continue;

			float dist = Vector3.Distance(node.transform.position, targetPos) + Random.Range(0f, 0.1f);

			if (dist < bestDistance)
			{
				bestDistance = dist;
				bestNode = node;
			}
		}

		// If stuck → allow backtracking
		if (bestNode == null)
		{
			foreach (NodeController node in options)
			{
				if (node == null) continue;

				float dist = Vector3.Distance(node.transform.position, targetPos);

				if (dist < bestDistance)
				{
					bestDistance = dist;
					bestNode = node;
				}
			}
		}

		if (bestNode != null)
		{
			targetNode = bestNode;
		}
	}
}