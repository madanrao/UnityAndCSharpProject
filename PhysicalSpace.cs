using UnityEngine;
using System.Collections;

// This class provides a method for translating the user to a different virtual location
// Directions:
// 		1. Attach to the GameObject representing the User
public class PhysicalSpace : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		// Map rotation and position of user to virtual world
		transform.localPosition = CommonVariables.mappedPosition;
		transform.localRotation = Quaternion.Euler(CommonVariables.mappedRotation);
			
	}
}
