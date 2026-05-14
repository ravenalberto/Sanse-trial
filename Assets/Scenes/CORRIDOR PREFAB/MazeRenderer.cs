using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public GameObject exitDoorPrefab;

    [Tooltip("Drag a light or glowing sphere here. It will form a line to the exit.")]
    public GameObject guideMarkerPrefab;

    [Header("Abandoned Building Settings")]
    public float cellSize = 10f;
    public int width = 5;
    public int depth = 15; // Reduced slightly so it's not a marathon to the end

    [Range(0, 1)]
    public float doorChance = 0.5f;
    [Range(0, 1)]
    public float wallRemovalChance = 0.4f;

    [Header("Door Alignment")]
    public Vector3 doorScale = new Vector3(1f, 1f, 1f);
    public float doorYOffset = -1.5f;
    public Vector3 doorRotationOffset = new Vector3(0, 90, 0);

    [Header("Hole Size Adjustment")]
    public float actualDoorWidth = 4.5f; // Wider for easier movement

    void Start()
    {
        Vector3 startPos = transform.position - new Vector3((width * cellSize) / 2f - (cellSize / 2f), 0, (depth * cellSize) / 2f - (cellSize / 2f));
        Random.InitState(System.DateTime.Now.Millisecond);

        int exitX = width / 2; // This is our "Main Hallway" column

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 pos = startPos + new Vector3(x * cellSize, 0, z * cellSize);

                // BREADCRUMB TRAIL: Place a marker at every step in the center aisle
                if (guideMarkerPrefab != null && x == exitX)
                {
                    Instantiate(guideMarkerPrefab, pos + new Vector3(0, 1f, 0), Quaternion.identity, transform);
                }

                // --- NORTH WALLS (Horizontal Dividers) ---
                if (z == depth - 1)
                {
                    // THE FINAL EXIT
                    if (x == exitX)
                        SpawnDoorWithFillers(exitDoorPrefab, pos + new Vector3(0, 0, cellSize / 2f), Quaternion.identity);
                    else
                        Instantiate(wallPrefab, pos + new Vector3(0, 0, cellSize / 2f), Quaternion.identity, transform);
                }
                else
                {
                    // MAIN HALLWAY LOGIC: Center aisle should always have a door or opening
                    if (x == exitX)
                    {
                        // 90% chance of a door in the center aisle to keep you moving forward
                        if (Random.value < 0.9f)
                            SpawnDoorWithFillers(doorPrefab, pos + new Vector3(0, 0, cellSize / 2f), Quaternion.identity);
                        else
                        // 10% chance it's just a wide open gap
                        { }
                    }
                    else
                    {
                        // Side rooms: use standard random generation
                        if (Random.value > wallRemovalChance)
                        {
                            if (Random.value < doorChance)
                                SpawnDoorWithFillers(doorPrefab, pos + new Vector3(0, 0, cellSize / 2f), Quaternion.identity);
                            else
                                Instantiate(wallPrefab, pos + new Vector3(0, 0, cellSize / 2f), Quaternion.identity, transform);
                        }
                    }
                }

                // --- WEST WALLS (Vertical Dividers) ---
                if (x == 0)
                {
                    Instantiate(wallPrefab, pos + new Vector3(-cellSize / 2f, 0, 0), Quaternion.Euler(0, 90, 0), transform);
                }
                else
                {
                    // Rooms adjacent to the hallway are more likely to have doors so you don't get trapped
                    float effectiveRemoval = (x == exitX || x == exitX + 1) ? wallRemovalChance + 0.2f : wallRemovalChance;

                    if (Random.value > effectiveRemoval)
                    {
                        if (Random.value < doorChance)
                            SpawnDoorWithFillers(doorPrefab, pos + new Vector3(-cellSize / 2f, 0, 0), Quaternion.Euler(0, 90, 0));
                        else
                            Instantiate(wallPrefab, pos + new Vector3(-cellSize / 2f, 0, 0), Quaternion.Euler(0, 90, 0), transform);
                    }
                }

                if (x == width - 1)
                    Instantiate(wallPrefab, pos + new Vector3(cellSize / 2f, 0, 0), Quaternion.Euler(0, 90, 0), transform);

                if (z == 0)
                    Instantiate(wallPrefab, pos + new Vector3(0, 0, -cellSize / 2f), Quaternion.identity, transform);
            }
        }
    }

    void SpawnDoorWithFillers(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return;
        Quaternion finalRotation = rotation * Quaternion.Euler(doorRotationOffset);
        GameObject door = Instantiate(prefab, position + new Vector3(0, doorYOffset, 0), finalRotation, transform);
        door.transform.localScale = doorScale;

        float fillerWidth = (cellSize - actualDoorWidth) / 2f;
        Vector3 leftOffset = rotation * new Vector3(-cellSize / 2f + fillerWidth / 2f, 0, 0);
        GameObject leftFiller = Instantiate(wallPrefab, position + leftOffset, rotation, transform);
        leftFiller.transform.localScale = new Vector3(fillerWidth, leftFiller.transform.localScale.y, leftFiller.transform.localScale.z);

        Vector3 rightOffset = rotation * new Vector3(cellSize / 2f - fillerWidth / 2f, 0, 0);
        GameObject rightFiller = Instantiate(wallPrefab, position + rightOffset, rotation, transform);
        rightFiller.transform.localScale = new Vector3(fillerWidth, rightFiller.transform.localScale.y, rightFiller.transform.localScale.z);
    }
}