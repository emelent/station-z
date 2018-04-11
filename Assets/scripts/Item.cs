using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	protected void effect(Collider2D collider){
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D collider){
		effect(collider);
	}
}
