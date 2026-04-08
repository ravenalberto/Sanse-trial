using UnityEngine;

public class GhostNodeAI : MonoBehaviour
{
	public float speed = 2f;

	private Transform player;
	private Transform currentNode;
	private Transform targetNode;

	void Start()
	{
		GameObject p = GameObject.FindGameObjectWithTag("Player");

		if (p != null)
			player = p.transform;

		// 👇 find nearest node at start
		Node[] allNodes = FindObjectsOfType<Node>();

		float closestDist = Mathf.Infinity;
		Node closestNode = null;

		foreach (Node node in allNodes)
		{
			float dist = Vector2.Distance(transform.position, node.transform.position);

			if (dist < 5f) // 👈 bigger search radius

				if (dist < closestDist)
			{
				closestDist = dist;
				closestNode = node;
			}
		}

		if (closestNode != null)
		{
			currentNode = closestNode.transform;
			ChooseNextNode();
		}
	}

	void Update()
	{
		if (targetNode != null)
		{
			transform.position = Vector2.MoveTowards(
				transform.position,
				targetNode.position,
				speed * Time.deltaTime
			);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Node"))
		{
			currentNode = other.GetComponent<Node>().transform;
			ChooseNextNode();
		}

		if (other.CompareTag("Player"))
		{
			Debug.Log("CAUGHT 👁");
			other.transform.position = Vector3.zero;
		}
	}

	void ChooseNextNode()
	{
		if (currentNode == null) return; // 👈 prevents crash

		Node nodeScript = currentNode.GetComponent<Node>();

		if (nodeScript == null) return;

		float bestDistance = Mathf.Infinity;
		Node bestNode = null;

		foreach (Node neighbor in nodeScript.neighbors)
		{
			if (neighbor == null) continue;

			float dist = Vector2.Distance(neighbor.transform.position, player.position);

			if (dist < bestDistance)
			{
				bestDistance = dist;
				bestNode = neighbor;
			}
		}

		if (bestNode != null)
		{
			targetNode = bestNode.transform;
		}
	}
}