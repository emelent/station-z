using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float movementSpeed = 200f;
	public float rotateStep = 1f;
	

	Vector2 velocity = Vector2.zero;
	Rigidbody2D rb;


	void Awake(){
		rb = GetComponent<Rigidbody2D>();
	}

	void Update(){
		// TODO set random direction
	}

	
	void FixedUpdate(){
		// move
		rb.velocity = velocity;
	}


	public Vector2 GetVelocity(){
		return velocity;
	}

	public void SetVelocity(Vector2 v){
		velocity = v;
	}

	public void Reset(){
		velocity = Vector2.zero;
	}
}
