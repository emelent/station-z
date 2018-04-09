using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponPickupType{
	public Transform WeaponPrefab;
	public Sprite Sprite;
}

public class WeaponPickup : MonoBehaviour {

	public enum WeaponType{Pistol, Shotgun, Uzzi};

	public WeaponType type = WeaponType.Shotgun;

	[SerializeField]
	WeaponPickupType[] Weapons;

	void Awake(){
		GetComponent<SpriteRenderer>()
			.sprite = Weapons[(int)type].Sprite;
	}

	void OnTriggerEnter2D(Collider2D collider){
		if(collider.tag == "Player"){
			GameMaster.instance.CreatePlayerWeapon(
				collider.GetComponent<Player>(),
				Weapons[(int)type].WeaponPrefab
			);
			GameMaster.PlayAudio("WeaponPickup");
			Destroy(gameObject);
		}
	}
}
