using UnityEngine;
using System.Collections;


public class FriendlyFollow : FriendlyAIBehaviour
{
	Vector3 lastPos = Vector3.zero;
	bool isMoving = false;
	public FriendlyFollow (FriendlyAI ai) : base(ai)
	{
		AI.renderer.material.color = Color.blue;
	}
	
	public override FriendlyAIBehaviour Update()
	{
		Vector3 curPos;
		curPos = AI.transform.position;
		if (Vector3.Distance(curPos, lastPos) < 0.01f)
		{
						isMoving = false;
				} 
		else {

			isMoving = true;
			Quaternion dirRot = Quaternion.LookRotation(curPos - lastPos);
			dirRot.x = 0f;
			dirRot.z = 0f;
			AI.transform.rotation = dirRot;
		}
		lastPos = curPos;
		if(!AI.animation.IsPlaying("Idle") && isMoving == false)
		{
			AI.animation.Play ("Idle");
		}
		if(!AI.animation.IsPlaying("Walk") && isMoving == true)
		{	
			AI.animation.Play ("Walk");
		}
		if((new Vector3(AI.transform.position.x, 0, AI.transform.position.z) - new Vector3(CommonVariables.HMD.transform.position.x, 0, CommonVariables.HMD.transform.position.z)).sqrMagnitude > 2.25) //basically mag > 1.5F
		{
			Vector3 dir = Vector3.Normalize(new Vector3(CommonVariables.HMD.transform.position.x, 0, CommonVariables.HMD.transform.position.z) - 
			                                new Vector3(AI.transform.position.x, 0, AI.transform.position.z));
			AI.transform.position += (AI.speed * Time.deltaTime * dir);
		}

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

		return this;
	}
				
	public override FriendlyAIBehaviour OnDamage(DamageObject dmg)
	{
		return new FriendlyCombat (AI);
	}
}