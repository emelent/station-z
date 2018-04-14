using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	An equippable item, currently just weapons. Could be extended to other things
	the player can carry. equipsthe prefab to the player.
*/
public class EquipItem : Item {

	public Transform itemPrefab;

	protected override void effect(Collider2D collider){
		if(collider.tag == "Player"){
			collider.GetComponent<PlayerController>()
				.EquipItem(itemPrefab);
			playAudio();
			Destroy(gameObject);
		}
	}
}
