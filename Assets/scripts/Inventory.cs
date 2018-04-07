using UnityEngine;

public class Inventory{

	Transform[] playerWeapons;

	public Inventory(uint numPlayers){
		playerWeapons = new Transform[numPlayers];
	}

	public void StorePlayerWeapon(uint playerNum, Transform weapon){
		if(playerNum < playerWeapons.Length){
			playerWeapons[playerNum] = weapon;
		}
	}
}