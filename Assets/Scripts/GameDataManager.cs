using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState { Menu, InGame }

public class GameDataManager : MonoBehaviour {
    public static GameDataManager instance;

    private GameState _gameState = GameState.InGame;
    public GameState GameState { get { return _gameState; } }

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
        return Players[playerID];
    }

    public void ResetPlayerData() {
        Players.Clear();
    }

    public void SetGameState(GameState gameState) {
        _gameState = gameState;
    }
}
