using UnityEngine;
using System.Collections;

//Idle state
//Color is Green

public class EnemyIdle : EnemyAIBehaviour
{
	public EnemyIdle (EnemyAI ai) : base(ai)
	{
		AI.renderer.material.color = Color.green;
	}
	
	public override EnemyAIBehaviour Update()
	{
		//Color is Green
		//Check raycast to Player
		foreach(GameObject part in CommonVariables.PlayerParts)
		{
			RaycastHit hit;
			if(Physics.Raycast(AI.sightOrigin.transform.position, part.transform.position - AI.sightOrigin.transform.position, out hit, AI.sightRange))
			{
				if(hit.collider.gameObject == part)
				{
					//I see the player, or at least part of him
					return new EnemyCombatNoShoot(AI);
				}
			}
		}
		//Any other update logic can go here too
		
		
		
		
		//Don't transition
		return this;
	}
	
	public override EnemyAIBehaviour OnDamage(DamageObject dmg)
	{
		return new EnemyCombatNoShoot(AI);
	}
}