using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour {
	public bool allowedToMove = true;
	public float movementSpeed = 200f;
	public float rotateStep = 1f;
	public Color damageColor = Color.red;
	public ParticleSystem bloodSplatter;

	Health health;
	[SerializeField]
	Vector2 velocity = Vector2.zero;
	SpriteRenderer spriteRenderer;
	SightRange sightRange;

	void Awake(){
		health = GetComponent<Health>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		sightRange =transform.Find("SightRange").GetComponent<SightRange>();
	}


	IEnumerator showDamage(){
		spriteRenderer.color = damageColor;
		//blood particles
		if(bloodSplatter){
			bloodSplatter.Play();
		}
		
		yield return new WaitForSeconds(0.2f);
		spriteRenderer.color = Color.white;
	}


	public void Reset(){
		// reset hp
		health.Reset();
	}

	public Health GetHealth(){
		return health;
	}

	public void Damage(float amount){
		health.Damage(amount);
		if(health.GetHealth() == 0f){
			StopAllCoroutines();
			GameMaster.KillEnemy(this);
		}else{
			StartCoroutine(showDamage());
		}
	}

	public void SetTarget(Transform target){
		sightRange.target =  target;
	}
	public Vector2 GetVelocity(){
		return velocity;
	}

	public void SetVelocity(Vector2 v){
		velocity = v;
	}
	
}
