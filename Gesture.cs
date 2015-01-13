using UnityEngine;
using System.Collections;

//To Be put on the physical space because uses that for up
public class Gesture : MonoBehaviour 
{
	public static bool StopWasLastGesture  = false;

	[Header("GameObjects")]
	public string GloveName = "RightGlove";
	public string HMD = "RiftDK1";

	[Header("GestureThreshholds")]
	public float StopBelowHeadThresh = 0.3F;
	public float DirectionDotThresh = 0.25F;
	
	//times for the stop Gesture
	[Header("TimerThreshholds")]
	public float stopTimerThresh = 2.0F;
	private float stopTimer = 0;

	//Times for the forward Gesture
	public float forwardTimerThresh = 0.5F;
	private float forwardTimer = 0;

	//Times for follow Gesture
	public float followTimerThresh = 0.5F;
	private float followTimer = 0;

	//Times for a Gesture ending
	public float endTimerThresh = 1.0F;
	private float endTimer = 0;

	GameObject gloveObj;
	GameObject hmd;

	void Start ()
	{
		gloveObj = GameObject.Find (GloveName);
		hmd = GameObject.Find (HMD);
	}
	
	// Update is called once per frame
	void Update ()
	{
		checkForGesture ();							
	}

	void checkForGesture()
	{
		// to check if hand lies between shoulder and head in Y axis
		if (Vector3.Dot (gloveObj.transform.forward, hmd.transform.up) > 1 - DirectionDotThresh && (gloveObj.transform.position.y <= hmd.transform.position.y && gloveObj.transform.position.y > (hmd.transform.position.y - StopBelowHeadThresh)))
		{
			stopTimer += Time.deltaTime;
			if (stopTimer > stopTimerThresh) 
			{
				CommonVariables.StopGestureListener();
				StopWasLastGesture = true;
			}
		}

		// check for forward pointing
		else if (StopWasLastGesture && Vector3.Dot (gloveObj.transform.forward, transform.up) > -DirectionDotThresh && Vector3.Dot (gloveObj.transform.forward, transform.up) < DirectionDotThresh) 
		{ 	
			forwardTimer += Time.deltaTime;
			if (forwardTimer > forwardTimerThresh) 
			{
				forwardTimer = 0;
				StopWasLastGesture = false;

				Vector3 dir = new Vector3(gloveObj.transform.forward.x, 0, gloveObj.transform.forward.z);

				RaycastHit hit;
				if(Physics.Raycast(hmd.transform.position, dir, out hit, 20F))
				{
					CommonVariables.Pointing = hit.point;
				}
				else
				{
					CommonVariables.Pointing = hmd.transform.position + 20F * dir;
				}
			}
		}

		else if (StopWasLastGesture && Vector3.Dot (gloveObj.transform.forward, hmd.transform.up) < -1 + DirectionDotThresh) 
		{
			followTimer += Time.deltaTime;
			if (followTimer > followTimerThresh) 
			{
				followTimer = 0;
				StopWasLastGesture = false;

				CommonVariables.FollowGestureListener();
			}
		}
		
		else if (!StopWasLastGesture)
		{
			stopTimer = 0F;
			forwardTimer = 0F;
			followTimer = 0F;
		}
		
		else
		{
			endTimer += Time.deltaTime;
			if(endTimer > endTimerThresh)
			{
				StopWasLastGesture = false;
				stopTimer = 0F;
				forwardTimer = 0F;
				endTimer = 0F;
			}
		}
	}
}