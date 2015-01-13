using UnityEngine;
using System.Collections;

// This class provides a method for dynamically changing the IPD of the OculusRift
// Directions:
// 		1. Attach to any GameObject
//		2. Change WiimoteName to the name of the Wiimote that will be used for input
//		3. Change RateOfChange to the amount of space to change with each button press
public class DynamicIPD : MonoBehaviour {
	
	public string WiimoteName = "RightWiimote";	
	public float RateOfChange = 0.004f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		// Minus button decreases the IPD
		if(InputBroker.GetKeyDown(WiimoteName + ":Minus")) {
			CommonVariables.dynamicIPD -= RateOfChange * Time.deltaTime;
		}
		
		// Plus button increases the IPD
		if(InputBroker.GetKeyDown(WiimoteName + ":Plus")) {
			CommonVariables.dynamicIPD += RateOfChange * Time.deltaTime;
		}

	}
}
