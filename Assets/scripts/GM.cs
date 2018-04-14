using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// This one is in need of refactoring, I added a bunch of new
// features as soon as I finished refactoring it, as things 
// sprang up. It handles a lot more than I want to, and that
// makes it less reusable in other projects. I'll fix it up
// once I give some thought to its roll.

/* But for now, this handles:
		Player Spawns and Respawns
		Killing Characters
		Keeping Player Scores
		Instantiating Equipped Weapons
		Dropping Items when Characters are killed
		Starting, Ending and Restarting the game		
*/
public class GM : MonoBehaviour {
	public static GM instance;

	[System.Serializable]
	public class PlayerHUD{
		public Transform healthGauge;
		public Transform coolDownGauge;
		public Text scoreText;
		public int score = 0;
		public int deathCount = 0;

		public PlayerHUD(){
			if(scoreText)
				scoreText.text = score.ToString();
		}
		public void incrementScore(){
			score ++;
			if(scoreText)
				scoreText.text = score.ToString();
		}

		public int getPoints(){
			return score - deathCount;
		}
	}

	public SpriteRenderer indicator;
	public Text text;


	[Range(0f, 1f)]
	public float dropChance = 0.6f;

	public float respawnDelay = 4f;	
	public bool respawnPlayers = true;
	public bool friendlyFire = true;

	public CameraShake cameraShake;
	public Transform[] SpawnLocations;

	[SerializeField]
	PlayerHUD[] playerHUD = new PlayerHUD[GameSettings.MAX_PLAYERS];

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
		}else if(Input.GetKeyDown(KeyCode.Escape)){
			SceneManager.LoadScene(0);
		}
	}
	void spawnPlayers(){
		for(int i=0; i < GameSettings.GetInstance().numberOfPlayers; i++){
			int r = Random.Range(0, SpawnLocations.Length);
			Transform player = (Transform) Instantiate(
				PlayerPrefab,
				SpawnLocations[r].position,
				SpawnLocations[r].rotation
			);

			PlayerController playerCtrl = player.GetComponent<PlayerController>();
			playerCtrl.SetPlayerNumber(i+1);
			GameCharacter character = player.GetComponent<GameCharacter>();
			

			// enable HUD
			playerHUD[i].healthGauge.gameObject.SetActive(true);
			playerHUD[i].coolDownGauge.gameObject.SetActive(true);
			playerHUD[i].scoreText.gameObject.SetActive(true);
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

	void dropRandomItem(Vector3 position){
		if(Random.Range(0f, 1f) > dropChance){
			Transform item = DropItemPrefabs[Random.Range(0, DropItemPrefabs.Length)];
			Instantiate(item, position, Quaternion.identity);
		}
	}

	string getMVP(){
		string mvp = "player1";
		int pts = playerHUD[0].getPoints();
		for(int i = 1; i < GameSettings.GetInstance().numberOfPlayers;i++){
			if(playerHUD[i].getPoints() > pts){
				mvp = "player" + (i + 1).ToString();
				pts = playerHUD[i].getPoints();
			}
		}
		return mvp;
	}

	string getMostDeaths(){
		string mostDeaths = "player1";
		int deaths = playerHUD[0].deathCount;
		for(int i = 1; i < GameSettings.GetInstance().numberOfPlayers;i++){
			if(playerHUD[i].deathCount > deaths){
				mostDeaths = "player" + (i + 1).ToString();
				deaths = playerHUD[i].deathCount;
			}
		}
		return mostDeaths;
	}

	void endGame(){
		float time = (Time.time - startTime) / 60;
		if(text){
			text.gameObject.SetActive(true);
			text.text = "Time: " + time + " minutes\nDeaths: " + deathCount
				+ "\nMVP: " + getMVP()
				+ "\nMOST DEATHS: " + getMostDeaths();

		}
		indicator.color =  Color.green;
		respawnPlayers = false;
	}

	void incrementScore(int playerNumber){
		playerHUD[playerNumber - 1].incrementScore();
	}

	public static void PlayAudio(string name){
		instance.audioManager.PlaySound(name);
	}

	public static void KillCharacter(GameCharacter character){
		if(character.tag == "Player"){
			int playerNum = int.Parse(character.name.Substring(character.name.Length - 1));
			instance.playerHUD[playerNum - 1].deathCount ++;
			instance.killPlayer(character.GetComponent<PlayerController>());
			instance.dropRandomItem(character.transform.position);
		}else if(character.tag == "Enemy"){
			string killer = character.killer;
			if(killer.StartsWith("player")){
				instance.incrementScore(int.Parse(killer.Substring(killer.Length - 1)));
			}
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
