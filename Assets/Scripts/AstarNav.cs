using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarNav : MonoBehaviour
{
    private Node[][] masterGrid;
    private List<Node> openNodes;
    private List<Node> closedNodes;
    private Node currentNode;
    private Node goalNode;
    private Node maxNode;
    private List<Node> neighbors;
    private int newCost;

    // Start is called before the first frame update
    void Start()
    {
        maxNode = new Node(-1, -1, 1000);
        // Instantiate base difficulty value of 1 to each cell
        masterGrid = new Node[200][];
        for (int i = 0; i < masterGrid.Length; i++) 
        {
            masterGrid[i] = new Node[200];
            for (int j = 0; j < masterGrid[i].Length; j++)
            {
                masterGrid[i][j] = new Node(i, j);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addToGrid(int costTerrain, Vector2Int pos)
    {
        masterGrid[pos.x][pos.y] = new Node(pos.x, pos.y, costTerrain);
        //Debug.Log("Item added to grid at " + pos.x + " " + pos.y + " with the cost of " + masterGrid[pos.x][pos.y].terrainCost);
    }

    public void removeFromGrid(Vector2Int pos)
    {
        masterGrid[pos.x][pos.y] = new Node(pos.x, pos.y);
        //Debug.Log("Item removed from grid at " + pos.x + " " + pos.y);
    }

    public int findDistance(Vector2Int source, Vector2Int des)
    {
        return Mathf.Abs(source.x - des.x) + Mathf.Abs(source.y - des.y);
    }

    public bool isValid(int x, int y)
    {
        if (x >= 0 && x < masterGrid.Length)
        {
            if (y >= 0 && y < masterGrid[x].Length)
            {
                if (masterGrid[x][y].terrainCost != -1) return true; // space is not valid if it's a wall tile
            }
        }
        return false;
    }

    public Node findPath(Vector2Int source, Vector2Int des)
    {
        openNodes = new List<Node>();
        closedNodes = new List<Node>();
        neighbors = new List<Node>();
        currentNode = masterGrid[source.x][source.y];
        currentNode.g = 0;
        currentNode.h = findDistance(source, des);
        currentNode.f = currentNode.h;
        openNodes.Add(currentNode);
        goalNode = masterGrid[des.x][des.y];

        //Debug.Log("Searching for a path...");
        while (openNodes.Count != 0)
        {
            //Debug.Log("testing " + openNodes.Count + " nodes in OPEN list");
            currentNode = maxNode;
            foreach (Node testNode in openNodes)
            {
                testNode.f = testNode.g + testNode.h;
                if (testNode.f < currentNode.f) currentNode = testNode;
            }

            if (currentNode.getCoord().Equals(des)) break;
            //Debug.Log("exploring neighbors of current node: " + currentNode.getCoord().ToString());
            
            neighbors.Clear();
            if (isValid(currentNode.x + 1, currentNode.y)) neighbors.Add(masterGrid[currentNode.x + 1][currentNode.y]);
            if (isValid(currentNode.x - 1, currentNode.y)) neighbors.Add(masterGrid[currentNode.x - 1][currentNode.y]);
            if (isValid(currentNode.x, currentNode.y + 1)) neighbors.Add(masterGrid[currentNode.x][currentNode.y + 1]);
            if (isValid(currentNode.x, currentNode.y - 1)) neighbors.Add(masterGrid[currentNode.x][currentNode.y - 1]);

            foreach(Node neighbor in neighbors)
            {
                newCost = currentNode.g + neighbor.terrainCost;
                if (openNodes.Contains(neighbor))
                {
                    if (neighbor.g <= newCost) continue;
                }
                else if (closedNodes.Contains(neighbor))
                {
                    if (neighbor.g <= newCost) continue;
                }
                else
                {
                    openNodes.Add(neighbor);
                    neighbor.h = findDistance(neighbor.getCoord(), des);
                    //Debug.Log("adding neighbor to open list with values of g=" + newCost + " and h=" + neighbor.h + " located at " + neighbor.getCoord().ToString());
                }
                neighbor.setParent(currentNode);
                neighbor.g = newCost;
            }
            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);
        }
        if (!currentNode.getCoord().Equals(des)) return null;
        Debug.Log("Found path from " + source.ToString() + " to " + des.ToString() + " using this path: " + currentNode.ToString());
        return currentNode;
    }

    public void resetGrid()
    {
        for (int i = 0; i < masterGrid.Length; i++)
        {
            for (int j = 0; j < masterGrid[i].Length; j++)
            {
                masterGrid[i][j] = new Node(i, j);
            }
        }
    }
}

public class Node
{
    public int x;
    public int y;
    public int f;
    public int g;
    public int h;
    public int terrainCost;
    private Node parent = null;

    public Node(int newx, int newy, int newTerrainCost)
    {
        x = newx;
        y = newy;
        f = 999;
        h = 999;
        g = 999;
        terrainCost = newTerrainCost;
    }

    public Node()
    {
        x = 999;
        y = 999;
        f = 999;
        h = 999;
        g = 999;
        terrainCost = 1;
    }

    public Node(int newX, int newY)
    {
        x = newX;
        y = newY;
        f = 999;
        h = 999;
        g = 999;
        terrainCost = 1;
    }

    public Vector2Int getCoord()
    {
        return new Vector2Int(x, y);
    }

    public void setParent(Node newParent)
    {
        parent = newParent;
    }

    public Node getParent()
    {
        return parent;
    }

    override
    public string ToString()
    {
        string path = "";
        Node tmp = this;
        while (tmp.parent != null)
        {
            path += (" " + tmp.getCoord().ToString() + ", ");
            tmp = tmp.parent;
        }
        path += " " + tmp.getCoord().ToString();
        return path;
    }
}