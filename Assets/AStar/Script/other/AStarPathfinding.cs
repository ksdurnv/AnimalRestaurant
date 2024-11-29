using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    public Transform targetA;
    public Transform obstacleB;
    public Transform startC;
    public LayerMask obstacleLayer;
    public float nodeSize = 1.0f;
    public Vector2 gridWorldSize;

    private List<Node> path;

    void Start()
    {
        path = FindPath(startC.position, targetA.position);
    }

    void Update()
    {
        if (path != null && path.Count > 0)
        {
            Vector3 nextPosition = path[0].position;
            if (Vector3.Distance(transform.position, nextPosition) < 0.1f)
            {
                path.RemoveAt(0);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, nextPosition, Time.deltaTime * 5.0f);
            }
        }
    }

    List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = new Node(startPos, true);
        Node targetNode = new Node(targetPos, true);

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

            if (currentNode.position == targetNode.position)
            {
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                float newMovementCostToNeighbor = currentNode.gCost + Vector3.Distance(currentNode.position, neighbor.position);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = Vector3.Distance(neighbor.position, targetNode.position);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
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

    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                Vector3 neighborPos = new Vector3(node.position.x + x * nodeSize, node.position.y, node.position.z + y * nodeSize);
                bool walkable = !Physics.CheckSphere(neighborPos, nodeSize / 2, obstacleLayer);
                neighbors.Add(new Node(neighborPos, walkable));
            }
        }

        return neighbors;
    }
}
