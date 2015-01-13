using UnityEngine;
using System.Collections;

public enum FootState
{
	Ground = 0,
	Up = 1,
	Down = 2,
	NA = 3
};

public class Enums : MonoBehaviour {
	
	FootState LeftFoot = FootState.Down;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
