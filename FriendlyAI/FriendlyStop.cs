using UnityEngine;
using System.Collections;

public class FriendlyStop : FriendlyAIBehaviour
{
	public float waitThresh = 5F;
	public float timer = 0F;

	public FriendlyStop (FriendlyAI ai) : base(ai)
	{
		AI.renderer.material.color = Color.yellow;
	}

	public override FriendlyAIBehaviour Update()
	{
		timer += Time.deltaTime;

		if(timer > waitThresh)
		{
			return new FriendlyFollow(AI);
		}
		//Any other update logic can go here too

		return this;
	}
	
	public override FriendlyAIBehaviour OnDamage(DamageObject dmg)
	{
		return this;
	}
}
