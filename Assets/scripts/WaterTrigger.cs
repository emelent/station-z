using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTrigger : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D collider){
		GameCharacter  character = collider.GetComponent<GameCharacter>();
		if(character){
			character.inWater = true; 
			GM.PlayAudio("WaterIn");
		}

	}

	void OnTriggerExit2D(Collider2D collider){
		GameCharacter  character = collider.GetComponent<GameCharacter>();
		if(character){
			character.inWater = false;
			GM.PlayAudio("WaterOut");
		}
	}
}
