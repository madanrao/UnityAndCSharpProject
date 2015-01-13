using UnityEngine;
using System.Collections;

[AddComponentMenu("AI/Friendly")]
public class FriendlyAI : MonoBehaviour
{
	public float maxHealth = 10F;
	public float speed = 2F;
	public float sightDistance = 20F;
	public float gunRange = 100F;
	public float damage = 2F;
	private float health;
	public LineRenderer lineRen;
	public float lineTimer = 0F;

	public GameObject GunEmission;
	public ParticleSystem muzzleSpark;
	public ParticleSystem muzzleBlast;
	public Light blastLight;

	public FriendlyAIBehaviour behave;

	// Use this for initialization
	void Awake ()
	{
		behave = new FriendlyFollow (this);
		health = maxHealth;
		CommonVariables.StopGestureListener += StopGesture;
		CommonVariables.PointGestureListener += PointGesture;
		CommonVariables.FollowGestureListener += FollowGesture;
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
	}

	public void OnDamage(DamageObject dmg)
	{
		health -= dmg.damage;
		//fix the other classes accordingly
		behave = behave.OnDamage (dmg);
		if(health <= 0)
		{
			//Dead Behaviour
			//behave = new Dead();
		}
	}

	public void StopGesture()
	{
		if(behave.GetType() == typeof(FriendlyStop))
		{
			((FriendlyStop)behave).timer = 0F;
		}
		else
		{
			behave = new FriendlyStop(this);
		}
	}

	public void PointGesture()
	{
		behave = new FriendlyGo(this, CommonVariables.Pointing);
	}

	public void FollowGesture()
	{
		behave = new FriendlyFollow(this);
	}
}
