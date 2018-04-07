using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

	const int MAX_PLAYERS = 3;
	[Range(1, MAX_PLAYERS)]
	public uint numberOfPlayers = MAX_PLAYERS;
	public float respawnDelay = 2f;	
	public bool respawnPlayers = true;
	public bool friendlyFire = true;

	public Transform[] Players = new Transform[MAX_PLAYERS];
	public Transform[] HealthBars = new Transform[MAX_PLAYERS];

	public Transform[] SpawnLocations;
	public Transform StartWeapon;
	public CameraShake cameraShake;


	[HideInInspector]
	public static GameMaster instance;
	AudioManager audioManager;

	// Use this for initialization
	void Start () {
		if(instance == null){
			instance = this;
		}
		audioManager = GetComponent<AudioManager>();

		spawnPlayers();
	}
	
	void spawnPlayers(){
		for(int i=0; i < numberOfPlayers; i++){
			Transform newPlayer = (Transform) Instantiate(
				Players[i],
				SpawnLocations[i]
			);
			// link healthbars
			HealthBars[i].gameObject.SetActive(true);
			newPlayer.GetComponent<Health>().LinkHealthBar(
				HealthBars[i].Find("Bar")
			);
			createPlayerWeapon(newPlayer.GetComponent<Player>());
		}
	}

	void createPlayerWeapon(Player player){
		//create and equip starter weapon
		Transform newWeapon = (Transform)  Instantiate(StartWeapon, player.transform);
		player.EquipWeapon(newWeapon.GetComponent<Weapon>());
	}


	void killPlayer(Player player){
		player.gameObject.SetActive(false);
		if(respawnPlayers)
			StartCoroutine(respawnPlayer(player));
	}

	IEnumerator respawnPlayer(Player player){
		yield return new WaitForSeconds(respawnDelay);
		player.Reset();
		player.transform.position = 
			SpawnLocations[player.playerNumber - 1].position;

		// equip starter weapon
		createPlayerWeapon(player);
	
		player.gameObject.SetActive(true);
	}


	void killEnemy(Enemy enemy){
		// TODO enemy death particles
		// TODO drop item

		// TODO use an object pool
		Destroy(enemy.gameObject);
	}

	public static void PlayAudio(string name){
		instance.audioManager.PlaySound(name);
	}

	public static void KillPlayer(Player player){
		instance.killPlayer(player);
		// do death particles
	}

	public static void KillEnemy(Enemy enemy){
		instance.killEnemy(enemy);
		// do death particles
	}

	public static void ShakeCamera(float amount, float length){
		instance.cameraShake.Shake(amount, length);
	}
}
