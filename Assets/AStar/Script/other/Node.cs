using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Node
{
    public Vector3 position;
    public bool isWalkable;
    public Node parent;
    public float gCost; // ���� ��忡�� ���� �������� ���
    public float hCost; // ���� ��忡�� ��ǥ �������� ���� ���
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
