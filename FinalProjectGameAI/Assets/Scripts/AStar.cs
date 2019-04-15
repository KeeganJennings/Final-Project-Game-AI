using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    public static PrioritQueue openList;
    public static HashSet<Node> closedList;

    private static float HEstimateCost(Node curNode, Node goalNode)
    {
        Vector3 vecCost = curNode.position - goalNode.position;
        return vecCost.magnitude;
    }

    public static ArrayList FindPath(Node start, Node goal)
    {
        openList = new PrioritQueue();
        openList.Push(start);

        start.nodeTotalCost = 0.0f;
        start.estimatedCost = HEstimateCost(start, goal);

        closedList = new HashSet<Node>();
        Node node = null;

        while (openList.Length != 0)
        {
            node = openList.First();
            if (node.position == goal.position)
            {
                return CalculatePath(node);
            }

            ArrayList neighbors = new ArrayList();

            GridManager.instance.GetNeighbors(node, neighbors);

            for (int i = 0; i < neighbors.Count; i++)
            {
                Node neighborNode = (Node)neighbors[i];
                if (!closedList.Contains(neighborNode))
                {
                    if (!neighborNode.bObstacle)
                    {
                        float cost = HEstimateCost(node, neighborNode);

                        float totalCost = node.nodeTotalCost + cost;
                        float neighborNodeEstCost = HEstimateCost(neighborNode, goal);

                        neighborNode.nodeTotalCost = totalCost;
                        neighborNode.parent = node;
                        neighborNode.estimatedCost = totalCost + neighborNodeEstCost;

                        if (!openList.Cointains(neighborNode))
                        {
                            openList.Push(neighborNode);
                        }
                    }
                }
            }
            closedList.Add(node);
            openList.Remove(node);
        }

        if (node.position != goal.position)
        {
            Debug.LogError("Goal Not Found");
            return null;
        }

        return CalculatePath(node);
    }

    private static ArrayList CalculatePath(Node node)
    {
        ArrayList list = new ArrayList();
        while (node != null)
        {
            list.Add(node);
            node = node.parent;
        }
        list.Reverse();
        return list;
    }
}
