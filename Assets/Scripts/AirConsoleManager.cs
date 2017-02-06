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
        PlayerData d = addPlayerData(controllerID);
        foreach (var p in players) {
            AirConsole.instance.Message(p.playerID, PlayerData.hexColor(p.color));
        }
        var cameraController = GameObject.Find( "Main Camera" ).GetComponent<cameraController>();
        if ( cameraController != null ) {
            cameraController.updateValues();
        }
    }

    void OnDisconnect( int controllerID ) {
        AirConsole.instance.SetActivePlayers(8);
        var playerID = AirConsole.instance.ConvertDeviceIdToPlayerNumber( controllerID );
        removePlayerData(controllerID);
        foreach (var p in AirConsole.instance.GetActivePlayerDeviceIds) {
            AirConsole.instance.Message(p, PlayerData.hexColor((PlayerData.PlayerColors)AirConsole.instance.ConvertDeviceIdToPlayerNumber(p)));
        }
        var cameraController = GameObject.Find( "Main Camera" ).GetComponent<cameraController>();
        if ( cameraController != null ) {
            cameraController.updateValues();
        }
    }

    public void removePlayerData(int playerID) {
        var missingID = 0;
        foreach (var pp in players) {
            if (pp.playerID == -1)
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

    public PlayerData addPlayerData(int playerID) {
        string name = AirConsole.instance.GetNickname(playerID) + " : " + playerID;
        PlayerData d = new PlayerData(playerID, name, (PlayerData.PlayerColors)players.Count);
        players.Add(d);
        if (PlayerHUDHandler.instance != null)
            PlayerHUDHandler.instance.loadList();
        return d;
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
							} else{
								playerController.rotateTowards(0,0);
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
						var moveBoat = Camera.main.GetComponent<MoveToBoat> ();
						if (!BoatManager.instance.InTeamSelectMode) {
							moveBoat.MoveToNewPosition ();
							BoatManager.instance.InTeamSelectMode = true;
						} else {
							moveBoat.StartGame ();
						}
                    }
                }
				
				var dpadmenu = data["dpad-right"];
				if (dpadmenu != null ) {
					var message = dpadmenu["message"];
					if (message != null ) {
						var dPadDirection = (string)message["direction"];
						if ( oldDpadDir != dPadDirection ) {
							oldDpadDir = dPadDirection;
							switch ( dPadDirection ) {
							case "left":
							case "right":
								//Make it have a "Team One", "Free For All" and "Team Two" menu buttons
								BoatManager.instance.SetPlayerTeamSelection( playerID, dPadDirection == "left" ? -1 : 1 );
								break;
							case "up":
								BoatManager.instance.SetPlayerTeamSelection( playerID, 0 );
								break;
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
