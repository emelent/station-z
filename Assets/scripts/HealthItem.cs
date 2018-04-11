using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : Item {

	public float amount = 40f;

	protected override void effect(Collider2D collider){
		if(collider.tag == "Player"){
			collider.GetComponent<GameCharacter>()
				.Heal(amount);
			playAudio();
			Destroy(gameObject);
		}
	}
}
