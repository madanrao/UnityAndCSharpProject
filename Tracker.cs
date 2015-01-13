using UnityEngine;
using System.Collections;

// This class provides an easy method of controlling a GameObject with a Vicon tracker
// Directions:
// 		1. Attach to the GameObject that will be tracked
//		2. Change ViconName to the name of the Vicon object that is being tracked
public class Tracker : MonoBehaviour {
	
	public string ViconName = "none";
	private Vector3 LastFrame = Vector3.zero;
	private Vector3 CurrentFrame = Vector3.zero;
	private GameObject collidedObject = null;
	
	// Use this for initialization
	void Start ()
	{
		if(ViconName == "RiftDK1")
		{
			CommonVariables.HMD = gameObject;
		}
		CommonVariables.PlayerParts.Add (gameObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Set rotation and position of object to match Vicon tracker
		transform.localRotation = InputBroker.GetRotation(ViconName);
		Vector3 newLocalPos = InputBroker.GetPosition (ViconName);
		
		Vector3 oldPos = transform.position;
		Vector3 newPos = newLocalPos + CommonVariables.mappedPosition;
		
		RaycastHit hit;
		float mag = (newPos - oldPos).magnitude;
		Vector3 dirNorm = (newPos - oldPos).normalized;
		
		Debug.DrawRay(oldPos, dirNorm * mag, Color.blue, 0.1f);
		int layerMask = 1 << 11;
		
		if(Physics.Raycast(oldPos, dirNorm, out hit, mag, layerMask))//rigidbody.SweepTest(dirNorm, out hit, mag))
		{
			CommonVariables.mappedPosition += (-mag) * dirNorm;
			Debug.Log ("Hit");
		}
		else
		{
			transform.localPosition = newLocalPos;
		}
	}
	
	void LateUpdate()
	{
		
	}
	
	void PreventWallGhosting()
	{
		if(LastFrame != Vector3.zero && LastFrame != CurrentFrame && CurrentFrame != Vector3.zero)
		{
			
			Vector3 FrameDiff = CurrentFrame - LastFrame;
			RaycastHit hitInfo;
			//transform.position = LastFrame;
			/*if(rigidbody.SweepTest(Vector3.Normalize(FrameDiff), out hitInfo, FrameDiff.magnitude))
			{
				Vector3 Offset = -1 * (FrameDiff.magnitude - hitInfo.distance) * (Vector3.Normalize(FrameDiff));
				Debug.Log ("PreventWallGhosting " + ViconName + " " + Offset);
				CommonVariables.mappedPosition += Offset;
			}*/
			if(collidedObject != null)
			{
				Vector3 Offset = -1 * FrameDiff;
				Debug.Log ("PreventWallGhosting " + ViconName + " " + Offset);
				CommonVariables.mappedPosition += new Vector3(Offset.x, 0, Offset.z);
			}
			//transform.position = CurrentFrame;
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		// If an object is not already grabbed, check for collisions with another
		collidedObject = other.gameObject;
	}
	
	void OnTriggerExit(Collider other)
	{
		collidedObject = null;
	}

	public bool canMove(Vector3 Move)
	{
		int layerMask = 1 << 11;
		if(Physics.Raycast(transform.position, Move.normalized, Move.magnitude, layerMask))
		{
			return false;
		}
		else
		{
			return true;
		}
	}
}
