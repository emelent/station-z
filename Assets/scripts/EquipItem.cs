using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
