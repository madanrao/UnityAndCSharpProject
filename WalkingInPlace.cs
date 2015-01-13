using UnityEngine;
using System.Collections;

public class WalkingInPlace : MonoBehaviour {
	
	
	
	public float WalkThresh = 0.05F;
	public float JitterThresh = 0.005F;
	public bool BothAtSameTime = false;
	public float UpToForward = 1.5F;
	public float DownToForward = 1.5F;
	
	//[Header( "Left First")]
	public GameObject[] FeetToes = new GameObject[2];

	public GameObject[] Heels = new GameObject[2];

	private GameObject[] FeetPoints = new GameObject[4];
	
	public float[] GroundThresh = new float[4];

	private Vector3[] LastPos = new Vector3[4];
	
	private Vector2[] LastGroundLoc = new Vector2[4];
	
	public FootState[] FeetState = new FootState[2];

	private float[] yHeight = new float[2];

	private float[] yDeltaDist = new float[2];
	
	public bool IsCalibrated = false;
	
	void Awake ()
	{
		FeetPoints[0] = Heels[0];
		FeetPoints[1] = FeetToes[0];
		FeetPoints[2] = Heels[1];
		FeetPoints[3] = FeetToes[1];

		FeetState [0] = FootState.NA;
		FeetState [1] = FootState.NA;
	}
	
	// Use this for initialization
	void Start ()
	{

	}

	Vector3 GetLocalPosition(int i)
	{
		return (FeetPoints[i].transform.position - CommonVariables.mappedPosition);
	}
	
	void Calibrate()
	{
		for(int i = 0; i < 4; i++)
		{
			GroundThresh[i] = GetLocalPosition (i).y + 0.005F;
			LastPos[i] = GetLocalPosition(i);
			LastGroundLoc[i] = new Vector2 (LastPos[i].x, LastPos[i].z);
			FeetState[i / 2] = FootState.NA;
		}
	}
	
	bool ValidData()
	{
		if(FeetToes[0].transform.localPosition.x != 0F)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!IsCalibrated && ValidData())
		{
			Calibrate();
			IsCalibrated = true;
		}
		
		if(IsCalibrated)
		{
			UpdateFootState ();
			FixYDists ();
			if(CheckWalkingInPlace())
			{
				float Forward = 0F;
				if(BothAtSameTime)
				{
					for(int i = 0; i < 2; i++)
					{
						if(FeetState[i] == FootState.Up)
						{
							Forward += yDeltaDist[i] * UpToForward;
						}
						else if(FeetState[i] == FootState.Down)
						{
							Forward += yDeltaDist[i] * DownToForward;
						}
					}
				}
				else
				{
					int num = 0;
					if(yDeltaDist[1] > yDeltaDist[0])
					{
						num = 1;
					}
					
					if(FeetState[num] == FootState.Up)
					{
						Forward = yDeltaDist[num] * UpToForward;
					}
					else if(FeetState[num] == FootState.Down)
					{
						Forward = yDeltaDist[num] * DownToForward;
					}
				}
				Vector3 movement = Forward * Vector3.Slerp(FeetToes[0].transform.forward, FeetToes[1].transform.forward, 0.5F);
				movement.y = 0F;
				CommonVariables.mappedPosition += movement;
			}
			for(int i = 0; i < 4; i++)
				LastPos[i] = GetLocalPosition(i);
		}
	}
	
	private bool CheckWalkingInPlace ()
	{
		//If a foot is off the ground
		if(FeetState[0] == FootState.Down || FeetState[0] == FootState.Up ||
		   FeetState[1] == FootState.Down || FeetState[1] == FootState.Up)
		{
			for(int i = 0; i < 4; i++)
			{
				Vector2 pos2d = new Vector2 (GetLocalPosition(i).x, GetLocalPosition(i).z);
				if((pos2d - LastGroundLoc[i]).magnitude > WalkThresh)
				{
					//One foot point is out of the threshhold
					return false;
				}
			}
			return true;
		}
		else
		{
			return false;
		}
	}
	
	private void UpdateFootState()
	{
		for(int i = 0; i < 4; i+=2)
		{
			//If either heel or toe is on the ground
			if(GetLocalPosition(i).y < GroundThresh[i] || GetLocalPosition(i + 1).y < GroundThresh[i + 1])
			{
				FeetState[i / 2] = FootState.Ground;
				yDeltaDist[i / 2] = 0F;
				LastGroundLoc[i] = new Vector2 (GetLocalPosition(i).x, GetLocalPosition(i).z);
				LastGroundLoc[i + 1] = new Vector2 (GetLocalPosition(i + 1).x, GetLocalPosition(i + 1).z);
			}
			else
			{
				yHeight[i / 2] = (GetLocalPosition(i).y + GetLocalPosition(i + 1).y) / 2;
				float LastYHeight = (LastPos[i].y + LastPos[i + 1].y) / 2;

				if(yHeight[i / 2] >= LastYHeight)
				{
					if(FeetState[i / 2] == FootState.Ground)
					{
						//Moved Up
						FeetState[i / 2] = FootState.Up;
					}
				}
				else if(FeetState[i / 2] == FootState.Up)
				{
					//Moved Down
					FeetState[i / 2] = FootState.Down;
				}
				yDeltaDist[i / 2] = yHeight[i / 2] - LastYHeight;
			}
		}
	}
	
	private void FixYDists()
	{
		for(int i = 0; i < 2; i++)
		{
			if(FeetState[i] == FootState.Up && yDeltaDist[i] < 0)
			{
				yDeltaDist[i] = 0;
			}
			else if(FeetState[i] == FootState.Down && yDeltaDist[i] > 0)
			{
				yDeltaDist[i] = 0;
			}
			
			//Play with this formula
			//Was originally yDist[i] = Mathf.Abs (yDist[i]);
			yDeltaDist[i] = Mathf.Pow((Mathf.Abs (yDeltaDist[i]) + 1), 2) - 1;

			if(yDeltaDist[i] < JitterThresh)
				yDeltaDist[i] = 0;
		}
	}
}
