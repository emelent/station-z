using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

	public static GameMaster instance;
	CameraShake cameraShake;
	AudioManager audioManager;

	// Use this for initialization
	void Start () {
		if(instance == null){
			instance = this;
		}
		audioManager = GetComponent<AudioManager>();
		cameraShake = Camera.main.GetComponent<CameraShake>();
	}
	
	void killPlayer(GameObject gameObj){
		Player player = gameObj.GetComponent<Player>();
		player.Reset();
		gameObj.SetActive(false);
		// Destroy(gameObj);
	}

	void killEnemy(GameObject gameObj){
		Destroy(gameObj);
	}

	public static void PlayAudio(string name){
		instance.audioManager.PlaySound(name);
	}

	public static void Kill(GameObject gameObj){
		if(gameObj.tag == "Player")
			instance.killPlayer(gameObj);
		else if(gameObj.tag == "Enemy")
			instance.killEnemy(gameObj);
		else
			Destroy(gameObj);
		
		// do death particles
		
	}

	public static void ShakeCamera(float amount, float length){
		instance.cameraShake.Shake(amount, length);
	}
}
