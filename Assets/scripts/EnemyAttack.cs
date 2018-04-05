using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour {

	public Player target;

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.tag == "Player"){
			target = collider.GetComponent<Player>();

			// enter attack mode
		}
	}

	void OnTriggerExit2D(Collider2D collider){
		if(collider.gameObject == target.gameObject){
			target = null;
			// get out of attack mode
		}
	}
}
