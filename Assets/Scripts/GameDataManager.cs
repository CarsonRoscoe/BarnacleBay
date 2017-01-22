using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
}
