using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour {
	public bool allowedToMove = true;
	public float movementSpeed = 200f;
	public float rotateStep = 1f;
	public Color damageColor = Color.red;

	Health health;
	[SerializeField]
	Vector2 velocity = Vector2.zero;
	SpriteRenderer spriteRenderer;

	void Awake(){
		health = GetComponent<Health>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}


	IEnumerator showDamage(){
		spriteRenderer.color = damageColor;
		//TODO: random blood particles
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
		StartCoroutine(showDamage());
	}

	public Vector2 GetVelocity(){
		return velocity;
	}

	public void SetVelocity(Vector2 v){
		velocity = v;
	}
	
}
