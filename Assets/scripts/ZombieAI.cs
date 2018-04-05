﻿using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;


[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Seeker))]
[RequireComponent (typeof(Enemy))]

public class ZombieAI : MonoBehaviour {

	public enum State{Roaming, Chasing, Attacking};

	public State aiState = State.Roaming;
	public float chaseFactor = 1.2f;
	[Header("Roam Config")]
	public float minChangeDelay = 1f;
	public float maxChangeDelay = 3f;
	public float minRoamDistance = 100f;
	public float maxRoamDistance = 400f;

	[Header("Chase Config")]
	public float updateRate = 2f;
	public float nextWayPointDistance = 3f;

	[Header("Attack Config")]
	public float attackDamage = 10f;
	public float attackRate = 2f;


	[Header("Attack Components")]
	public SightRange vision;
	public AttackRange attack;


	bool pathIsEnded = false;
	int currentWaypoint = 0;
	float roamDistance;
	float roamTravelled = 0f;

	Transform forwardPoint;	
	Enemy enemy;
	Rigidbody2D rb;
	Seeker seeker;
	Path path;

	void Awake(){
		rb = GetComponent<Rigidbody2D>();
		enemy = GetComponent<Enemy>();
		seeker = GetComponent<Seeker>();

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
					print("Start chasing");
					StartCoroutine(updatePath());
				}
				break;

			case State.Chasing:
				if(attack.target){
					aiState = State.Attacking;
					enemy.SetVelocity(Vector2.zero);
					print("Start attacking");
					StartCoroutine(attackTarget());
				}
				break;
		
			case State.Attacking:
				if(attack.target == null){
					aiState = State.Chasing;
					print("Go back to chasing");
					StartCoroutine(updatePath());
				}
				break;
		}
	}

	void FixedUpdate () {
		switch(aiState){
			case State.Roaming:
				Roam();
				break;
			case State.Chasing:
				Chase();
				break;
		}

		if(enemy.allowedToMove)
			rb.velocity = enemy.GetVelocity();
	}


	IEnumerator delayedChooseRandomDirection(){
		roamTravelled = 0f;
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

	void OnPathComplete(Path p){
		print("Path received: " + p.error);
		if(!p.error){
			path = p;
			currentWaypoint = 0;
		}
	}

	IEnumerator updatePath(){
		if(!vision.target){
			print("back to roaming");
			aiState = State.Roaming;
			yield return false;
		}
		print("Updating path to: " + vision.target.position);
		seeker.StartPath(
			transform.position,
			vision.target.position,
			OnPathComplete
		);
		yield return new WaitForSeconds(1f/updateRate);
		if(aiState == State.Chasing)
			StartCoroutine(updatePath());
	}

	IEnumerator attackTarget(){
		if(!attack.target){
			print("No target");
			aiState = State.Roaming;
			yield return false;
		}

		Health  targetHealth = attack.target.GetComponent<Health>();
		targetHealth.Damage(attackDamage);
		yield return new WaitForSeconds(1f/attackRate);
		if(aiState == State.Attacking)
			StartCoroutine(attackTarget());
	}

	void Chase(){
		if(path == null){
			return;
		}

		if(currentWaypoint >= path.vectorPath.Count){
			if(pathIsEnded)
				return;
			
			print("End of path  reached.");
			pathIsEnded = true;
			return;
		}

		pathIsEnded = false;
		Vector3 dir = path.vectorPath[currentWaypoint] - transform.position;
		dir.Normalize();
		dir *= enemy.movementSpeed * Time.deltaTime * chaseFactor;
		
		// update rotation
		transform.rotation = Quaternion.FromToRotation(
			Vector3.up,
			dir
		);
		//  Move the AI
		enemy.SetVelocity(dir);
		float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
		if(dist < nextWayPointDistance){
			currentWaypoint ++;
			return;
		}
	}

	void Roam(){
		if(roamTravelled > roamDistance){
			StartCoroutine(delayedChooseRandomDirection());
		}
		roamTravelled += rb.velocity.magnitude;  

	}




}
