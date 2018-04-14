using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This AI roams randomly till it spots a player, then it chases the
// player in hopes to attack him. 
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
				if(attackRange.target != null){ // still in range of attack? then attack
					if(Time.time > attackTime)
						attackTarget();
				}else{ // out of range to attack
					if(sightRange.target){ // player is in sight? chase the player
						aiState = State.Chasing;
						StartCoroutine(followTarget());
					}else{ // player not in range or in sight? go back to roaming
						aiState = State.Roaming;
						moveInRandomDirection();
					}
				}
				break;

			case State.Frozen: // mainly for debug. stops enemy from doing anything
				rb.Sleep();
				break;
		}
	}

	void FixedUpdate(){
		if(aiState == ZombieAI.State.Roaming){
			if(roamTravelled > roamDistance){ // change direction when random distance travelled
				StartCoroutine(pauseThenMoveInRandomDirection());
			}
			roamTravelled += rb.velocity.magnitude;  
		}
	}

	void OnCollisionEnter2D(Collision2D collision){
		rb.Sleep();
		if(collision.collider.tag != "Player"){ // change direction when colliding with anything except the player
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
		if(sightRange.target == null){ // if there isn't a target anymore for some reason, go back to roaming
			aiState = State.Roaming;
			yield return false;
		}	

		if(sightRange.target != null){ // move towards target..
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
		//  TODO for some reason the attackSound doesn't play
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
