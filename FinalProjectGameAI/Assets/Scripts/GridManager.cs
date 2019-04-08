using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private static GridManager s_Instance = null;

    public static GridManager instance
    {
        get
        {
            if(s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(GridManager)) as GridManager;
                if(s_Instance == null)
                {
                    Debug.Log("Could Not Locate GridManager object. You have to have exactly one Gridmanager in the scene.");
                }
            }
            return s_Instance;
        }
    }

    public int numOfRows;
    public int numOfColums;
    public float gridCellSize;
    public bool showGrid = true;
    public bool showObstableBlocks = true;

    private Vector3 origin = new Vector3();
    private GameObject[] obstacleList;
    public Node[,] nodes { get; set; }
    public Vector3 Origin
    {
        get { return origin; }
    }

    private void Awake()
    {
        obstacleList = GameObject.FindGameObjectsWithTag("Wall");
        CalculateObstacles();
    }

    private void CalculateObstacles()
    {
        nodes = new Node[numOfColums, numOfRows];
        int index = 0;
        for(int i = 0; i < numOfColums; i++)
        {
            for(int j = 0; j < numOfRows; j++)
            {
                Vector3 cellPos = GetGridCellCenter(index);
                Node node = new Node(cellPos);
                nodes[i, j] = node;
                index++;
            }
        }
        if(obstacleList != null && obstacleList.Length > 0)
        {
            foreach(GameObject data in obstacleList)
            {
                int indexCell = GetGridIndex(data.transform.position);
                int col = GetColumn(indexCell);
                int row = GetRow(indexCell);
                nodes[row, col].MarkAsObstacle();
            }
        }
    }

    public Vector3 GetGridCellCenter(int index)
    {
        Vector3 cellPosition = GetGridCellPosition(index);
        cellPosition.x += (gridCellSize / 2.0f);
        cellPosition.z += (gridCellSize / 2.0f);
        return cellPosition;
    }

    public Vector3 GetGridCellPosition(int index)
    {
        int row = GetRow(index);
        int col = GetColumn(index);
        float xPosInGrid = col * gridCellSize;
        float zPosInGrid = row * gridCellSize;
        return Origin + new Vector3(xPosInGrid, 0.0f, zPosInGrid);
    }

    public int GetGridIndex(Vector3 pos)
    {
        if(!IsInBounds(pos))
        {
            return -1;
        }

        pos -= Origin;
        int col = (int)(pos.x / gridCellSize);
        int row = (int)(pos.z / gridCellSize);
        return (row * numOfColums + col);
    }

    public bool IsInBounds(Vector3 pos)
    {
        float width = numOfColums * gridCellSize;
        float height = numOfRows * gridCellSize;
        return (pos.x >= Origin.x && pos.z <= Origin.x + width && pos.x <= Origin.z + height && pos.z >= Origin.z);
    }

    public int GetRow(int index)
    {
        int row = index / numOfColums;
        return row;
    }

    public int GetColumn(int index)
    {
        int col = index % numOfColums;
        return col;
    }

    public void GetNeighbors(Node node, ArrayList neighbors)
    {
        Vector3 neighborPos = node.position;
        int neightborIndex = GetGridIndex(neighborPos);

        int row = GetRow(neightborIndex);
        int column = GetColumn(neightborIndex);

        int leftNodeRow = row - 1;
        int leftNodeColum = column;
        AssignNeighbor(leftNodeRow, leftNodeColum, neighbors);

        leftNodeRow = row + 1;
        leftNodeColum = column;
        AssignNeighbor(leftNodeRow, leftNodeColum, neighbors);

        leftNodeRow = row;
        leftNodeColum = column + 1;
        AssignNeighbor(leftNodeRow, leftNodeColum, neighbors);

        leftNodeRow = row;
        leftNodeColum = column - 1;
        AssignNeighbor(leftNodeRow, leftNodeColum, neighbors);

    }

    void AssignNeighbor(int row, int column, ArrayList neighbors)
    {
        if(row != -1 && column != 1 && row < numOfRows && column < numOfColums)
        {
            Node nodeToAdd = nodes[row, column];
            if(!nodeToAdd.bObstacle)
            {
                neighbors.Add(nodeToAdd);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(showGrid)
        {
            DebugDrawGrid(transform.position, numOfRows, numOfColums, gridCellSize, Color.blue);
        }
        Gizmos.DrawSphere(transform.position, 0.5f);
        if(showObstableBlocks)
        {
            Vector3 cellSize = new Vector3(gridCellSize, 1.0f, gridCellSize);
            if(obstacleList != null && obstacleList.Length > 0)
            {
                foreach(GameObject data in obstacleList)
                {
                    Gizmos.DrawCube(GetGridCellCenter(GetGridIndex(data.transform.position)), cellSize);
                }
            }
        }
    }

    public void DebugDrawGrid(Vector3 origin, int numRows, int numCols, float cellSize, Color color)
    {
        float width = (numCols * cellSize);
        float height = (numOfRows * cellSize);

        for(int i = 0; i < numRows + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 endPos = startPos + width * new Vector3(1.0f, 0.0f, 0.0f);
            Debug.DrawLine(startPos, endPos, color);
        }
        for(int i = 0; i < numCols + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 endPos = startPos + height * new Vector3(0.0f, 0.0f, 1.0f);
            Debug.DrawLine(startPos, endPos, color);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
