using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>
{
	public Vector3 nodeWorldPosition;
	public Connection[] connections;
	public bool walkable;
	public int gridX;
	public int gridZ;

	public int gCost;
	public int hCost;
	public Node parent;
	int heapIndex;

	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridZ)
	{
		walkable = _walkable;
		nodeWorldPosition = _worldPos;
		connections = new Connection[4];
		gridX = _gridX;
		gridZ = _gridZ;
	}

	public int fCost
	{
		get
		{
			return gCost + hCost;
		}
	}

	public int HeapIndex
	{
		get
		{
			return heapIndex;
		}
		set
		{
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare)
	{
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if(compare == 0)
		{
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}