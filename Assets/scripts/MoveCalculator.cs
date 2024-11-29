using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
//한글

public class Coord
{
    public int r, c;
    public bool blocked;
    public int startPoint;
    public int endPoint;
    public int dPoint;
    public int point;

    public Coord parent = null;
    public Coord(int r, int c, int startPoint = 0, int endPoint = 0, bool blocked = false)
    {
        this.r = r;
        this.c = c;
        this.blocked = blocked;
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        dPoint = 0;
        point = 0;
    }

    public void Setup()
    {
        point = startPoint + endPoint;
    }
}


public struct CalculatorScale
{
    public int sizeX;
    public int sizeY;
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
    public float distanceSize;
    public float height;

}

public class MoveCalculator
{
    int[] moveX = { 1, -1, 0, 0, 1, 1, -1, -1 };
    int[] moveY = { 0, 0, 1, -1, 1, -1, 1, -1 };
    static CalculatorScale calculatorScale = new CalculatorScale();
    public Coord result;
    Coord[,] coords;
    static bool[,] blockedAreas;

    public void Init()
    {
        coords = new Coord[calculatorScale.sizeY, calculatorScale.sizeX];
        for (int i = 0; i < calculatorScale.sizeY; i++)
        {
            for (int j = 0; j < calculatorScale.sizeX; j++)
            {
                coords[i, j] = new Coord(i, j);
            }
        }

        for (int i = 0; i < calculatorScale.sizeY; i++)
        {
            for (int j = 0; j < calculatorScale.sizeX; j++)
            {
                if (blockedAreas[i,j]) coords[i,j].blocked = true;
            }
        }

    }

    public static void CheckArea(CalculatorScale calculatorScale)
    {
        MoveCalculator.calculatorScale = calculatorScale;
        float maxX = calculatorScale.maxX;
        float minX = calculatorScale.minX;
        float maxY = calculatorScale.maxY;
        float minY = calculatorScale.minY;
        float distanceSize = calculatorScale.distanceSize;

        int calculateScaleX = (int)((maxX - minX) / distanceSize);
        int calculateScaleY = (int)((maxY - minY) / distanceSize);
        MoveCalculator.calculatorScale.sizeX = calculateScaleX;
        MoveCalculator.calculatorScale.sizeY = calculateScaleY;
        blockedAreas = new bool[calculateScaleY ,calculateScaleX];
        for (int i = 0; i < calculateScaleY; i++)
        {
            for (int j = 0; j < calculateScaleX; j++)
            {
                float r = minY + i * distanceSize;
                float c = minX + j * distanceSize;
                Vector3 worldPoint = (Vector3.right * (c) + Vector3.forward * (r));
                bool isBlock = Physics.CheckBox(worldPoint, new Vector3(distanceSize, distanceSize, distanceSize), Quaternion.Euler(0, 0, 0), LayerMask.GetMask("block"));
                bool isWall = Physics.CheckBox(worldPoint, new Vector3(distanceSize, distanceSize, distanceSize), Quaternion.Euler(0, 0, 0), LayerMask.GetMask("wall"));
                if (isBlock || isWall)
                {
                    blockedAreas[i, j] = true; 
                }
            }
        }
    }

    public Coord AStarAlgorithm(Vector3 startVector, Vector3 endVector) //(Coord start, Coord end)
    {
        
        int playerX = (int)((startVector.x - calculatorScale.minX) / calculatorScale.distanceSize);
        int playerY = (int)((startVector.z - calculatorScale.minY) / calculatorScale.distanceSize);
        int targetX = (int)((endVector.x - calculatorScale.minX) / calculatorScale.distanceSize);
        int targetY = (int)((endVector.z - calculatorScale.minY) / calculatorScale.distanceSize);

        bool playerValidCheck = ValidCheck(playerX, playerY);
        bool targetValidCheck = ValidCheck(targetX, targetY);
        if (!(playerValidCheck && targetValidCheck))
        {
            if (!playerValidCheck) Debug.Log("PlayerInvalid");
            if (!targetValidCheck) Debug.Log("targetInvalid");
            return null;
        }
     
        Coord start = coords[playerY, playerX];
        Coord end = coords[targetY, targetX];

        if (playerY == targetY && playerX == targetX)
        {
            Coord already = new Coord(100, 100);
       //     Debug.Log("alreadyEnd");
            return already;
        }
        //result = start;
        //start.blocked = true;

        if (end.blocked)
        {
        //    Debug.Log("alreadyblock");
            //Debug.Log("EndBlock");
            return null;
        }
      //  Debug.Log(startVector + " " + endVector);
        List<Coord> openList = new List<Coord>();
        List<Coord> closedList = new List<Coord>();
        openList.Add(start);
        while (openList.Count > 0)
        {
            Coord coord = null;
            if (openList.Count == 1)
            coord = openList[0];
            else
            {
                int min = 99999;
                Coord minCoord = null;
                for (int i = 0; i < openList.Count; i++)
                {
                    if(min > openList[i].point)
                    {
                        min = openList[i].point;
                        minCoord = openList[i];
                    }

                 
                }
                
                coord = minCoord;
                
            }
            if(coord == null) return null;
            // Debug.Log(coord.r + " " + coord.c);
            closedList.Add(coord);

            openList.Remove(coord);
           // Debug.Log($"{openList.Count} : {coord.r} : {coord.c}");

            List<Coord> coordSort = new List<Coord>();
            for (int i = 0; i < 8; i++)
            {
                int r = coord.r + moveY[i];
                int c = coord.c + moveX[i];
               
                if (ValidCheck(r, c))
                {
                 
                    if (closedList.Contains(coords[r, c]) == false && coords[r, c].blocked == false)
                    {
                        if (end.r == r && end.c == c)
                        {
                            end.parent = coord;
                          
                            return end;
                        }
                        int startPoint = coord.startPoint + GetDistance(coord, coords[r, c]);  
                        if (startPoint < coords[r, c].startPoint || !openList.Contains(coords[r, c]))
                        {
                            coords[r, c].startPoint = startPoint;
                            coords[r, c].endPoint = GetDistance(coords[r, c], end);
                            coords[r, c].Setup();
                            coords[r, c].parent = coord;
                            if (!openList.Contains(coords[r, c])) openList.Add(coords[r, c]);
                        }
                    }
                }
                
            }
        }

        Debug.Log("NotFound");
        return null;
    }
    int GetDistance(Coord nodeA, Coord nodeB)
    {
        int dstX = Mathf.Abs(nodeA.c - nodeB.c);
        int dstY = Mathf.Abs(nodeA.r - nodeB.r);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }


    int ReturnDiff(int diffX, int diffY)
    {
        if (diffX == diffY) return 14 * diffX;

        int s = diffY > diffX ? diffX : diffY;
        int b = diffY > diffX ? diffY : diffX;

        int p = s * 14;
        p += (b - s) * 10;


        return p;
    }

    bool ValidCheck(int r, int c)
    {
        if (r >= 0 && r < calculatorScale.sizeY && c >= 0 && c < calculatorScale.sizeX)
        {
            return true;
        }

        return false;
    }
}
