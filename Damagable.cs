using UnityEngine;
using System.Collections;

public class DamageObject
{
	public float damage;
	public DamageObject(float dmg)
	{
		damage = dmg;
	}
}

public interface Damagable
{
	void OnDamage(DamageObject dmg);
}
