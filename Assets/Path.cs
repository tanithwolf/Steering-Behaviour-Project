using UnityEngine;
using System.Collections.Generic;

public class Path : MonoBehaviour {
	
	List<Vector3> waypoints = new List<Vector3>();
	public int next = 0;
	public bool looped = true;
	
	public void CreatePath()
	{
		for(int i = 0; i < 6; i++)
		{
			//waypoint system set up to fill list with empty gameobjects co-ordinates used for waypoint system
			waypoints.Add (GameObject.FindWithTag("Waypoint"+i).transform.position);
			//check to see that list is filling out correctly
			Debug.Log(waypoints.Count);
			Debug.Log(((GameObject.FindWithTag("Waypoint"+i)).transform.position));
		}
	}
	
	public Vector3 NextWaypoint()
	{
		return waypoints[next];
	}
	
	public bool IsLastCheckpoint()
	{
		return(next == waypoints.Count-1);
	}
	
	public void AdvanceWaypoint()
	{
		if(looped)
		{
			next = (next + 1) % waypoints.Count;
		}
		else
		{
			if(!IsLastCheckpoint())
			{
				next = next + 1;
			}
		}
	}
}
