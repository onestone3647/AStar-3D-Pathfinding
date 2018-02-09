using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
	public LayerMask unwalkableMask;
	public LayerMask walkableMask;
	public Vector3 gridWorldSize = new Vector3(10f, 10f, 10f);
	public Node[,] grid;
	
	public bool displayGridGizmos;

	//연결 크기
	public float stepOffset = 0.1f;
	//노드 반지름
	public float nodeRadius = 0.5f;

	//노드 지름
	float nodeDiameter;
	int gridSizeX, gridSizeZ;

	public static Grid instence;

	void Awake()
	{
		instence = this;

		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);
		CreateGrid();
	}

	public int MaxSize
	{
		get
		{
			return gridSizeX * gridSizeZ;
		}
	}

	void CreateGrid()
	{
		grid = new Node[gridSizeX, gridSizeZ];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.z / 2;

		//노드 생성
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int z = 0; z < gridSizeZ; z++)
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (z * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
				grid[x, z] = new Node(walkable, worldPoint, x, z);
			}
		}

		//Raycast terrain
		foreach (Node n in grid)
		{
			Ray ray = new Ray(n.nodeWorldPosition + new Vector3(0, gridWorldSize.y, 0), Vector3.down);
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(ray, out hit, gridWorldSize.y * 2, walkableMask))
			{
				n.nodeWorldPosition = hit.point + new Vector3(0, 0.1f, 0);
			}
		}

		//Create connection
		for (int x = 0; x < gridSizeX; x++)
		{
			for (int z = 0; z < gridSizeZ; z++)
			{

				if (x + 1 < gridSizeX)
					grid[x, z].connections[0] = new Connection(true, grid[x, z].nodeWorldPosition, grid[x + 1, z].nodeWorldPosition);
				if (z + 1 < gridSizeZ)
					grid[x, z].connections[1] = new Connection(true, grid[x, z].nodeWorldPosition, grid[x, z + 1].nodeWorldPosition);
				if (x + 1 < gridSizeX && z + 1 < gridSizeZ)
					grid[x, z].connections[2] = new Connection(true, grid[x, z].nodeWorldPosition, grid[x + 1, z + 1].nodeWorldPosition);
				if (x + 1 < gridSizeX && z - 1 >= 0)
					grid[x, z].connections[3] = new Connection(true, grid[x, z].nodeWorldPosition, grid[x + 1, z - 1].nodeWorldPosition);
			}
		}

		//Remove invalid Connection
		foreach (Node n in grid)
		{
			foreach (Connection c in n.connections)
			{
				if (c != null)
				{
					if (Mathf.Abs(c.start.y - c.end.y) > stepOffset)
					{
						c.valid = false;
					}

				}

			}
		}
	}	

	public List<Node> GetNeighbours(Node node)
	{
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++)
		{
			for (int z = -1; z <= 1; z++)
			{
				if (x == 0 && z == 0)
					continue;

				int checkX = node.gridX + x;
				int checkZ = node.gridZ + z;

				if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ)
				{
					neighbours.Add(grid[checkX, checkZ]);
				}
			}
		}

		return neighbours;
	}


	public Node NodeFromWorldPoint(Vector3 worldPosition)
	{
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentZ = (worldPosition.z + gridWorldSize.z / 2) / gridWorldSize.z;
		percentX = Mathf.Clamp01(percentX);
		percentZ = Mathf.Clamp01(percentZ);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);
		return grid[x, z];
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.black;
		Gizmos.DrawWireCube(transform.position, gridWorldSize);

		if (grid != null && displayGridGizmos)
		{
			foreach (Node n in grid)
			{
				Gizmos.color = (n.walkable) ? Color.yellow : Color.red;
				//Gizmos.DrawCube(n.nodeWorldPosition, Vector3.one * nodeDiameter / 8f);
				Gizmos.DrawCube(n.nodeWorldPosition, Vector3.one * (nodeDiameter - 0.6f));
				
				foreach (Connection c in n.connections)
				{
					if (c != null && c.valid)
					{
						Gizmos.color = (c.walkable) ? Color.white : Color.red;
						Gizmos.DrawLine(c.start, c.end);
					}
				}

			}
		}
	}
}