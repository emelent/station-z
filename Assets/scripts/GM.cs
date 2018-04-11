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
	public float respawnDelay = 4f;	
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
			int r = Random.Range(0, SpawnLocations.Length);
			Transform player = (Transform) Instantiate(
				PlayerPrefab,
				SpawnLocations[r].position,
				SpawnLocations[r].rotation
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
		print("killing player");
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

	void dropRandomItem(Vector3 position){
		if(Random.Range(0f, 1f) > dropChance){
			Transform item = DropItemPrefabs[Random.Range(0, DropItemPrefabs.Length)];
			Instantiate(item, position, Quaternion.identity);
			print("item dropped");
		}else{
			print("no item dropped");
		}
	}

	void endGame(){
		float time = (Time.time - startTime) / 60;
		if(canvas && text){
			canvas.gameObject.SetActive(true);
			text.text = "Time: " + time + " minutes\nDeaths: " + deathCount;
		}
	}

	public static void PlayAudio(string name){
		instance.audioManager.PlaySound(name);
	}

	public static void KillCharacter(GameCharacter character){
		if(character.tag == "Player"){
			instance.killPlayer(character.GetComponent<PlayerController>());
			instance.dropRandomItem(character.transform.position);
		}else if(character.tag == "Enemy"){
			instance.dropRandomItem(character.transform.position);
			Destroy(character.gameObject);
		}
	}

	public static void ShakeCamera(float amount, float length){
		instance.cameraShake.Shake(amount, length);
	}

	public static void EndGame(){
		instance.endGame();
	}
}
