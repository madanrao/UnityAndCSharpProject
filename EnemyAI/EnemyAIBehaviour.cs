using UnityEngine;
using System.Collections;

public abstract class EnemyAIBehaviour
{
	public EnemyAI AI;
	
	public EnemyAIBehaviour(EnemyAI ai)
	{
		AI = ai;
	}
	
	public abstract EnemyAIBehaviour Update();
	
	public abstract EnemyAIBehaviour OnDamage(DamageObject dmg);
}