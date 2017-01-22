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

    void OnConnect(int controllerID ) {
        var playerID = AirConsole.instance.ConvertDeviceIdToPlayerNumber( controllerID );
        AirConsole.instance.SetActivePlayers();
        var cameraController = GameObject.Find( "Main Camera" ).GetComponent<cameraController>();
        if (cameraController != null ) {
            cameraController.updateValues();
        }
    }

    void OnDisconnect(int controllerID) {
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
	                    try {
	                        var joystickPressed = (bool)data["joystick-left"]["pressed"];
	                        var horizontal = joystickPressed ? (float)data["joystick-left"]["message"]["x"] : 0;
	                        var vertical = joystickPressed ? (float)data["joystick-left"]["message"]["y"] : 0;
						playerController.rotateTowards( horizontal, vertical );
	                    }
	                    catch ( Exception e ) {
	                        print( e.ToString() );
	                    }
						
	                    //Right side
	                    try {
	                        var dPadDirection = (string)data["dpad-right"]["message"]["direction"];
							if (oldDpadDir != dPadDirection){
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
						}else{
							oldDpadDir = "";
						}
					
						}catch ( Exception e ) {
	                        print( e.ToString() );
	                    }
                    }
                }
                #endregion
                break;
            case GameState.Menu:
                //Right side
                try {
                    var dPadDirection = (string)data["dpad-right"]["message"]["direction"];
                    if ( oldDpadDir != dPadDirection ) {
                        oldDpadDir = dPadDirection;
                        var splashMenuManager = GameObject.Find( "BoatManager" );
                        if (splashMenuManager != null ) {
                            var manager = splashMenuManager.GetComponent<BoatManager>();
                            manager.PlayerSwitchedSelection( playerID, dPadDirection == "left" ? -1 : 1 );
                        }
                    }
                    else {
                        oldDpadDir = "";
                    }

                }
                catch ( Exception e ) {
                    print( e.ToString() );
                }
                if (AirConsole.instance.GetMasterControllerDeviceId() == controllerID) {
                    
                }
                break;
        }
    }
}
