using UnityEngine;
using System.Collections.Generic;

// 1. DATA STRUCTURE: Keeps track of wall states
public class MazeCell
{
    public bool visited = false;
    public bool northWall = true;
    public bool southWall = true;
    public bool eastWall = true;
    public bool westWall = true;
}

// 2. GENERATOR: The Algorithm
public class MazeGenerator : MonoBehaviour
{
    public int width = 7;
    public int depth = 23;

    public MazeCell[,] GenerateMaze()
    {
        MazeCell[,] cells = new MazeCell[width, depth];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                cells[x, z] = new MazeCell();
            }
        }

        // Recursive Backtracker Logic
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int current = new Vector2Int(0, 0);
        cells[current.x, current.y].visited = true;

        int visitedCount = 1;
        int totalCells = width * depth;

        while (visitedCount < totalCells)
        {
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current, cells);

            if (neighbors.Count > 0)
            {
                Vector2Int next = neighbors[Random.Range(0, neighbors.Count)];
                RemoveWalls(current, next, cells);
                stack.Push(current);
                current = next;
                cells[current.x, current.y].visited = true;
                visitedCount++;
            }
            else if (stack.Count > 0)
            {
                current = stack.Pop();
            }
            else
            {
                break;
            }
        }
        return cells;
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int p, MazeCell[,] cells)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        if (p.x > 0 && !cells[p.x - 1, p.y].visited) neighbors.Add(new Vector2Int(p.x - 1, p.y));
        if (p.x < width - 1 && !cells[p.x + 1, p.y].visited) neighbors.Add(new Vector2Int(p.x + 1, p.y));
        if (p.y > 0 && !cells[p.x, p.y - 1].visited) neighbors.Add(new Vector2Int(p.x, p.y - 1));
        if (p.y < depth - 1 && !cells[p.x, p.y + 1].visited) neighbors.Add(new Vector2Int(p.x, p.y + 1));
        return neighbors;
    }

    void RemoveWalls(Vector2Int a, Vector2Int b, MazeCell[,] cells)
    {
        if (a.x < b.x) { cells[a.x, a.y].eastWall = false; cells[b.x, b.y].westWall = false; }
        else if (a.x > b.x) { cells[a.x, a.y].westWall = false; cells[b.x, b.y].eastWall = false; }
        else if (a.y < b.y) { cells[a.x, a.y].northWall = false; cells[b.x, b.y].southWall = false; }
        else if (a.y > b.y) { cells[a.x, a.y].southWall = false; cells[b.x, b.y].northWall = false; }
    }
}