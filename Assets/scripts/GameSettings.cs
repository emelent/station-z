using UnityEngine;

[System.Serializable]
public class GameSettings{

	static GameSettings instance;
	public static int MAX_PLAYERS = 3; 

	public int numberOfPlayers = 1;

	private GameSettings(){
		if(instance == null){
			instance = this;
		}
	}

	public void IncNumberOfPlayers(){
		instance.numberOfPlayers = Mathf.Min(MAX_PLAYERS, instance.numberOfPlayers + 1);
	}

	public void DecNumberOfPlayers(){
		instance.numberOfPlayers = Mathf.Max(1, instance.numberOfPlayers - 1);
	}

	public static GameSettings GetInstance(){
		if(instance == null){
			instance = new GameSettings();
		}
		return instance;
	}


}