using UnityEngine;
using System.Collections;

//Combat is a shooting state
//Color is Red

public class EnemyCombatShoot : EnemyAIBehaviour
{
	private float shotTimer = 0F;
	public float shotTimerThresh = 0.25F;
	private float cantSeeTimer = 0F;
	public float cantSeeTimerThresh = 2;
	
	
	public EnemyCombatShoot(EnemyAI ai) : base(ai)
	{
		AI.speed = 4.13065F; 	//Running speed = 9.24 mph
		//AI.sightRange = 25F;
		AI.healthGainRate = 2;
		AI.renderer.material.color = Color.red;
	}
	
	public override EnemyAIBehaviour Update()
	{
		if(!AI.animation.IsPlaying("StandingFire"))
		{
			AI.animation.Play ("StandingFire");
		}
		bool CanSee = false;

		//cantSeeTimer keeps increasing if player is not in sight 
		cantSeeTimer = cantSeeTimer + Time.deltaTime;

		//Check raycast to Player
		foreach(GameObject part in CommonVariables.PlayerParts)
		{
			RaycastHit hit;
			if(Physics.Raycast(AI.sightOrigin.transform.position, part.transform.position - AI.sightOrigin.transform.position, out hit, AI.sightRange))
			{
				if(hit.collider.gameObject == part)
				{
					//I see the player, or at least part of him
					CanSee = true;

					//reset the cantSeeTimer if player is spotted
					cantSeeTimer = 0F;
				}
			}
		}

		//I can't see the player & haven't seen him/her for 2 seconds
		if(!CanSee && cantSeeTimer >= 2)
		{
			cantSeeTimer = 0F;
			//Drop to Combat2
			return new EnemyCombatNoShoot(AI);
		}



		//Run Towards the player
		//Removed for Alpha
		/*Vector3 direction = CommonVariables.HMD.transform.position - AI.gameObject.transform.position;
		direction.y = 0F;
		AI.gameObject.transform.position += AI.speed * Time.deltaTime * Vector3.Normalize(direction);*/

		//Shoot every shotTimerThresh seconds
		shotTimer += Time.deltaTime;
		if(shotTimer > shotTimerThresh)
		{
			Shoot(CommonVariables.HMD);
			shotTimer = 0F;
		}
		
		//Don't transition
		return this;
	}

	public void Shoot(GameObject tar)
	{
		float ang1 = Random.Range (-15, 15);
		float ang2 = Random.Range (-15, 15);
		float ang3 = Random.Range (-15, 15);
		
		Vector3 trajectory = tar.transform.position - AI.GunEmission.transform.position;
		Vector3 dir = Quaternion.Euler(ang1, ang2, ang3) * trajectory;

		AI.transform.LookAt(tar.transform.position);
		Quaternion newRot = AI.transform.rotation;
		newRot.x = 0;
		newRot.z = 0;
		AI.transform.rotation = newRot;
		
		RaycastHit hit;
		if (Physics.Raycast (AI.GunEmission.transform.position, dir, out hit, AI.gunRange))
		{
			if(hit.collider.gameObject == tar)
			{
				try
				{
					tar.SendMessage("OnDamage", new DamageObject(AI.damage));
					Debug.DrawLine(hit.point, AI.GunEmission.transform.position, Color.red, 1F);
					Debug.Log (tar.name + " was shot by friendly");
				}
				catch(UnityException ex)
				{
					Debug.Log (ex);
				}
			}
			//Debug.DrawLine(hit.point, AI.gunEmission.transform.position, Color.red, 1F);
			AI.lineRen.SetVertexCount(2);
			AI.lineRen.SetPosition(0, AI.GunEmission.transform.position);
			AI.lineRen.SetPosition(1, hit.point);
			AI.lineTimer = 0;

			// Muzzle blast
			AI.StartCoroutine(LightFlash());
			AI.GunEmission.audio.Play ();
			AI.muzzleBlast.Play ();
			AI.muzzleSpark.Play ();
		}
	}
	
	public override EnemyAIBehaviour OnDamage(DamageObject dmg)
	{
		//Don't transition
		return this;
	}

	IEnumerator LightFlash()	{
		if (!AI.blastLight.light.enabled)	{
			AI.blastLight.enabled = true;
			yield return new WaitForSeconds(0.1f);
			AI.blastLight.enabled = false;
		}
		yield return null;
	}
}