using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour {

	public Transform target;


	void Update(){
		if(target && !target.gameObject.activeSelf)
			target = null;
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.tag == "Player"){
			target = collider.transform;

			// enter attack mode
		}
	}

	void OnTriggerExit2D(Collider2D collider){
		if(target == null)
			return;

		if(collider.gameObject == target.gameObject){
			target = null;
			// get out of attack mode
		}
	}
}
