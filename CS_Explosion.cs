using UnityEngine;
using System.Collections;

public class CS_Explosion : MonoBehaviour, Damagable {

	void Start () {
	
	}
	
	public void OnDamage (DamageObject dmg) {
		Debug.Log ("splode");
		GameObject hitBlast = (GameObject)Instantiate(Resources.Load ("explosion"));
		hitBlast.transform.position = transform.position;
		Destroy(this.gameObject);
	}
}
