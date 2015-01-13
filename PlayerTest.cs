using UnityEngine;
using System.Collections;

public class PlayerTest : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		CommonVariables.HMD = gameObject;
		CommonVariables.PlayerParts.Add (gameObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
