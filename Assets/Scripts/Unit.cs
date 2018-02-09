using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	//public Transform target;
	//float speed = 20;
	//Vector3[] path;
	//int targetIndex;

	//private void Start()
	//{
	//	PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
	//}

	//public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
	//{
	//	if (pathSuccessful)
	//	{
	//		path = newPath;
	//		StopCoroutine("FloowPath");
	//		StartCoroutine("FloowPath");
	//	}
	//}

	//IEnumerator FollowPath()
	//{
	//	Vector3 currentWaypoint = path[0];

	//	while(true)
	//	{
	//		if(transform.position == currentWaypoint)
	//		{
	//			targetIndex++;
	//			if(targetIndex >= path.Length)
	//			{
	//				targetIndex = 0;
	//				path = new Vector3[0];
	//				yield break;
	//			}
	//			currentWaypoint = path[targetIndex];
	//		}

	//		transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
	//		yield return null;
	//	}
	//}

	//public void OnDrawGizmos()
	//{
	//	if(path != null)
	//	{
	//		for(int i = targetIndex; i < path.Length; i++)
	//		{
	//			Gizmos.color = Color.black;
	//			Gizmos.DrawCube(path[i], Vector3.one);

	//			if(i == targetIndex)
	//			{
	//				Gizmos.DrawLine(transform.position, path[i]);
	//			}
	//			else
	//			{
	//				Gizmos.DrawLine(path[i - 1], path[i]);
	//			}
	//		}
	//	}
	//}

	public Vector3 target;
	float speed = 5f;
	public Vector3[] path;
	int targetIndex;

	public bool stopMoving;
	private Grid grid;

	public enum MoveFSM
	{
		findPosition,
		recalculatePath,
		move,
		turnToFace,
		interact
	}

	public MoveFSM moveFSM;

	void Start()
	{
		grid = GameObject.FindGameObjectWithTag("A*").GetComponent<Grid>();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			GetInteraction();
		}
		MoveStates();
	}

	public void MoveStates()
	{
		switch (moveFSM)
		{
			case MoveFSM.findPosition:

				break;
			case MoveFSM.recalculatePath:
				{
					Node targetNode = grid.NodeFromWorldPoint(target);
					if (targetNode.walkable == false)
					{
						stopMoving = false;
						FindClosestWalkableNode(targetNode);
						moveFSM = MoveFSM.move;
					}
					else if (targetNode.walkable == true)
					{
						Debug.Log("target node is walkable");
						stopMoving = false;
						PathRequestManager.RequestPath(transform.position, target, OnPathFound);
						moveFSM = MoveFSM.move;
					}
				}
				break;
			case MoveFSM.move:
				Move();
				break;
			case MoveFSM.turnToFace:
				//TurnToFace();
				break;
			case MoveFSM.interact:
				// if(currentInteractable != null)
				//currentInteractable.GetComponent<Interactable>().Interact(this.gameObject);
				break;
		}
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
	{
		if (pathSuccessful)
		{
			path = newPath;
			targetIndex = 0;
			RemoveUnitFromUnitManagerMovingUnitsList();
			UnitManager.instance.movingUnits.Add(this.gameObject);
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
			moveFSM = MoveFSM.move;
		}
	}

	private void FindClosestWalkableNode(Node originalNode)
	{
		Node comparisonNode = grid.grid[0, 0];
		Node incrementedNode = originalNode;
		for (int x = 0; x < incrementedNode.gridX; x++)
		{
			// Debug.Log("x: " + incrementedNode.gridX + " incremented node - 1: " + (incrementedNode.gridX - 1));
			incrementedNode = grid.grid[incrementedNode.gridX - 1, incrementedNode.gridZ];

			if (incrementedNode.walkable == true)
			{
				comparisonNode = incrementedNode;
				target = comparisonNode.nodeWorldPosition;
				PathRequestManager.RequestPath(transform.position, target, OnPathFound);
				moveFSM = MoveFSM.move;
				break;
			}
		}

	}

	public void Move()
	{

	}


	IEnumerator FollowPath()
	{
		Vector3 currentWaypoint = path[0];
		while (true)
		{
			if (transform.position == currentWaypoint)
			{
				targetIndex++;
				if (targetIndex >= path.Length)
				{
					yield break;
				}
				else if (stopMoving == true)
				{
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}

			transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
			yield return null;

		}
	}

	private void GetInteraction()
	{
		Ray interactionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit interactionInfo;
		if (Physics.Raycast(interactionRay, out interactionInfo, Mathf.Infinity))
		{
			target = interactionInfo.point;
			RemoveUnitFromUnitManagerMovingUnitsList();
			PathRequestManager.RequestPath(transform.position, target, OnPathFound);

			//animator.SetFloat(speedId, 3f);
		}
	}

	private void RemoveUnitFromUnitManagerMovingUnitsList()
	{
		if (UnitManager.instance.movingUnits.Count > 0)
		{
			for (int i = 0; i < UnitManager.instance.movingUnits.Count; i++)
			{
				if (this.gameObject == UnitManager.instance.movingUnits[i])
				{
					UnitManager.instance.movingUnits.Remove(UnitManager.instance.movingUnits[i]);
				}
			}
		}
	}

	public void OnDrawGizmos()
	{
		if (path != null)
		{
			for (int i = targetIndex; i < path.Length; i++)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], Vector3.one);

				if (i == targetIndex)
				{
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else
				{
					Gizmos.DrawLine(path[i - 1], path[i]);
				}
			}
		}
	}
}
