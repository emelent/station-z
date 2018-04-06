using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

	public static GameMaster instance;

	AudioManager audioManager;

	// Use this for initialization
	void Start () {
		if(instance == null){
			instance = this;
		}
		audioManager = GetComponent<AudioManager>();
	}
	
	
	public static void PlayAudio(string name){
		instance.audioManager.PlaySound(name);
	}
}
