using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public float cameraShakeAmount =  0.05f;
	public float  cameraShakeLength = 0.1f;
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
	
	void Start () {
		firePoint = transform.Find("FirePoint");
		if(muzzleFlash)
			muzzleFlash.gameObject.SetActive(false);
		print(firePoint);
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
		if(hit.collider){
			Debug.DrawLine(firePos, hit.point, Color.red);
			if(hit.collider.tag == "Enemy"){
				Enemy enemy =  hit.collider.GetComponent<Enemy>();
				enemy.Damage(attackDamage);
			} else if(hit.collider.tag ==  "Player"){
				Player player = hit.collider.GetComponent<Player>();
				player.Damage(attackDamage);
			}else{
				// only show this for non organic targets
				showEffect(hit.point, hit.normal);
			}
		}else{
			Debug.DrawLine(firePos, firePos + (dir * attackRange));
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

		// camera shake
		GameMaster.ShakeCamera(cameraShakeAmount, cameraShakeLength);
	}



}
