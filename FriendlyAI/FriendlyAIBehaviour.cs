using UnityEngine;
using System.Collections;

public abstract class FriendlyAIBehaviour
{
	public FriendlyAI AI;

	public FriendlyAIBehaviour(FriendlyAI ai)
	{
		AI = ai;
	}
	
	public abstract FriendlyAIBehaviour Update();
	
	public abstract FriendlyAIBehaviour OnDamage(DamageObject dmg);
}