using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
	public float damage = 2F;
	public float hitForce = 5F;
	public string WiimoteName = "RightWiimote";
	public GameObject GunEmission;
	public float range = 50F;
	public LineRenderer lineRen;
	public float lineTimer = 0F;

	public ParticleSystem muzzleSpark;
	public ParticleSystem muzzleBlast;
	public Light blastLight;
	public Material[] concreteMarks;
	public Material[] metalMarks;
	public Material[] woodMarks;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		if(InputBroker.GetKeyPress(WiimoteName + ":B") || Input.GetMouseButtonDown(0))	
		{
			//Gunshot
			GunEmission.audio.Play ();
			muzzleSpark.Play();
			muzzleBlast.Play();
			StartCoroutine(LightFlash());

			RaycastHit hit;
			if (Physics.Raycast (GunEmission.transform.position, transform.forward, out hit, range))
			{
				try
				{
					hit.collider.gameObject.SendMessage("OnDamage", new DamageObject(damage));
					Debug.Log (hit.collider.gameObject.name + " was shot by Player");
				}
				catch(UnityException ex)
				{
					Debug.Log (ex);
				}
				//Debug.DrawLine(hit.point, GunEmission.transform.position, Color.red, 1F);
				lineRen.SetVertexCount(2);
				lineRen.SetPosition(0, GunEmission.transform.position);
				lineRen.SetPosition(1, hit.point);
				lineTimer = 0;

				// Force on Movable objects
				if (hit.collider.gameObject.layer == 12)	{
					hit.collider.gameObject.rigidbody.AddForce (transform.forward * hitForce);
				}

				else {
					// Bullet marks
					if (hit.collider.gameObject.tag == "explosive")	{
						Debug.Log ("splode");
						GameObject hitBlast = (GameObject)Instantiate(Resources.Load ("explosion"));
						hitBlast.transform.position = hit.point;
						Destroy(hit.collider.gameObject);
					}
					if (hit.collider.gameObject.tag == "concrete")	{
						GameObject bulletHole = (GameObject)Instantiate(Resources.Load("PF_Bullet_Mark_01"));
						bulletHole.renderer.material = concreteMarks[Random.Range (0, 2)];
						Vector3 rotateTo = new Vector3(0,0,0);
						bulletHole.transform.position = hit.point;
						bulletHole.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -hit.normal) * Quaternion.Euler(rotateTo.x, rotateTo.y, rotateTo.z);;
						GameObject hitBlast = (GameObject)Instantiate(Resources.Load ("particle_concrete"));
						hitBlast.transform.position = hit.point;
						hitBlast.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -hit.normal) * Quaternion.Euler(rotateTo.x, rotateTo.y, rotateTo.z);;
					}
					if (hit.collider.gameObject.tag == "metal")	{
						GameObject bulletHole = (GameObject)Instantiate(Resources.Load("PF_Bullet_Mark_01"));
						bulletHole.renderer.material = metalMarks[Random.Range (0, 2)];
						Vector3 rotateTo = new Vector3(0,0,0);
						bulletHole.transform.position = hit.point;
						bulletHole.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -hit.normal) * Quaternion.Euler(rotateTo.x, rotateTo.y, rotateTo.z);;
						GameObject hitBlast = (GameObject)Instantiate(Resources.Load ("particle_metal"));
						hitBlast.transform.position = hit.point;
						hitBlast.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -hit.normal) * Quaternion.Euler(rotateTo.x, rotateTo.y, rotateTo.z);;
					}
					if (hit.collider.gameObject.tag == "wood")	{
						GameObject bulletHole = (GameObject)Instantiate(Resources.Load("PF_Bullet_Mark_01"));
						bulletHole.renderer.material = woodMarks[Random.Range (0, 2)];
						Vector3 rotateTo = new Vector3(0,0,0);
						bulletHole.transform.position = hit.point;
						bulletHole.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -hit.normal) * Quaternion.Euler(rotateTo.x, rotateTo.y, rotateTo.z);;
						GameObject hitBlast = (GameObject)Instantiate(Resources.Load ("particle_wood"));
						hitBlast.transform.position = hit.point;
						hitBlast.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -hit.normal) * Quaternion.Euler(rotateTo.x, rotateTo.y, rotateTo.z);;

					}
					if (hit.collider.gameObject.tag == "dirt")	{
						Vector3 rotateTo = new Vector3(0,0,0);
						GameObject hitBlast = (GameObject)Instantiate(Resources.Load ("particle_dirt"));
						hitBlast.transform.position = hit.point;
						hitBlast.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -hit.normal) * Quaternion.Euler(rotateTo.x, rotateTo.y, rotateTo.z);;
					}
				}
			}
		}
		lineTimer += Time.deltaTime;
		if(lineTimer > 0.5F)
		{
			lineRen.SetVertexCount(0);
		}
	}

	IEnumerator LightFlash()	{
		if (!blastLight.light.enabled)	{
			blastLight.enabled = true;
			yield return new WaitForSeconds(0.1f);
			blastLight.enabled = false;
		}
		yield return null;
	}
}
