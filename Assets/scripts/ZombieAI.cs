using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class ZombieAI : MonoBehaviour {


	float changeDistance;
	float changeDistanceTravelled = 0f;

	Enemy enemy;
	Transform forwardPoint;	
	Transform target;

	void Awake(){
		enemy = GetComponent<Enemy>();
		forwardPoint = transform.Find("ForwardPoint");
		chooseRandomDirection();
	}

	void OnCollisionEnter2D(Collision2D collision){
		if(collision.collider.tag != "Player"){
			// velocity *= -1;
			// changeDistanceTravelled = 0f;
			chooseRandomDirection();	
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		changeDistanceTravelled += enemy.movementSpeed * Time.deltaTime;  
		if(changeDistanceTravelled >  changeDistance){
			chooseRandomDirection();
		}
	}

	void chooseRandomDirection(){
		transform.Rotate(0f, 0f, Random.Range(0f, 359f));
		Vector2 direction = (transform.position - forwardPoint.position).normalized;
		enemy.SetVelocity(direction * enemy.movementSpeed * Time.deltaTime);
		changeDistanceTravelled = 0f;
		changeDistance  = Random.Range(100f, 200f);
	}


}
