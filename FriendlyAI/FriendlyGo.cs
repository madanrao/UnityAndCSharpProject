using UnityEngine;
using System.Collections;

public class FriendlyGo : FriendlyAIBehaviour
{
	public Vector3 location;
	public float timer = 0F;
	public float timerThresh = 5F;

	Vector3 lastPos = Vector3.zero;

	public FriendlyGo (FriendlyAI ai) : base(ai)
	{
		AI.renderer.material.color = Color.green;
	}

	public FriendlyGo(FriendlyAI ai, Vector3 loc) : base(ai)
	{
		location = loc;
		AI.renderer.material.color = Color.green;

	}

	public override FriendlyAIBehaviour Update()
	{
		if(!AI.animation.IsPlaying("Walk"))
		{
			AI.animation.Play ("Walk");
		}

		timer += Time.deltaTime;
		Vector3 diff = (new Vector3 (location.x, 0, location.z) - new Vector3 (AI.transform.position.x, 0, AI.transform.position.z));
		if(diff.sqrMagnitude > 2.25) //basically mag > 1.5F
		{
			Vector3 dir = Vector3.Normalize(diff);
			AI.transform.position += (AI.speed * Time.deltaTime * dir);
		}

		Vector3 curPos;
		curPos = AI.transform.position;
		Quaternion dirRot = Quaternion.LookRotation(curPos - lastPos);
		dirRot.x = 0f;
		dirRot.z = 0f;
		AI.transform.rotation = dirRot;
		
		foreach(GameObject enemy in CommonVariables.Enemies)
		{
			RaycastHit hit;
			if(Physics.Raycast(AI.transform.position, enemy.transform.position - AI.transform.position, out hit, AI.sightDistance))
			{
				if(hit.collider.gameObject == enemy)
				{
					return new FriendlyCombat(AI);
				}
			}
		}

		if(timer > timerThresh)
		{
			return new FriendlyFollow(AI);
		}
		else
		{
			return this;
		}
	}
	
	public override FriendlyAIBehaviour OnDamage(DamageObject dmg)
	{
		if(timer > 1.5F)
		{
			return new FriendlyCombat(AI);
		}
		else
		{
			return this;
		}
	}
}
