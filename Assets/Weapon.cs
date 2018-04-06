using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public float fireRate = 1f;
	public float attackDamage = 2f;
	public float attackRange = 2f;
	public string fireSound = "Pistol";
	public Transform muzzleFlash;
	public Transform hitParticlePrefab;
	
	public LayerMask whatToHit;

	[HideInInspector]
	public bool canShoot = true;
	Transform firePoint;
	float fireTime = 0f;

	Vector3 FAKE_NORMAL = new  Vector3(9999, 9999, 9999);
	
	void Awake () {
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


	 public void Shoot(){
		fireTime = Time.time + 1/fireRate;
		GameMaster.PlayAudio(fireSound);
		StartCoroutine(showMuzzleFlash());

		// TODO Raycast bullet
		Vector3 firePos = firePoint.position;
		Vector3 dir = (transform.position - firePos).normalized;
		RaycastHit2D hit = Physics2D.Raycast(firePos, dir, attackRange, whatToHit);
		Debug.DrawLine(firePos, dir * attackRange);
		if(hit.collider){
			Debug.DrawLine(firePos, dir * attackRange, Color.red);
			Enemy enemy =  hit.collider.GetComponent<Enemy>();
			if(enemy){
				enemy.Damage(attackDamage);
			}

			if(hit.collider){
				showEffect(hit.point, hit.normal);
			}
		}
	}

	void showEffect(Vector3 hitPos, Vector3 hitNormal){
		// particles
		if(!hitNormal.Equals(FAKE_NORMAL)){
			Transform hitParticles = (Transform) Instantiate(
				hitParticlePrefab, 
				hitPos, 
				Quaternion.FromToRotation(Vector3.right, hitNormal)
			);
			Destroy(hitParticles.gameObject, 1f);
		}
	}



}
