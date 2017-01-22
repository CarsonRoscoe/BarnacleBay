using UnityEngine;
using System.Collections;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;

public class AirConsoleManager : MonoBehaviour {
    private string oldDpadDir;
    void Start() {
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onDisconnect += OnDisconnect;
    }

    void OnConnect( int controllerID ) {
        var playerID = AirConsole.instance.ConvertDeviceIdToPlayerNumber( controllerID );
        AirConsole.instance.SetActivePlayers();
        var cameraController = GameObject.Find( "Main Camera" ).GetComponent<cameraController>();
        if ( cameraController != null ) {
            cameraController.updateValues();
        }
    }

    void OnDisconnect( int controllerID ) {
        var playerID = AirConsole.instance.ConvertDeviceIdToPlayerNumber( controllerID );
        AirConsole.instance.SetActivePlayers();
        var cameraController = GameObject.Find( "Main Camera" ).GetComponent<cameraController>();
        if ( cameraController != null ) {
            cameraController.updateValues();
        }
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
                var dpadright = data["dpad-right"];
                if ( dpadright != null ) {
                    var message = dpadright["message"];
                    if ( message != null ) {
                        var dPadDirection = (string)message["direction"];
                        if ( oldDpadDir != dPadDirection ) {
                            oldDpadDir = dPadDirection;
                            var splashMenuManager = GameObject.Find( "BoatManager" );
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
