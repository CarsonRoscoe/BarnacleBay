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
    private Dictionary<int, GameObject> Players = new Dictionary<int, GameObject>();

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
        if ( !Players.ContainsKey( playerID ) ) {
            Players.Add( playerID, player );
        }
        else {
            Players[playerID] = player;
        }
    }

    public void AddPlayer( int playerID ) {

    }

    public void RemovePlayer( int playerID ) {
        Players.Remove( playerID );
        PlayersTeam.Remove( playerID );

        var oneIsAlive = false;
        var twoIsAlive = false;
        foreach (var playerKey in PlayersTeam.Keys) {
            if (PlayersTeam[playerKey] == TeamSelection.One) {
                oneIsAlive = true;
            } else if (PlayersTeam[playerKey] == TeamSelection.Two) {
                twoIsAlive = true;
            }
        }
        if (oneIsAlive && !twoIsAlive) {
            print( "Team One WON" );
			Camera.main.GetComponent<cameraController> ().endGame ();
			StartCoroutine (ReturnToMenu ());
        } else if (!oneIsAlive && twoIsAlive) {
			print( "Team Two WON" );
			Camera.main.GetComponent<cameraController> ().endGame ();
			StartCoroutine (ReturnToMenu ());
        } else if (!oneIsAlive && !twoIsAlive && PlayersTeam.Keys.Count == 1) {
            print( "Player someone WON" );
			Camera.main.GetComponent<cameraController> ().endGame ();
			StartCoroutine (ReturnToMenu ());
        }
    }

    public GameObject GetPlayer( int playerID ) {
        if ( Players.ContainsKey( playerID ) ) {
            return Players[playerID];
        }
        return null;
    }

    public void ResetPlayerData() {
        Players.Clear();
    }

    public void SetGameState( GameState gameState ) {
        _gameState = gameState;
    }

    public void ReadyPlayers( Dictionary<int, TeamSelection> playersSelection ) {
        PlayersTeam = playersSelection;
    }

    public TeamSelection GetTeamSelection(int playerID) {
        return PlayersTeam[playerID];
    }

	IEnumerator ReturnToMenu() {
		yield return new WaitForSeconds (5f);
		SceneManager.LoadScene ("MenuScene");
	}
}
