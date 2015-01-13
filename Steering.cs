using UnityEngine;
using System.Collections;

// This class provides a 3D steering technique
// Directions:
// 		1. Attach to the GameObject that will be used to direct steering
//		2. Change WiimoteName to the name of the Wiimote that will be used for input
//		3. Change TravelSpeed if desired
//		4. Change RotationSpeed if desired
public class Steering : MonoBehaviour {
	
	public string WiimoteName = "RightWiimote";	
	public float TravelSpeed = 2f;
	public float RotationSpeed = 90f;
	public bool yLocked = true;
	private bool OneDown = false;
	private bool TwoDown = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

//		// Keep the player on one plane (MUST CHANGE FOR STAIRS)
//		if (yLocked) {
//			CommonVariables.mappedPosition.y = 0;
//		}

		// Up button moves forwards relative to the object's transformation
		if(InputBroker.GetKeyDown(WiimoteName + ":Up")) {
			CommonVariables.mappedPosition += transform.forward * TravelSpeed * Time.deltaTime;
		}

		// Down button moves backwards relative to the object's transformation
		if(InputBroker.GetKeyDown(WiimoteName + ":Down")) {
			CommonVariables.mappedPosition -= transform.forward * TravelSpeed * Time.deltaTime;
		}

		// Left button rotates the virtual world left (heading only) 
		if(InputBroker.GetKeyDown(WiimoteName + ":Left")) {
			CommonVariables.mappedRotation.y -= RotationSpeed * Time.deltaTime;
		}

		// Right button rotates the virtual world right (heading only) 
		if(InputBroker.GetKeyDown(WiimoteName + ":Right")) {
			CommonVariables.mappedRotation.y += RotationSpeed * Time.deltaTime;
		}

		// 1 button increases the travel speed
		if(InputBroker.GetKeyDown(WiimoteName + ":One")) {
			// Check for a click (not a continued press)
			if(!OneDown) {
				TravelSpeed *= 2.0f;
			}
			OneDown = true;
		}
		else {
			OneDown = false;
		}
		
		// 2 button decreases the travel speed
		if(InputBroker.GetKeyDown(WiimoteName + ":Two")) {
			// Check for a click (not a continued press)
			if(!TwoDown) {
				TravelSpeed /= 2.0f;
			}
			TwoDown = true;
		}
		else {
			TwoDown = false;
		}

	}
}
