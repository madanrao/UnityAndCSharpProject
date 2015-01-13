using UnityEngine;
using System.Collections;

public class CS_Raycast_01 : MonoBehaviour {

	public string WiimoteName = "RightWiimote";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		Vector3 direction = transform.forward;
		RaycastHit hit;
		Vector3 origin = GameObject.Find("child").transform.position;

		if (Physics.Raycast(origin, direction, out hit))	{
			Debug.Log("Hit");

			// Input
			if (InputBroker.GetKeyDown(WiimoteName + ":A"))	{
				hit.transform.parent = transform;
			}
		}

	}
}
