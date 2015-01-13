using UnityEngine;
using System.Collections;

public class AboveGround : MonoBehaviour
{

	public GameObject head;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
		CommonVariables.mappedPosition -= (Time.deltaTime * 1.5F) * transform.up;

		RaycastHit hit;
		int layerMask = 1 << LayerMask.NameToLayer ("Ground");
		if(Physics.Raycast(head.transform.position, -(head.transform.parent.up), out hit, head.transform.localPosition.y, layerMask))
		{
			CommonVariables.mappedPosition += ((head.transform.localPosition.y - hit.distance) * head.transform.parent.up);
			//Debug.Log ("AboveGround " + hit.distance);
		}
	}
}
