using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTiles : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.tag == "Player"){
			collider.GetComponent<Player>()
				.SetInWater(true);
		}

		GameMaster.PlayAudio("WaterSplash");
	}

	void OnTriggerExit2D(Collider2D collider){
		if(collider.tag == "Player"){
			collider.GetComponent<Player>()
				.SetInWater(false);
		}
	}
}
