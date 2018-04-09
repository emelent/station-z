using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {

	public float amount = 40f;
	
	void OnTriggerEnter2D(Collider2D collider){
		if(collider.tag == "Player"){
			collider.GetComponent<Player>()
				.GetHealthBar()
				.Heal(amount);
			GameMaster.PlayAudio("HealthPickup");
			Destroy(gameObject);
		}
	}
}
