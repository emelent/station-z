using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public float cameraShakeAmount =  0.01f;
	public float  cameraShakeLength = 0.1f;
	public float fireRate = 1f;
	public float attackDamage = 2f;
	public float attackRange = 2f;
	public string fireSound = "Pistol";
	public Transform muzzleFlash;
	public Transform hitParticlePrefab;
	public Transform BulletTrailPrefab;
	
	public LayerMask whatToHit;

	[HideInInspector]
	public bool canShoot = true;
	Transform firePoint;
	float fireTime = 0f;

	Vector3 FAKE_NORMAL = new  Vector3(9999, 9999, 9999);
	
	void Start () {
		firePoint = transform.Find("FirePoint");
		if(muzzleFlash)
			muzzleFlash.gameObject.SetActive(false);
	}
	
	void Update(){
		if(fireRate > 0f)
			canShoot = Time.time > fireTime;
	}

	IEnumerator showMuzzleFlash(){
		if(muzzleFlash)
			muzzleFlash.gameObject.SetActive(true);

		yield return new WaitForSeconds(0.01f);
		if(muzzleFlash)
			muzzleFlash.gameObject.SetActive(false);
	}


	 public void Shoot(Vector2 position){
		fireTime = Time.time + 1/fireRate;
		GameMaster.PlayAudio(fireSound);
		StartCoroutine(showMuzzleFlash());

		// TODO Raycast bullet
		Vector2 firePos = firePoint.position;
		Vector2 dir = (firePos - (Vector2) transform.position).normalized;
		RaycastHit2D hit = Physics2D.Raycast(firePos, dir, attackRange, whatToHit);
		Vector3 normal = FAKE_NORMAL;
		Vector3 hitPos = (dir * (attackRange/1)) + firePos;
		if(hit.collider){
			normal = hit.normal;
			hitPos = hit.point;
			Debug.DrawLine(firePos, hit.point, Color.red);
			if(hit.collider.tag == "Enemy"){
				Enemy enemy =  hit.collider.GetComponent<Enemy>();
				enemy.Damage(attackDamage);
				enemy.SetTarget(transform);
			} else if(hit.collider.tag ==  "Player"){
				Player player = hit.collider.GetComponent<Player>();
				float dmg = (GameMaster.instance.friendlyFire)?  attackDamage:0f;
				player.Damage(dmg);
			}
		}
		
		showEffect(hitPos, normal);
	}

	void showEffect(Vector3 hitPos, Vector3 hitNormal){
		// bullet trail
		Transform  trail = (Transform) Instantiate(BulletTrailPrefab, hitPos, transform.rotation);
		LineRenderer lr = trail.GetComponent<LineRenderer>();
		if(lr != null){
			lr.SetPosition(0, firePoint.position);
			lr.SetPosition(1, hitPos);
		}
		Destroy(trail.gameObject, 0.04f);

		// particles
		if(!hitNormal.Equals(FAKE_NORMAL)){
			Transform hitParticles = (Transform) Instantiate(
				hitParticlePrefab, 
				hitPos, 
				Quaternion.FromToRotation(Vector3.right, hitNormal)
			);
			Destroy(hitParticles.gameObject, 1f);
		}

		// camera shake
		GameMaster.ShakeCamera(cameraShakeAmount, cameraShakeLength);
	}



}
