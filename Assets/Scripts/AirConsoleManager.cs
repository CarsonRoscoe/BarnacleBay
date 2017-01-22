using UnityEngine;
using System.Collections;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public class AirConsoleManager : MonoBehaviour {
    public static AirConsoleManager instance;
    private string oldDpadDir;
    public List<PlayerData> players;

    void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onDisconnect += OnDisconnect;
    }

    void Start() {
         players = new List<PlayerData>();
    }

    void OnConnect( int controllerID ) {
        AirConsole.instance.SetActivePlayers(8);
        var playerID = AirConsole.instance.ConvertDeviceIdToPlayerNumber(controllerID);
        addPlayerData(playerID);
        var cameraController = GameObject.Find( "Main Camera" ).GetComponent<cameraController>();
        if ( cameraController != null ) {
            cameraController.updateValues();
        }
    }

    void OnDisconnect( int controllerID ) {
        AirConsole.instance.SetActivePlayers(8);
        var playerID = AirConsole.instance.ConvertDeviceIdToPlayerNumber( controllerID );
        removePlayerData(controllerID);
        var cameraController = GameObject.Find( "Main Camera" ).GetComponent<cameraController>();
        if ( cameraController != null ) {
            cameraController.updateValues();
        }
    }

    public void getPlayerDataArray() {
        List<int> playerIDs = AirConsole.instance.GetActivePlayerDeviceIds.ToList<int>();
        foreach (var p in playerIDs) {
            int pid = AirConsole.instance.ConvertDeviceIdToPlayerNumber(p);
            string name = AirConsole.instance.GetNickname(p);            
            players.Add(new PlayerData(pid, name));
        }
        if (PlayerHUDHandler.instance != null)
            PlayerHUDHandler.instance.loadList();
    }

    public void removePlayerData(int playerID) {
        var missingID = 0;
        foreach (var pp in players) {
            if (AirConsole.instance.ConvertPlayerNumberToDeviceId(pp.playerID) == -1)
                missingID = pp.playerID;
        }
        for (var i = 0; i < players.Count; i++) {
            if (players[i].playerID == missingID) {
                players.RemoveAt(i);
            }
        }
        if (PlayerHUDHandler.instance != null)
            PlayerHUDHandler.instance.loadList();
    }

    public void resetGame() {
        foreach (var p in players) {
            p.resetPlayer();
        }
    }

    public void addPlayerData(int playerID) {
        string name = AirConsole.instance.GetNickname(AirConsole.instance.ConvertPlayerNumberToDeviceId(playerID));
        players.Add(new PlayerData(playerID, name));
        if (PlayerHUDHandler.instance != null)
            PlayerHUDHandler.instance.loadList();
    }

    void OnMessage( int controllerID, JToken data ) {
        var playerID = AirConsole.instance.ConvertDeviceIdToPlayerNumber( controllerID );
        switch ( GameDataManager.instance.GameState ) {
            case GameState.InGame:
                #region inGame Control Ship
                var player = GameDataManager.instance.GetPlayer( playerID );
                if ( player != null ) {
                    var playerController = player.GetComponent<shipController>();
                    if ( playerController != null ) {
                        //Left Side
                        var joystickleft = data["joystick-left"];
                        if ( joystickleft != null ) {
                            var joystickPressed = joystickleft["pressed"] != null ? (bool)joystickleft["pressed"] : false;
                            var message = joystickleft["message"];
                            if ( message != null && joystickPressed ) {
                                var horizontal = (float)joystickleft["message"]["x"];
                                var vertical = (float)joystickleft["message"]["y"];
                                playerController.rotateTowards( horizontal, vertical );
                            }
                        }

                        //Right side
                        var dpad = data["dpad-right"];
                        if (dpad != null ) {
                            var message = dpad["message"];
                            if (message != null ) {
                                var dPadDirection = (string)message["direction"];
                                if ( oldDpadDir != dPadDirection ) {
                                    oldDpadDir = dPadDirection;
                                    switch ( dPadDirection ) {
                                        case "left":
                                            playerController.fireLeft();
                                            break;
                                        case "right":
                                            playerController.fireRight();
                                            break;
                                        case "up":
                                            playerController.boost();
                                            break;
                                        case "down":
                                            playerController.brake();
                                            break;
                                    }
                                }
                                else {
                                    oldDpadDir = "";
                                }
                            }
                        }
                    }
                }
                #endregion
                break;
            case GameState.Menu:
                    //Right side
                if (data["Start"] != null) {
                    if (controllerID == AirConsole.instance.GetMasterControllerDeviceId()) {
                        var splashMenuManager = GameObject.Find("SplashMenuManager");
                        if (splashMenuManager != null) 
                            splashMenuManager.GetComponent<SplashMenuManager>().GoToPlaySetup();
                        else {
                            var boatManager = GameObject.Find("BoatManager");
                            if (boatManager != null)
                                boatManager.GetComponent<BoatManager>().StartGame();
                        }
                    }
                }
                var dpadright = data["dpad-right"];
                if ( dpadright != null ) {
                    var message = dpadright["message"];
                    if ( message != null ) {
                        var dPadDirection = (string)message["direction"];
                        if ( oldDpadDir != dPadDirection ) {
                            oldDpadDir = dPadDirection;
                            var splashMenuManager = GameObject.Find("BoatManager");
                            if ( splashMenuManager != null ) {
                                var manager = splashMenuManager.GetComponent<BoatManager>();
                                manager.PlayerSwitchedSelection( playerID, dPadDirection == "left" ? -1 : 1 );
                            }
                        }
                        else {
                            oldDpadDir = "";
                        }
                    }
                }

                if ( AirConsole.instance.GetMasterControllerDeviceId() == controllerID ) {

                }
                break;
        }
    }
}
