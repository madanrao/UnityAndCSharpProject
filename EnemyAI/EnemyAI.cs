using UnityEngine;
using System.Collections;

[AddComponentMenu("AI/Enemy")]
public class EnemyAI : MonoBehaviour, Damagable
{
	public float sightRange = 20F;
	public float speed = 1.38889F; 	 //Walking speed = 3.10 mph	
	public float maxHealth = 10F;
	public float health;
	public int healthGainRate = 1;

	public float damage = 2F;
	public GameObject GunEmission;
	public ParticleSystem muzzleSpark;
	public ParticleSystem muzzleBlast;
	public Light blastLight;
	public Transform sightOrigin;
	public float gunRange = 30F;
	
	public EnemyAIBehaviour behave;

	public LineRenderer lineRen;
	public float lineTimer = 0F;

	// Use this for initialization
	void Awake ()
	{
		health = maxHealth;
		behave = new EnemyIdle (this);
		CommonVariables.Enemies.Add (gameObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
		behave = behave.Update ();
		lineTimer += Time.deltaTime;
		if(lineTimer > 0.5F)
		{
			lineRen.SetVertexCount(0);
		}

		//HealthGain was crazy strong
		/*if(health < maxHealth)
		{
			health += healthGainRate * Time.deltaTime;
		}
		if(health > maxHealth)
		{
			health = maxHealth;
		}*/
	}

	public void OnDamage(DamageObject dmg)
	{
		health -= dmg.damage;
		//fix the other classes accordingly
		behave = behave.OnDamage (dmg);
		if(health <= 0)
		{
			//Dead Behaviour
			behave = new EnemyDead(this);
		}
	}
}
