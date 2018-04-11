using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour {
	public static GM instance;
	
	[System.Serializable]
	public class PlayerHUD{
		public Transform healthGauge;
		public Transform coolDownGauge;
	}


	const int MAX_PLAYERS = 3;

	public SpriteRenderer indicator;
	public Canvas canvas;
	public Text text;


	[Range(0f, 1f)]
	public float dropChance = 0.4f;

	[Range(1, MAX_PLAYERS)]
	public uint numberOfPlayers = MAX_PLAYERS;
	public float respawnDelay = 2f;	
	public bool respawnPlayers = true;
	public bool friendlyFire = true;

	public CameraShake cameraShake;
	public Transform[] SpawnLocations;

	[SerializeField]
	PlayerHUD[] playerHUD = new PlayerHUD[MAX_PLAYERS];

	[Header("Prefabs")]
	public Transform PlayerPrefab;
	public Transform StartWeaponPrefab;
	public Transform[] DropItemPrefabs;


	AudioManager audioManager;
	float startTime = 0f;
	int deathCount;

	// Use this for initialization
	void Start () {
		if(instance == null){
			instance = this;
		}
		audioManager = GetComponent<AudioManager>();
		startTime = Time.time;
		deathCount = 0;
		spawnPlayers();
	}
	
	void Update(){
		if(Input.GetKeyDown(KeyCode.Backspace)){
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);	
		}
	}
	void spawnPlayers(){
		for(int i=0; i < numberOfPlayers; i++){
			Transform player = (Transform) Instantiate(
				PlayerPrefab,
				SpawnLocations[i].position,
				SpawnLocations[i].rotation
			);

			PlayerController playerCtrl = player.GetComponent<PlayerController>();
			playerCtrl.SetPlayerNumber(i+1);
			GameCharacter character = player.GetComponent<GameCharacter>();
			

			// enable gauges
			playerHUD[i].healthGauge.gameObject.SetActive(true);
			playerHUD[i].coolDownGauge.gameObject.SetActive(true);

			// link health bar
			character.healthSystem.LinkHealthBar(
				playerHUD[i].healthGauge.Find("Bar")
			);

			// link weapon
			playerHUD[i].coolDownGauge.gameObject.SetActive(true);
			CreatePlayerWeapon(playerCtrl, StartWeaponPrefab);
		}
	}

	public void CreatePlayerWeapon(PlayerController playerCtrl, Transform weaponPrefab){
		//create weapon
		Transform newWeapon = (Transform) Instantiate(weaponPrefab, playerCtrl.transform);
		Weapon weapon = newWeapon.GetComponent<Weapon>();

		// link weapon cooldown gauge
		weapon.LinkCoolDownBar(
			playerHUD[playerCtrl.playerNumber - 1]
			.coolDownGauge.Find("Bar")
		);

		// equip weapon to player
		playerCtrl.EquipWeapon(weapon);
	}


	void killPlayer(PlayerController playerCtrl){
		playerCtrl.gameObject.SetActive(false);
		if(respawnPlayers)
			StartCoroutine(respawnPlayer(playerCtrl));
	}


	IEnumerator respawnPlayer(PlayerController playerCtrl){
		yield return new WaitForSeconds(respawnDelay);
		playerCtrl.Reset();
		playerCtrl.transform.position = 
			SpawnLocations[Random.Range(0, SpawnLocations.Length)].position;

		// equip starter weapon
		CreatePlayerWeapon(playerCtrl, StartWeaponPrefab);

		playerCtrl.gameObject.SetActive(true);
		deathCount ++;
	}


	void killEnemy(Enemy enemy){
		// TODO enemy death particles
		// TODO drop item

		// TODO use an object pool
		// if(Random.Range(0f, 1f) > dropChance){
		// 	Transform pickup = DropItems[Random.Range(0, DropItems.Length)];
		// 	if(pickup.tag  != "Pickup"){
		// 		pickup.GetComponent<WeaponPickup>()
		// 			.type = (WeaponPickup.WeaponType) Random.Range(0, 3); 
		// 	}
		// 	Instantiate(
		// 		pickup,
		// 		enemy.transform.position,
		// 		Quaternion.identity
		// 	);
		// }
		// enemyCount --;
		// if(enemyCount == 0){
		// 	indicator.color = Color.green;
		// 	float dur = (Time.time - startTime / 1000) / 60;
		// 	string message = "Time: " + dur.ToString() + " minutes \ndeathCount: " + deathCount.ToString();
		// 	canvas.gameObject.SetActive(true);
		// 	text.text = message;
		// 	respawnPlayers = false;
		// }
		// Destroy(enemy.gameObject);
	}

	public static void PlayAudio(string name){
		instance.audioManager.PlaySound(name);
	}

	public static void KillCharacter(GameCharacter character){
		if(character.tag == "Player"){
			instance.killPlayer(character.GetComponent<PlayerController>());
		}else if(character.tag == "Enemy"){

		}
	}

	public static void ShakeCamera(float amount, float length){
		instance.cameraShake.Shake(amount, length);
	}
}