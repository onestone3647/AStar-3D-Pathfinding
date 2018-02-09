using UnityEngine;
using System.Collections;

public class Connection
{

	public Vector3 start;
	public Vector3 end;


	public bool walkable; //walkable or not
	public bool valid = true; //if it respect grid restriction

	public Connection(bool _walkable, Vector3 _start, Vector3 _end)
	{
		walkable = _walkable;
		start = _start;
		end = _end;
	}
}