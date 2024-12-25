using System.Collections.Generic;
using UnityEngine;

public interface IPathfinding
{
    List<Node> FindPath(Vector3 startPos, Vector3 targetPos);
    
    bool IsClearPath(Vector2 a, Vector2 b);
    bool IsWalkable(Vector2 targetPosition);
    
    ICollection<Vector2> GetSpreadPosition(Vector2 position, int amount);
}

[RequireComponent(typeof(GridGenerator))]
public class Pathfinding : MonoBehaviour, IPathfinding
{
    private GridGenerator _grid;

    void Awake()
    {
        _grid = GetComponent<GridGenerator>();
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = _grid.NodeFromWorldPoint(startPos);
        Node targetNode = _grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbour in _grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        // Path not found
        return new List<Node>();
    }

    public bool IsClearPath(Vector2 a, Vector2 b)
    {
        // Calculate the direction from point a to point b
        Vector2 direction = b - a;
        float distance = direction.magnitude;

        // Perform the Raycast
        RaycastHit2D hit = Physics2D.Raycast(a, direction, distance, _grid.unwalkableMask);
        if (hit.collider != null)
        {
            // If a collider is hit on the specified layer, return false
            return false;
        }

        // If no collider is hit on the specified layer, return true
        return true;
    }

    public bool IsWalkable(Vector2 targetPosition)
    {
        Node node = _grid.NodeFromWorldPoint(targetPosition);
        return node.walkable;
    }

    public ICollection<Vector2> GetSpreadPosition(Vector2 position, int amount)
    {
        List<Vector2> spreadPositions = new List<Vector2>();
        float spacing = 1.5f; // Distance between points
        int maxAttempts = 100; // Maximum attempts to find positions
        int attempts = 0;

        while (spreadPositions.Count < amount && attempts < maxAttempts)
        {
            // Generate random offset in a circular area
            float angle = Random.Range(0, Mathf.PI * 2);
            float radius = Random.Range(0.5f, spacing * amount / Mathf.PI);

            Vector2 candidate = position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            // Check if the candidate position is walkable
            if (IsWalkable(candidate) && !spreadPositions.Contains(candidate))
            {
                spreadPositions.Add(candidate);
            }

            attempts++;
        }

        // If we couldn't find enough walkable positions, log a warning
        if (spreadPositions.Count < amount)
        {
            Debug.LogWarning($"Only found {spreadPositions.Count}/{amount} spread positions around {position}");
        }

        return spreadPositions;
    }



    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        return path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
