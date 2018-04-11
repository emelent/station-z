using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItem : MonoBehaviour {

	public Transform item;

	protected void effect(Collider2D collider){
		if(collider.tag == "Player"){
			collider.GetComponent<PlayerController>()
				.EquipItem(item);
			Destroy(gameObject);
		}
	}
}
