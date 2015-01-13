using UnityEngine;
using System.Collections;

public class FriendlyCombat : FriendlyAIBehaviour
{
	public GameObject Target;
	public float falloutThresh = 2F;
	private float falloutTimer = 0F;
	public float shotThresh = 0.25F;
	private float shotTimer = 0F;

	ParticleSystem muzzleSpark;
	ParticleSystem muzzleBlast;

	public FriendlyCombat (FriendlyAI ai) : base(ai)
	{
		AI.renderer.material.color = Color.red;
	}

	public override FriendlyAIBehaviour Update()
	{
		if(!AI.animation.IsPlaying("StandingFire"))
		{
			AI.animation.Play ("StandingFire");
		}
		foreach(GameObject enemy in CommonVariables.Enemies)
		{
			RaycastHit hit;
			if(Physics.Raycast(AI.transform.position, enemy.transform.position - AI.transform.position, out hit, AI.sightDistance))
			{
				if(hit.collider.gameObject == enemy)
				{
					Target = enemy;
					break;
				}
			}
			Target = null;
		}
		shotTimer += Time.deltaTime;

		if(Target != null)
		{
			if(shotTimer > shotThresh)
			{
				Shooting(Target);
				shotTimer = 0F;
			}
			falloutTimer = 0F;
		}
		else
		{
			//Can't see any enemies
			falloutTimer += Time.deltaTime;
		}

		if(falloutTimer > falloutThresh)
		{
			return new FriendlyFollow(AI);
		}
		else
		{
			return this;
		}
	}

	public void Shooting(GameObject tar)
	{
		float ang1 = Random.Range (-10, 10);
		float ang2 = Random.Range (-10, 10);
		float ang3 = Random.Range (-10, 10);

		Vector3 trajectory = tar.transform.position - AI.GunEmission.transform.position;
		Vector3 dir = Quaternion.Euler(ang1, ang2, ang3) * trajectory;

		Quaternion dirRot = Quaternion.LookRotation(tar.transform.position - AI.GunEmission.transform.position);
		dirRot.x = 0f;
		dirRot.z = 0f;
		AI.transform.rotation = dirRot;

		RaycastHit hit;
		if (Physics.Raycast (AI.GunEmission.transform.position, dir, out hit, AI.gunRange))
		{
			if(hit.collider.gameObject == tar)
			{
				try
				{
					tar.SendMessage("OnDamage", new DamageObject(AI.damage));
					Debug.Log (tar.name + " was shot by friendly");
				}
				catch(UnityException ex)
				{
					Debug.Log (ex);
				}
			}
			//Debug.DrawLine(hit.point, AI.GunEmission.transform.position, Color.red, 1F);
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
	
	public override FriendlyAIBehaviour OnDamage(DamageObject dmg)
	{
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
