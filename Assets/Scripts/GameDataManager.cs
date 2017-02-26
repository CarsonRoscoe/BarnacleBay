using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public enum GameState { Menu, InGame }

public class GameDataManager : MonoBehaviour {
    public static GameDataManager instance;

    private GameState _gameState = GameState.Menu;
    public GameState GameState { get { return _gameState; } }

    private Dictionary<int, TeamSelection> PlayersTeam = new Dictionary<int, TeamSelection>();

    void Awake() {
        if ( instance == null ) {
            instance = this;
            DontDestroyOnLoad( gameObject );
        }
        else {
            Destroy( this );
        }
    }

    public void SetPlayer( int playerID, GameObject player ) {
        UserHandler.Player p = UserHandler.getInstance().getPlayerByID(playerID);
        if (p != null)
            p.playerObject = player;
    }

    public void RemovePlayer( int playerID ) {
        print(playerID);
        UserHandler.getInstance().playerEliminated(playerID, true);
        PlayersTeam.Remove( playerID );

        var oneIsAlive = false;
        var twoIsAlive = false;
        var someoneAlive = 0;
        foreach (UserHandler.Player p in UserHandler.getInstance().players) {
            if (p.playerObject != null && p.teamType == UserHandler.TeamType.LEFT) {
                oneIsAlive = true;
            } else if (p.playerObject != null && p.teamType == UserHandler.TeamType.RIGHT) {
                twoIsAlive = true;
            } else if (p.playerObject != null && p.teamType == UserHandler.TeamType.FFA) {
                someoneAlive++;
            }
        }
        print(string.Format("OneIsAlive: {0}, TwoisAlive: {1}, PlayerTeamCount: {2}", oneIsAlive, twoIsAlive, PlayersTeam.Keys.Count));
        if (oneIsAlive && !twoIsAlive) {
            print( "Team One WON" );
			Camera.main.GetComponent<cameraController> ().endGame ();
			StartCoroutine (ReturnToMenu ());
        } else if (!oneIsAlive && twoIsAlive) {
			print( "Team Two WON" );
			Camera.main.GetComponent<cameraController> ().endGame ();
			StartCoroutine (ReturnToMenu ());
        } else if (!oneIsAlive && !twoIsAlive && someoneAlive <= 1) {
            print( "Player someone WON" );
			Camera.main.GetComponent<cameraController> ().endGame ();
			StartCoroutine (ReturnToMenu ());
        }
    }
    

    public void SetGameState( GameState gameState ) {
        _gameState = gameState;
    }

    public void ReadyPlayers( Dictionary<int, TeamSelection> playersSelection ) {
        print("ReadyPlayers: " + playersSelection.Count);
        PlayersTeam = playersSelection;
    }

    public TeamSelection GetTeamSelection(int playerID) {
        return PlayersTeam[playerID];
    }

	IEnumerator ReturnToMenu() {
		yield return new WaitForSeconds (8f);
        SetGameState( GameState.Menu );
        PlayersTeam.Clear();
        AirConsoleManager.instance.setController(0, true, "splash");
        AirConsoleManager.instance.broadcastMessage("resetSelection", "");
        foreach (UserHandler.Player p in UserHandler.getInstance().players) {
            p.teamType = UserHandler.TeamType.FFA;
        }
		SceneManager.LoadScene ("MainMenu");
	}

	void Update() {
		var playSeagullesOrWaves = UnityEngine.Random.value <= .01f;
		if (playSeagullesOrWaves) {
			PlaySeagullesOrWaves ();
		}
	}

	void PlaySeagullesOrWaves() {
		var isSeagulls = UnityEngine.Random.value <= .5f;
		if (isSeagulls) {
			AudioManager.instance.playSound (AudioManager.SFXID.SEAGULLS);
		} else {
			AudioManager.instance.playSound (AudioManager.SFXID.WAVES);
		}
	}
}
