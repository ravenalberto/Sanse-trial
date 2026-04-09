using UnityEngine;

public class NodeConnector : MonoBehaviour
{
	public float connectionDistance = 1.1f;

	[ContextMenu("Connect Nodes")]
	void ConnectNodes()
	{
		GridNode[] nodes = GetComponentsInChildren<GridNode>();

		foreach (GridNode node in nodes)
		{
			node.up = null;
			node.down = null;
			node.left = null;
			node.right = null;

			foreach (GridNode other in nodes)
			{
				if (node == other) continue;

				Vector2 dir = other.transform.position - node.transform.position;

				// Horizontal
				if (Mathf.Abs(dir.y) < 0.1f)
				{
					if (dir.x > 0 && dir.magnitude < connectionDistance)
						node.right = other;

					if (dir.x < 0 && dir.magnitude < connectionDistance)
						node.left = other;
				}

				// Vertical
				if (Mathf.Abs(dir.x) < 0.1f)
				{
					if (dir.y > 0 && dir.magnitude < connectionDistance)
						node.up = other;

					if (dir.y < 0 && dir.magnitude < connectionDistance)
						node.down = other;
				}
			}
		}

		Debug.Log("Nodes connected!");
	}
}