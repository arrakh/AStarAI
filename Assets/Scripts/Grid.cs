using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public Transform StartPosition;
    public LayerMask WallMask;
    public Vector2 vGridWorldSize;
    public float fNodeRadius;
    public float fDistanceBetweenNodes;
    public bool checkDiagonal = true;

    public bool drawGrid = true;
    public bool drawPath = false;

    Node[,] NodeArray;
    public List<Node> FinalPath;


    float fNodeDiameter;
    int iGridSizeX, iGridSizeY;


    private void Start()
    {
        fNodeDiameter = fNodeRadius * 2;
        iGridSizeX = Mathf.RoundToInt(vGridWorldSize.x / fNodeDiameter);
        iGridSizeY = Mathf.RoundToInt(vGridWorldSize.y / fNodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        NodeArray = new Node[iGridSizeX, iGridSizeY];
        Vector3 bottomLeft = transform.position - Vector3.right * vGridWorldSize.x / 2 - Vector3.forward * vGridWorldSize.y / 2;
        for (int x = 0; x < iGridSizeX; x++)
        {
            for (int y = 0; y < iGridSizeY; y++)
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * fNodeDiameter + fNodeRadius) + Vector3.forward * (y * fNodeDiameter + fNodeRadius);
                bool Wall = true;

                
                
                
                if (Physics.CheckSphere(worldPoint, fNodeRadius, WallMask))
                {
                    Wall = false;
                }

                NodeArray[x, y] = new Node(Wall, worldPoint, x, y);
            }
        }
    }

    
    public List<Node> GetNeighboringNodes(Node a_NeighborNode)
    {
        List<Node> NeighborList = new List<Node>();

        if (!checkDiagonal)
        {
            int icheckX;
            int icheckY;

            icheckX = a_NeighborNode.iGridX + 1;
            icheckY = a_NeighborNode.iGridY;
            if (icheckX >= 0 && icheckX < iGridSizeX)
            {
                if (icheckY >= 0 && icheckY < iGridSizeY)
                {
                    NeighborList.Add(NodeArray[icheckX, icheckY]);
                }
            }

            icheckX = a_NeighborNode.iGridX - 1;
            icheckY = a_NeighborNode.iGridY;
            if (icheckX >= 0 && icheckX < iGridSizeX)
            {
                if (icheckY >= 0 && icheckY < iGridSizeY)
                {
                    NeighborList.Add(NodeArray[icheckX, icheckY]);
                }
            }

            icheckX = a_NeighborNode.iGridX;
            icheckY = a_NeighborNode.iGridY + 1;
            if (icheckX >= 0 && icheckX < iGridSizeX)
            {
                if (icheckY >= 0 && icheckY < iGridSizeY)
                {
                    NeighborList.Add(NodeArray[icheckX, icheckY]);
                }
            }

            icheckX = a_NeighborNode.iGridX;
            icheckY = a_NeighborNode.iGridY - 1;
            if (icheckX >= 0 && icheckX < iGridSizeX)
            {
                if (icheckY >= 0 && icheckY < iGridSizeY)
                {
                    NeighborList.Add(NodeArray[icheckX, icheckY]);
                }
            }
        }
        else
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    //if we are on the node tha was passed in, skip this iteration.
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    int checkX = a_NeighborNode.iGridX + x;
                    int checkY = a_NeighborNode.iGridY + y;

                    //Make sure the node is within the grid.
                    if (checkX >= 0 && checkX < iGridSizeX && checkY >= 0 && checkY < iGridSizeY)
                    {
                        NeighborList.Add(NodeArray[checkX, checkY]); //Adds to the neighbours list.
                    }

                }
            }
        }

        return NeighborList;
    }

    
    public Node NodeFromWorldPoint(Vector3 a_vWorldPos)
    {
        float ixPos = ((a_vWorldPos.x + vGridWorldSize.x / 2) / vGridWorldSize.x);
        float iyPos = ((a_vWorldPos.z + vGridWorldSize.y / 2) / vGridWorldSize.y);

        ixPos = Mathf.Clamp01(ixPos);
        iyPos = Mathf.Clamp01(iyPos);

        int ix = Mathf.RoundToInt((iGridSizeX - 1) * ixPos);
        int iy = Mathf.RoundToInt((iGridSizeY - 1) * iyPos);

        return NodeArray[ix, iy];
    }


    
    private void OnDrawGizmos()
    {

        Gizmos.DrawWireCube(transform.position, new Vector3(vGridWorldSize.x, 1, vGridWorldSize.y));

        if (NodeArray != null)
        {
            if (drawGrid)
            {
                foreach (Node n in NodeArray)
                {
                    if (n.bIsWall)
                    {
                        Gizmos.color = Color.white;
                    }
                    else
                    {
                        Gizmos.color = Color.yellow;
                    }

                    Gizmos.DrawCube(n.vPosition, Vector3.one * (fNodeDiameter - fDistanceBetweenNodes));
                }
            }

            if (drawPath && FinalPath != null)
            {
                for (int i = 0; i < FinalPath.Count - 1; i++)
                {
                    var p1 = FinalPath[i].vPosition;
                    var p2 = FinalPath[i+1].vPosition;
                    Handles.DrawBezier(p1, p2, p1, p2, Color.red, null, 3f);
                }
            }
        }
    }
}