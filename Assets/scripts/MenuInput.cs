using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuInput : MonoBehaviour {

	public Text numPlayersText;
	GameSettings settings;
	void Start(){
		settings = GameSettings.GetInstance();
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.UpArrow)){
			settings.IncNumberOfPlayers();
			numPlayersText.text = settings.numberOfPlayers.ToString();
		} else if(Input.GetKeyDown(KeyCode.DownArrow)){
			settings.DecNumberOfPlayers();
			numPlayersText.text = settings.numberOfPlayers.ToString();
		} else if (Input.GetKeyDown(KeyCode.Return)){
			SceneManager.LoadScene(1);
		} else if (Input.GetKeyDown(KeyCode.Space)){
			SceneManager.LoadScene(2);
		}
	}
}
