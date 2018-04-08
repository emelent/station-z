using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTiles : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.tag == "Player"){
			collider.GetComponent<Player>()
				.SetInWater(true);
			GameMaster.PlayAudio("WaterSplash");
		}else if(collider.tag == "Enemy"){
			GameMaster.PlayAudio("WaterSplash");
			Health health = collider.GetComponent<Health>();
			health.drainAmount = 5f;
			health.drainRate = 1f;
		}

	}

	void OnTriggerExit2D(Collider2D collider){
		if(collider.tag == "Player"){
			collider.GetComponent<Player>()
				.SetInWater(false);
		}else if(collider.tag == "Enemy"){
			Health health = collider.GetComponent<Health>();
			health.drainAmount = 0f;
			health.drainRate = 0f;	
		}
	}
}
