using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour {

	public float amount = 40f;

	protected void effect(Collider2D collider){
		if(collider.tag == "Player"){
			collider.GetComponent<GameCharacter>()
				.Heal(amount);
			Destroy(gameObject);
		}
	}
}
