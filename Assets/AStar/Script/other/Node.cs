using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Node
{
    public Vector3 position;
    public bool isWalkable;
    public Node parent;
    public float gCost; // 시작 노드에서 현재 노드까지의 비용
    public float hCost; // 현재 노드에서 목표 노드까지의 예상 비용
    public float fCost
    {
        get { return gCost + hCost; }
    }

    public Node(Vector3 pos, bool walkable)
    {
        position = pos;
        isWalkable = walkable;
    }
}
