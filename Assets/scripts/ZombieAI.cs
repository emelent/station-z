﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Enemy))]

public class ZombieAI : MonoBehaviour {

	public enum State{Roaming, Chasing, Attacking, Frozen};

	public State aiState = State.Roaming;
	public float chaseFactor = 1.2f;
	[Header("Roam Config")]
	public float minChangeDelay = 1f;
	public float maxChangeDelay = 5f;
	public float minRoamDistance = 100f;
	public float maxRoamDistance = 400f;

	[Header("Chase Config")]
	public float updateRate = 2f;

	[Header("Attack Config")]
	public float attackDamage = 10f;
	public float attackRate = 2f;


	[Header("Attack Components")]
	public SightRange vision;
	public AttackRange attack;


	float roamDistance;
	float roamTravelled = 0f;

	Transform forwardPoint;	
	Enemy enemy;
	Rigidbody2D rb;

	
	void Awake(){
		rb = GetComponent<Rigidbody2D>();
		enemy = GetComponent<Enemy>();
		forwardPoint = transform.Find("ForwardPoint");
		chooseRandomDirection();
	}

	void OnCollisionEnter2D(Collision2D collision){
		if(collision.collider.tag != "Player"){
			chooseRandomDirection();
		}
	}
	
	void Update(){
		switch(aiState){
			case State.Roaming:
				if(vision.target){
					aiState = State.Chasing;
					GameMaster.PlayAudio("ZombieChase");
					StartCoroutine(followPlayer());
				}
				break;

			case State.Chasing:
				if(!vision.target){
					// target lost go back to roaming
					aiState = State.Roaming;
					chooseRandomDirection();
				}
				if(attack.target){
					aiState = State.Attacking;
					enemy.SetVelocity(Vector2.zero);
					print("Start attacking");
					StartCoroutine(attackTarget());
				}
				break;
		
			case State.Attacking:
				if(attack.target == null){
					if(vision.target){
						aiState = State.Chasing;
						print("Go back to chasing");
						StartCoroutine(followPlayer());
					}else{
						// target lost go back to roaming
						aiState = State.Roaming;
						chooseRandomDirection();
					}
				}
				break;
			case State.Frozen:
				enemy.SetVelocity(Vector2.zero);
				break;
		}
	}

	void FixedUpdate () {
		switch(aiState){
			case State.Roaming:
				Roam();
				break;
		}

		if(enemy.allowedToMove)
			rb.velocity = enemy.GetVelocity();
	}


	IEnumerator delayedChooseRandomDirection(){
		roamTravelled = 0f;
		enemy.SetVelocity(Vector2.zero);
		yield return new WaitForSeconds(
			Random.Range(minChangeDelay, maxChangeDelay)
		);
		chooseRandomDirection();
	}

	void chooseRandomDirection(){
		rb.Sleep();
		transform.Rotate(0f, 0f, Random.Range(0f, 359f));
		Vector2 direction = (transform.position - forwardPoint.position).normalized;
		roamTravelled = 0f;
		roamDistance  = Random.Range(minRoamDistance, maxRoamDistance);
		enemy.SetVelocity(direction * -enemy.movementSpeed * Time.deltaTime);
	}

	IEnumerator attackTarget(){
		if(!attack.target){
			print("No target");
			aiState = State.Roaming;
			yield return false;
		}


		GameMaster.PlayAudio("ZombieAttack");
		Player player = attack.target.GetComponent<Player>();
		player.Damage(attackDamage);
		yield return new WaitForSeconds(1f/attackRate);
		if(aiState == State.Attacking)
			StartCoroutine(attackTarget());
	}


	IEnumerator followPlayer(){

		Vector2 dir = vision.target.position - transform.position;
		dir.Normalize();
		// update rotation
		transform.rotation = Quaternion.FromToRotation(
			Vector3.up,
			dir
		);

		//  Move the AI
		enemy.SetVelocity(dir * enemy.movementSpeed * Time.deltaTime * chaseFactor);
		yield return new WaitForSeconds(1 / updateRate);

		if(aiState == State.Chasing)
			StartCoroutine(followPlayer());
	}

	void Roam(){
		if(roamTravelled > roamDistance){
			StartCoroutine(delayedChooseRandomDirection());
		}
		roamTravelled += rb.velocity.magnitude;  

	}




}
