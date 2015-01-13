using UnityEngine;
using System.Collections;

// This class provides a means of using global variables among other scripts
using System.Collections.Generic;
using System;


public class CommonVariables
{

	// Positional mapping between the physical space and the virtual world
	static private Vector3 _mappedPosition = new Vector3 (0f, 0f, 0f);
	static public Vector3 mappedPosition
	{
		get
		{
			return _mappedPosition;
		}
		set
		{
			Vector3 move = value - _mappedPosition;
			foreach(GameObject go in PlayerParts)
			{
				if(!((Tracker)go.GetComponent<Tracker>()).canMove(move))
				{
					return;
				}
			}
			_mappedPosition = value;
		}
	}
	
	// Rotational mapping between the physical space and the virtual world
	static public Vector3 mappedRotation = new Vector3(0f, 0f, 0f);
	
	// Interpupillary distance between the eyes
	static public float dynamicIPD = 0.064f;

	static public List<GameObject> PlayerParts = new List<GameObject>();

	static public GameObject HMD;

	static private Vector3 pointing = Vector3.zero;

	static public Vector3 Pointing
	{
		get
		{
			return pointing;
		}
		set
		{
			pointing = value;
			PointGestureListener();
		}
	}

	static public Action StopGestureListener;

	static public Action PointGestureListener;

	static public Action FollowGestureListener;

	static public List<GameObject> Enemies = new List<GameObject>();
}