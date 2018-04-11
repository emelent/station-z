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
			case ZombieAI.State.Roaming:
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

	void moveInRandomDirection(){
		rb.Sleep();
		transform.Rotate(0f, 0f, Random.Range(0f, 359f));
		Vector2 dir = (forwardPoint.position - transform.position).normalized;
		roamTravelled = 0f;
		roamDistance  = Random.Range(minRoamDistance, maxRoamDistance);
		rb.velocity = dir * character.movementSpeed * Time.deltaTime;
	}
}
