using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class WaveSpawner : MonoBehaviour {

	public enum SpawnState { SPAWNING, WAITING, COUNTING };

	[System.Serializable]
	public class Wave
	{
		public string name;
		public Transform enemy;
		public int count;
		public float rate;
	}


	public Wave[] waves;
	public Text waveTitle;

	private int nextWave = 0;
	public int NextWave
	{
		get { return nextWave + 1; }
	}

	public Transform[] spawnPoints;

	public float timeBetweenWaves = 5f;
	private float waveCountdown;
	public float WaveCountdown
	{
		get { return waveCountdown; }
	}

	private float searchCountdown = 1f;
	bool autoTargetting = false;
	private SpawnState state = SpawnState.COUNTING;
	public SpawnState State
	{
		get { return state; }
	}

	void Start()
	{
		waveCountdown = timeBetweenWaves;
	}

	void Update()
	{
		if (state == SpawnState.WAITING){
			if (!EnemyIsAlive())
			{
				WaveCompleted();
			}
			else
			{
				return;
			}
		}

		if (waveCountdown <= 0){	
			if(waveTitle)
				waveTitle.gameObject.SetActive(false);
			if (state != SpawnState.SPAWNING)
			{
				StartCoroutine( SpawnWave ( waves[nextWave] ) );
			}
		}
		else
		{
			if(waveTitle){
				waveTitle.gameObject.SetActive(true);
				waveTitle.text = "WAVE " + NextWave.ToString() + " IN ...\n" + ((int)waveCountdown).ToString();
			}
			waveCountdown -= Time.deltaTime;
		}
	}

	void WaveCompleted()
	{
		state = SpawnState.COUNTING;
		waveCountdown = timeBetweenWaves;

		if (nextWave + 1 > waves.Length - 1)
		{
			// nextWave = 0;
			state = SpawnState.WAITING;
			GM.EndGame();
			gameObject.SetActive(false);
		}
		else
		{
			nextWave++;
		}
	}

	bool EnemyIsAlive()
	{
		searchCountdown -= Time.deltaTime;
		if (searchCountdown <= 0f)
		{
			searchCountdown = 1f;
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			if (enemies.Length == 0){
				return false;
			}else if(enemies.Length < 7 && state == SpawnState.WAITING &&  !autoTargetting){ 
				// the last few enemies auto target players
				autoTargetting = true;
				print("Auto  targeting");
				GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
				for(int i=0; i < enemies.Length; i++){
					ZombieAI z = enemies[i].GetComponent<ZombieAI>();
					z.SetTarget(players[Random.Range(0, players.Length)].transform);
					
				}
			}
		}
		return true;
	}

	IEnumerator SpawnWave(Wave _wave)
	{
		state = SpawnState.SPAWNING;
		autoTargetting = false;
		for (int i = 0; i < _wave.count; i++)
		{
			SpawnEnemy(_wave.enemy);
			yield return new WaitForSeconds( 1f/_wave.rate );
		}

		state = SpawnState.WAITING;

		yield break;
	}

	void SpawnEnemy(Transform _enemy)
	{
		Transform _sp = spawnPoints[ Random.Range (0, spawnPoints.Length) ];

		// zero out the z axis coz inherits from the parents
		_sp.position = new Vector3(
			_sp.position.x,
			_sp.position.y,
			0
		);
		Instantiate(_enemy, _sp.position, _sp.rotation);
	}

}
