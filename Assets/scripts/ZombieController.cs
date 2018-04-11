
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour {

	SightRange sightRange;
	Rigidbody2D rb;
	GameCharacter character;

	void Awake(){
		character = GetComponent<GameCharacter>();
		rb = GetComponent<Rigidbody2D>();
		sightRange =transform.Find("SightRange").GetComponent<SightRange>();
	}
	
	public void SetTarget(Transform target){
		sightRange.target =  target;
	}

}
