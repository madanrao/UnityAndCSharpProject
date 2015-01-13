using UnityEngine;
using System.Collections;

// This class provides a simple virtual hand technique
// Directions:
// 		1. Attach to the GameObject that will be used for collision detection
//		2. Ensure that GameObject is encapsulated in an empty GameObject with a clean transform
//		3. Change WiimoteName to the name of the Wiimote that will be used for input
public class VirtualHand : MonoBehaviour {

	public string WiimoteName = "RightWiimote";

	private GameObject collidedObject = null;
	private GameObject grabbedObject = null;
	private Transform rootObject = null;
	private Vector3 grabbedPosition;
	private Quaternion grabbedRotation;

	// Use this for initialization
	void Start () {

		// We assume the attached GameObject has a collider so ensure the collider is trigger-based
		collider.isTrigger = true;
		// Turn off the object's physics by making it kinematic
		rigidbody.isKinematic = true;

	}
	
	// Update is called once per frame
	void Update () {

		// Check if no object is being collided and if no object is grabbed
		if(collidedObject == null && grabbedObject == null) {
			// If A and B are pressed, treat the virtual hand as a fist by turning off the collider's trigger
			if(InputBroker.GetKeyDown(WiimoteName + ":A") && InputBroker.GetKeyDown(WiimoteName + ":B")) {
				collider.isTrigger = false;
			}
			// Otherwise, ensure the collider is treated as a trigger
			else {
				collider.isTrigger = true;
			}
		}

		// Check if an object is being collided (but no object is grabbed yet)
		else if(collidedObject != null && grabbedObject == null) {
			// If A and B are pressed, grab the object, turn off its physics, and add it to the virtual hand's empty parent
			if(InputBroker.GetKeyDown(WiimoteName + ":A") && InputBroker.GetKeyDown(WiimoteName + ":B")) {

				// Turn off physics by turning on kinematics
				grabbedObject = collidedObject;
				grabbedObject.rigidbody.isKinematic = true;

				// Find the root of the grabbed object (for hierarchical objects)
				rootObject = grabbedObject.transform;
				while(rootObject.transform.parent != null) {
					rootObject = rootObject.transform.parent;
				}

				// Move the root of the grabbed object under the virtual hand's parent
				rootObject.parent = transform.parent;

				// Determine the root's initial position and rotation relative to the virtual hand's parent				
				grabbedPosition = rootObject.localPosition;
				grabbedRotation = rootObject.localRotation;
			}
		}

		// Check if an object is grabbed
		else if(grabbedObject != null) {
			// Update the root's position and rotation relative to the virtual hand's parent
			rootObject.localPosition = grabbedPosition;
			rootObject.localRotation = grabbedRotation;

			// If A and B are NOT pressed, turn the object's physics back on and release it
			if(!InputBroker.GetKeyDown(WiimoteName + ":A") || !InputBroker.GetKeyDown(WiimoteName + ":B")) {
				grabbedObject.rigidbody.isKinematic = false;
				rootObject.parent = null;
				grabbedObject = null;
			}
		}
	}

	// Trigger function for collisions
	void OnTriggerEnter(Collider other) {
		// If an object is not already grabbed, check for collisions with another
		if(grabbedObject == null) {
			collidedObject = other.gameObject;
		}
	}

	// Trigger function for exiting collisions
	void OnTriggerExit(Collider other) {
		// If an object is not grabbed, forget the collided object
		if(grabbedObject == null) {
			collidedObject = null;
		}
	}
}
