using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEffect : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.tag == "Player"){
			collider.GetComponent<Player>()
				.SetInWater(true);
			GameMaster.PlayAudio("WaterSplash");
		}else if(collider.tag == "Enemy"){
			GameMaster.PlayAudio("WaterSplash");
		}

	}

	void OnTriggerExit2D(Collider2D collider){
		if(collider.tag == "Player"){
			collider.GetComponent<Player>()
				.SetInWater(false);
			GameMaster.PlayAudio("WaterSplash");
		}else if(collider.tag == "Enemy"){
			GameMaster.PlayAudio("WaterSplash");
		}
	}
}
