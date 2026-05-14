using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject wallPrefab;      // The main railing/wall
    public GameObject doorPrefab;      // Standard classroom doors
    public GameObject exitDoorPrefab;  // The Rooftop exit door

    [Header("Abandoned Building Settings")]
    public float cellSize = 10f;
    public int width = 5;              // Wider building for multiple rooms
    public int depth = 20;             // Building depth

    [Range(0, 1)]
    public float doorChance = 0.45f;   // Chance for a door to appear in a divider
    [Range(0, 1)]
    public float wallRemovalChance = 0.3f; // Chance to skip a wall to create larger rooms

    [Header("Door Alignment")]
    public Vector3 doorScale = new Vector3(1f, 1f, 1f);
    public float doorYOffset = -1.5f;
    public Vector3 doorRotationOffset = new Vector3(0, 90, 0);

    [Header("Hole Size Adjustment")]
    public float actualDoorWidth = 4.0f;

    void Start()
    {
        // Calculate start position to center the building
        Vector3 startPos = transform.position - new Vector3((width * cellSize) / 2f - (cellSize / 2f), 0, (depth * cellSize) / 2f - (cellSize / 2f));

        // Use current time as seed for a different layout every play
        Random.InitState(System.DateTime.Now.Millisecond);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 pos = startPos + new Vector3(x * cellSize, 0, z * cellSize);

                // --- NORTH WALLS (Horizontal Dividers) ---
                if (z == depth - 1)
                {
                    // THE ROOFTOP EXIT: Place at the back center of the building
                    if (x == width / 2)
                    {
                        SpawnDoorWithFillers(exitDoorPrefab, pos + new Vector3(0, 0, cellSize / 2f), Quaternion.identity);
                    }
                    else
                    {
                        Instantiate(wallPrefab, pos + new Vector3(0, 0, cellSize / 2f), Quaternion.identity, transform);
                    }
                }
                else
                {
                    // Internal partitions: Randomly decide to place a wall, a door, or open space
                    float rand = Random.value;
                    if (rand > wallRemovalChance)
                    {
                        if (Random.value < doorChance)
                            SpawnDoorWithFillers(doorPrefab, pos + new Vector3(0, 0, cellSize / 2f), Quaternion.identity);
                        else
                            Instantiate(wallPrefab, pos + new Vector3(0, 0, cellSize / 2f), Quaternion.identity, transform);
                    }
                }

                // --- WEST WALLS (Vertical Dividers) ---
                if (x == 0)
                {
                    // Outer left boundary
                    Instantiate(wallPrefab, pos + new Vector3(-cellSize / 2f, 0, 0), Quaternion.Euler(0, 90, 0), transform);
                }
                else
                {
                    float rand = Random.value;
                    if (rand > wallRemovalChance)
                    {
                        if (Random.value < doorChance)
                            SpawnDoorWithFillers(doorPrefab, pos + new Vector3(-cellSize / 2f, 0, 0), Quaternion.Euler(0, 90, 0));
                        else
                            Instantiate(wallPrefab, pos + new Vector3(-cellSize / 2f, 0, 0), Quaternion.Euler(0, 90, 0), transform);
                    }
                }

                // --- OUTER BOUNDARIES ---
                // EAST WALL
                if (x == width - 1)
                {
                    Instantiate(wallPrefab, pos + new Vector3(cellSize / 2f, 0, 0), Quaternion.Euler(0, 90, 0), transform);
                }
                // SOUTH WALL (Start/Entrance)
                if (z == 0)
                {
                    Instantiate(wallPrefab, pos + new Vector3(0, 0, -cellSize / 2f), Quaternion.identity, transform);
                }
            }
        }
    }

    void SpawnDoorWithFillers(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return;

        Quaternion finalRotation = rotation * Quaternion.Euler(doorRotationOffset);
        GameObject door = Instantiate(prefab, position + new Vector3(0, doorYOffset, 0), finalRotation, transform);
        door.transform.localScale = doorScale;

        // Ensure the collider is on the moving panel so students can pass when open
        Transform panel = null;
        foreach (Transform child in door.GetComponentsInChildren<Transform>())
        {
            if (child.name.Contains("01")) { panel = child; break; }
        }

        if (panel != null)
        {
            if (panel.GetComponent<Collider>() == null) panel.gameObject.AddComponent<BoxCollider>();

            // Remove collider from parent frame if it exists to prevent "invisible walls"
            BoxCollider parentCol = door.GetComponent<BoxCollider>();
            if (parentCol != null && !parentCol.isTrigger) Destroy(parentCol);
        }

        float fillerWidth = (cellSize - actualDoorWidth) / 2f;

        // Left Filler
        Vector3 leftOffset = rotation * new Vector3(-cellSize / 2f + fillerWidth / 2f, 0, 0);
        GameObject leftFiller = Instantiate(wallPrefab, position + leftOffset, rotation, transform);
        leftFiller.transform.localScale = new Vector3(fillerWidth, leftFiller.transform.localScale.y, leftFiller.transform.localScale.z);
        FixFillerCollider(leftFiller);

        // Right Filler
        Vector3 rightOffset = rotation * new Vector3(cellSize / 2f - fillerWidth / 2f, 0, 0);
        GameObject rightFiller = Instantiate(wallPrefab, position + rightOffset, rotation, transform);
        rightFiller.transform.localScale = new Vector3(fillerWidth, rightFiller.transform.localScale.y, rightFiller.transform.localScale.z);
        FixFillerCollider(rightFiller);
    }

    void FixFillerCollider(GameObject filler)
    {
        BoxCollider bc = filler.GetComponent<BoxCollider>();
        if (bc == null) bc = filler.GetComponentInChildren<BoxCollider>();
        if (bc != null)
        {
            bc.center = Vector3.zero;
            bc.size = new Vector3(1f, 1f, 1f);
        }
    }
}