using UnityEngine;
using System.Collections;

//Dead state
//Color is Black

public class EnemyDead : EnemyAIBehaviour
{

	bool fallen = false;

	public EnemyDead(EnemyAI ai) : base(ai)
	{
		if (!fallen)	{
			AI.renderer.material.color = Color.black;
			//AI.collider.enabled = false;
			AI.animation.Play ("Crouch");
			//AI.rigidbody.useGravity = true;
			AI.rigidbody.constraints = RigidbodyConstraints.None;
			AI.rigidbody.AddForce (Vector3.forward * 100);
			fallen = true;
		}
	}
	
	// Update is called once per frame
	public override EnemyAIBehaviour Update()
	{

		//Do nothing and return reference to self.
		return this;
	}

	public override EnemyAIBehaviour OnDamage(DamageObject dmg)
	{
		return this;
	}
}

