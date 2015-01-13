using UnityEngine;
using System.Collections;

public class CS_Door_Open_01 : MonoBehaviour {

	public GameObject door;

	private bool open = false;

	void Start () {
	
	}
	
	void OnTriggerEnter (Collider other) {
		if ((other.tag == "PhysicalWallPrevention" || other.tag == "Player") && !open) {
			door.animation.Play ();
			open = true;
		}
	}
}
