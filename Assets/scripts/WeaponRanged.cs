using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRanged: Weapon{

	public float range = 2f;
	public float cameraShakeAmount =  0.01f;
	public float  cameraShakeLength = 0.1f;

	public Transform hitParticlePrefab;
	public Transform bulletTrailPrefab;

	private Transform firePoint;
	private Transform muzzleFlash;
	private Vector3 fakeNormal = new Vector3(9999,9999, 9999);

	void Awake(){
		firePoint = transform.Find("FirePoint");
		muzzleFlash = transform.Find("MuzzleFlash");
		if(muzzleFlash)
			muzzleFlash.gameObject.SetActive(false);
	}

	
	protected override void attack(string attacker){
		// play sound
		GM.PlayAudio(attackSound);
		// Show muzzle flash
		StartCoroutine(showMuzzleFlash());

		// Raycast bullet
		Vector2 firePos = firePoint.position;
		Vector2 dir = (firePos - (Vector2) transform.position).normalized;
		RaycastHit2D hit = Physics2D.Raycast(firePos, dir, range, whatToHit);
		Vector3 normal = fakeNormal;
		Vector3 hitPos = (dir * (range/1)) + firePos;

		if(hit.collider){
			normal = hit.normal;
			hitPos = hit.point;
			Debug.DrawLine(firePos, hit.point, Color.red);
			GameCharacter c = hit.collider.GetComponent<GameCharacter>();
			if(c && c.tag != "Player" || GM.instance.friendlyFire){
				c.Hurt(attackDamage, attacker);
				c.KnockBack(dir * attackKnockBack);
			}
		}
		
		showEffect(hitPos, normal);
	}
	
	IEnumerator showMuzzleFlash(){
		if(muzzleFlash)
			muzzleFlash.gameObject.SetActive(true);

		yield return new WaitForSeconds(0.01f);
		if(muzzleFlash)
			muzzleFlash.gameObject.SetActive(false);
	}

	void showEffect(Vector3 hitPos, Vector3 hitNormal){
		// bullet trail
		Transform  trail = (Transform) Instantiate(bulletTrailPrefab, hitPos, transform.rotation);
		LineRenderer lr = trail.GetComponent<LineRenderer>();
		if(lr != null){
			lr.SetPosition(0, firePoint.position);
			lr.SetPosition(1, hitPos);
		}
		Destroy(trail.gameObject, 0.04f);

		// particles
		if(!hitNormal.Equals(fakeNormal)){
			Transform hitParticles = (Transform) Instantiate(
				hitParticlePrefab, 
				hitPos, 
				Quaternion.FromToRotation(Vector3.right, hitNormal)
			);
			Destroy(hitParticles.gameObject, 1f);
		}

		// camera shake
		GM.ShakeCamera(cameraShakeAmount, cameraShakeLength);
	}


}