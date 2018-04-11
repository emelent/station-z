using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(Enemy))]

public class _ZombieAI : MonoBehaviour {

	public enum State{Roaming, Chasing, Attacking, Frozen};

	public State aiState = State.Roaming;
	public float chaseFactor = 1.2f;
	[Header("Roam Config")]
	public float minChangeDelay = 1f;
	public float maxChangeDelay = 2f;
	public float minRoamDistance = 100f;
	public float maxRoamDistance = 400f;


	[Header("Chase Config")]
	public float updateRate = 2f;
	public float maxFollowDistance = 100f;

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
			rb.Sleep();
			roamTravelled =  1f;
			roamDistance  = 0f;
			delayedChooseRandomDirection(0.2f);
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
					rb.Sleep();
					aiState = State.Attacking;
					StartCoroutine(attackTarget());
				}
				break;
		
			case State.Attacking:
				if(attack.target == null){
					if(vision.target){
						aiState = State.Chasing;
						StartCoroutine(followPlayer());
					}else{
						// target lost go back to roaming
						aiState = State.Roaming;
						chooseRandomDirection();
					}
				}
				break;
			case State.Frozen:
				rb.Sleep();
				break;
		}
	}

	void FixedUpdate () {
		switch(aiState){
			case State.Roaming:
				Roam();
				break;
		}
	}


	IEnumerator delayedChooseRandomDirection(float delay=-1f){
		roamTravelled = 0f;
		delay = (delay == -1)? Random.Range(minChangeDelay, maxChangeDelay): delay;
		rb.Sleep();
		yield return new WaitForSeconds(delay);
		chooseRandomDirection();
	}

	void chooseRandomDirection(){
		rb.Sleep();
		transform.Rotate(0f, 0f, Random.Range(0f, 359f));
		Vector2 dir = (forwardPoint.position - transform.position).normalized;
		roamTravelled = 0f;
		roamDistance  = Random.Range(minRoamDistance, maxRoamDistance);
		move(dir * enemy.moveForce * Time.deltaTime);
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

	void move(Vector2 force){
		if(!enemy.allowedToMove) return;
		Vector2 limit = enemy.movementSpeed *  force.normalized;
		if(rb.velocity.magnitude < limit.magnitude)
			rb.AddForce(force, ForceMode2D.Impulse);
	}

	IEnumerator followPlayer(){
		if(vision.target == null){
			aiState = State.Roaming;
			yield return false;
		}	
		if(vision.target != null){
			float dist = Vector2.Distance(transform.position, vision.target.position);
			if(dist > maxFollowDistance){
				aiState = State.Roaming;
				vision.target = null;
				yield break;
			}
			Vector2 dir = vision.target.position - transform.position;
			dir.Normalize();
			// update rotation
			transform.rotation = Quaternion.FromToRotation(
				Vector3.up,
				dir
			);

			//  Move the AI
			rb.Sleep();
			move(dir * enemy.moveForce * chaseFactor * Time.deltaTime);
			yield return new WaitForSeconds(1 / updateRate);

			if(aiState == State.Chasing)
				StartCoroutine(followPlayer());
		}
		
			
	}

	void Roam(){
		if(roamTravelled > roamDistance){
			StartCoroutine(delayedChooseRandomDirection());
		}
		roamTravelled += rb.velocity.magnitude;  

	}




}
