﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

	const int MAX_PLAYERS = 3;

	public SpriteRenderer indicator;
	public Canvas canvas;
	public Text text;


	[Range(0f, 1f)]
	public float dropChance = 0.7f;
	[Range(1, MAX_PLAYERS)]
	public uint numberOfPlayers = MAX_PLAYERS;
	public float respawnDelay = 2f;	
	public bool respawnPlayers = true;
	public bool friendlyFire = true;

	public CameraShake cameraShake;
	public Transform PlayerPrefab;
	public Transform StartWeapon;

	public Transform[] HealthBars = new Transform[MAX_PLAYERS];
	public Transform[] WeaponGauges = new Transform[MAX_PLAYERS];
	public Transform[] SpawnLocations;
	public Transform[] Weapons;
	public Transform[] PickupPrefabs;

	[HideInInspector]
	public static GameMaster instance;
	AudioManager audioManager;
	public int enemyCount= 5;
	float startTime = 0f;
	int  deaths;
	// Use this for initialization
	void Start () {
		if(instance == null){
			instance = this;
		}
		audioManager = GetComponent<AudioManager>();
		startTime = Time.time;
		deaths = 0;
		spawnPlayers();
	}
	
	void Update(){
		if(Input.GetKeyDown(KeyCode.Backspace)){
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);	
		}
	}
	void spawnPlayers(){
		for(int i=0; i < numberOfPlayers; i++){
			Transform newPlayer = (Transform) Instantiate(
				PlayerPrefab,
				SpawnLocations[i].position,
				SpawnLocations[i].rotation
			);

			Player player = newPlayer.GetComponent<Player>();
			player.SetPlayerNumber(i+1);
			// link healthbars
			HealthBars[i].gameObject.SetActive(true);
			WeaponGauges[player.playerNumber - 1].gameObject.SetActive(true);
			newPlayer.GetComponent<HealthSystem>().LinkHealthBar(
				HealthBars[i].Find("Bar")
			);
			CreatePlayerWeapon(player, StartWeapon);
		}
	}

	public void CreatePlayerWeapon(Player player, Transform weaponPrefab){
		//create and equip starter weapon
		Transform newWeapon = (Transform)  Instantiate(weaponPrefab, player.transform);
		WeaponOld weapon = newWeapon.GetComponent<WeaponOld>();
		// link weapon gauges
		weapon.LinkGaugeBar(WeaponGauges[player.playerNumber - 1].Find("Bar"));
		player.EquipWeapon(weapon);

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
			SpawnLocations[Random.Range(0, SpawnLocations.Length)].position;

		// Randomize weapon
		StartWeapon = Weapons[Random.Range(0, Weapons.Length)];

		// equip starter weapon
		CreatePlayerWeapon(player, StartWeapon);
	
		player.gameObject.SetActive(true);
		deaths ++;
	}


	void killEnemy(Enemy enemy){
		// TODO enemy death particles
		// TODO drop item

		// TODO use an object pool
		if(Random.Range(0f, 1f) > dropChance){
			Transform pickup = PickupPrefabs[Random.Range(0, PickupPrefabs.Length)];
			if(pickup.tag  != "Pickup"){
				pickup.GetComponent<WeaponPickup>()
					.type = (WeaponPickup.WeaponType) Random.Range(0, 3); 
			}
			Instantiate(
				pickup,
				enemy.transform.position,
				Quaternion.identity
			);
		}
		enemyCount --;
		if(enemyCount == 0){
			indicator.color = Color.green;
			float dur = (Time.time - startTime / 1000) / 60;
			string message = "Time: " + dur.ToString() + " minutes \nDeaths: " + deaths.ToString();
			canvas.gameObject.SetActive(true);
			text.text = message;
			respawnPlayers = false;
		}
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
