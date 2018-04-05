using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour {
	public bool allowedToMove = true;
	public float movementSpeed = 200f;
	public float rotateStep = 1f;

	Health health;
	[SerializeField]
	Vector2 velocity = Vector2.zero;

	void Awake(){
		health = GetComponent<Health>();
	}

	public void Reset(){
		// reset hp
		health.Reset();
	}

	public Health GetHealth(){
		return health;
	}

	public Vector2 GetVelocity(){
		return velocity;
	}

	public void SetVelocity(Vector2 v){
		velocity = v;
	}
	
}
