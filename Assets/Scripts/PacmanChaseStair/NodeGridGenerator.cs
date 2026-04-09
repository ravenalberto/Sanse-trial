using UnityEngine;

public class NodeGridGenerator : MonoBehaviour
{
	public GameObject nodePrefab;

	public int width = 10;
	public int height = 10;
	public float spacingX = 1f;
	public float spacingY = 1f;

	[ContextMenu("Generate Grid")]
	void GenerateGrid()
	{
		// Clear old nodes
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			DestroyImmediate(transform.GetChild(i).gameObject);
		}

		// Generate new grid
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				float offsetX = (width - 1) * spacingX / 2f;
				float offsetY = (height - 1) * spacingY / 2f;

				Vector3 pos = new Vector3(
					transform.position.x + (x * spacingX) - offsetX,
					transform.position.y + (y * spacingY) - offsetY,
					0
				);

				GameObject node = Instantiate(nodePrefab, pos, Quaternion.identity);
				node.transform.parent = transform;
				node.name = "Node";
			}
		}
	}
}