using UnityEngine;
using System.Collections;

//Combat 2 is no-shoot state
//Color is Yellow

public class EnemyCombatNoShoot : EnemyAIBehaviour
{
	float SeeTimer = 0F;
	float NotSeeTimer = 0F;

	public EnemyCombatNoShoot(EnemyAI ai) : base(ai)
	{
		SeeTimer = 0F;
		NotSeeTimer = 0F;
		AI.renderer.material.color = Color.yellow;
	}
	
	public override EnemyAIBehaviour Update()
	{
		if(!AI.animation.IsPlaying("StandingAim"))
		{
			AI.animation.Play ("StandingAim");
		}
		bool CanSee = false;
		//Check raycast to Player
		foreach(GameObject part in CommonVariables.PlayerParts)
		{
			RaycastHit hit;
			if(Physics.Raycast(AI.sightOrigin.transform.position, part.transform.position - AI.sightOrigin.transform.position, out hit, AI.sightRange))
			{
				if(hit.collider.gameObject == part)
				{
					CanSee = true;
					//Just be aware this triggers for each body part
					SeeTimer += Time.deltaTime;
					AI.transform.LookAt(part.transform.position);
					Quaternion newRot = AI.transform.rotation;
					newRot.x = 0;
					newRot.z = 0;
					AI.transform.rotation = newRot;
				}
			}
		}

		//I have been able to see the player for a total of 2 seconds
		if(SeeTimer >= 2.0F)
		{
			return new EnemyCombatShoot(AI);
		}

		//I can't see
		if(!CanSee)
		{
			NotSeeTimer += Time.deltaTime;
			//I haven't been able to see for a total of 5 seconds
			if(NotSeeTimer >= 5.0F)
			{
				return new EnemyIdle(AI);
			}
		}

		//Any other update logic can go here too


		
		//Don't transition
		return this;
	}
	
	public override EnemyAIBehaviour OnDamage(DamageObject dmg)
	{
		//Do I need to do anything if I get hit and am Aware? Maybe run away?
				
		//Don't transition
		return this;
	}
}