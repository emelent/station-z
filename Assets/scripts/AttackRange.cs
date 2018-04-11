using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour {

	public GameCharacter target;

	void Update(){
		if(target && !target.gameObject.activeSelf)
			target = null;
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.tag == "Player"){
			target = collider.GetComponent<GameCharacter>();
		}
	}

	void OnTriggerExit2D(Collider2D collider){
		if(target && collider.gameObject == target.gameObject){
			target = null;
		}
	}
}
