using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightRange : MonoBehaviour {

	// [HideInInspector]
	public Transform target;

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.tag  == "Player"){
			target = collider.transform;
			print("Player spotted");
		}
	}
}
