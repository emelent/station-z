using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbZombieAI: ZombieAI {

	[Header("Roam State")]
	public float minChangeDelay = 1f;
	public float maxChangeDelay = 2f;
	public float minRoamDistance = 100f;
	public float maxRoamDistance = 400f;


	[Header("Chase State")]
	public float updateRate = 2f;
	public float maxFollowDistance = 100f;

	float roamDistance;
	float roamTravelled = 0f;

	void Start(){
		moveInRandomDirection();
	}

	void Update(){
		switch(aiState){
			case State.Roaming:
				if(sightRange.target){
					aiState = State.Chasing;
					GM.PlayAudio("ZombieChase");
					StartCoroutine(followTarget());
				}
				break;

			case State.Chasing:
				if(!sightRange.target){
					// target lost go back to roaming
					aiState = State.Roaming;
					moveInRandomDirection();
				}
				if(attackRange.target){
					rb.Sleep();
					aiState = State.Attacking;
				}
				break;
		
			case State.Attacking:
				if(attackRange.target != null){
					if(attackRange.target.gameObject != sightRange.target.gameObject){
						aiState = State.Chasing;
					}else if(Time.time > attackTime)
						attackTarget();
				}else{
					if(sightRange.target){
						aiState = State.Chasing;
						StartCoroutine(followTarget());
					}else{
						// target lost go back to roaming
						aiState = State.Roaming;
						moveInRandomDirection();
					}
				}
				break;

			case State.Frozen:
				rb.Sleep();
				break;
		}
	}

	void FixedUpdate(){
		if(aiState == ZombieAI.State.Roaming){
			if(roamTravelled > roamDistance){
				StartCoroutine(pauseThenMoveInRandomDirection());
			}
			roamTravelled += rb.velocity.magnitude;  
		}
	}

	void OnCollisionEnter2D(Collision2D collision){
		rb.Sleep();
		if(collision.collider.tag != "Player"){
			roamTravelled =  1f;
			roamDistance  = 0f;
			pauseThenMoveInRandomDirection(0.2f);
		}
	}

	
	IEnumerator pauseThenMoveInRandomDirection(float delay=-1f){
		roamTravelled = 0f;
		delay = (delay == -1)? Random.Range(minChangeDelay, maxChangeDelay): delay;
		rb.Sleep();
		yield return new WaitForSeconds(delay);
		moveInRandomDirection();
	}

	IEnumerator followTarget(){
		if(sightRange.target == null){
			aiState = State.Roaming;
			yield return false;
		}	

		if(sightRange.target != null){
			float dist = Vector2.Distance(transform.position, sightRange.target.position);
			if(dist > maxFollowDistance){
				aiState = State.Roaming;
				sightRange.target = null;
				yield break;
			}
			Vector2 dir = sightRange.target.position - transform.position;
			dir.Normalize();

			// update rotation
			transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);

			//  Move the AI
			rb.velocity = dir * character.movementSpeed * chaseFactor * Time.deltaTime;
			yield return new WaitForSeconds(1 / updateRate);

			if(aiState == State.Chasing)
				StartCoroutine(followTarget());
		}	
	}

	void attackTarget(){
		attackTime = Time.time + 1/attackRate;
		GM.PlayAudio(attackSound);
		if(attackRange.target){
			attackRange.target.Hurt(attackDamage, name);
		}
	}

	void moveInRandomDirection(){
		rb.Sleep();
		transform.Rotate(0f, 0f, Random.Range(0f, 359f));
		Vector2 dir = (forwardPoint.position - transform.position).normalized;
		roamTravelled = 0f;
		roamDistance  = Random.Range(minRoamDistance, maxRoamDistance);
		rb.velocity = dir * character.movementSpeed * Time.deltaTime;
	}
}
