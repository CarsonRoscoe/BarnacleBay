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

    //used for determining player colors
    public enum PlayerColors { PINK, PURPLE, BLUE, TEAL, GREEN, LIME, YELLOW, ORANGE, RED, WHITE };
    private int[] playerColorsCount = new int[10];

    void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onDisconnect += OnDisconnect;

    }

    void OnConnect( int controllerID ) {
        UserHandler.Player p = UserHandler.getInstance().addPlayer(controllerID, AirConsole.instance.GetUID(controllerID), getNewPlayerColor());
        var cameraController = GameObject.Find( "Main Camera" ).GetComponent<cameraController>();
        if ( cameraController != null ) {
            cameraController.updateValues();
        }
    }

    void OnDisconnect( int controllerID ) {
        UserHandler.getInstance().deletePlayer(controllerID);
        var cameraController = GameObject.Find( "Main Camera" ).GetComponent<cameraController>();
        if ( cameraController != null ) {
            cameraController.updateValues();
        }
    }

    public void removePlayerData(int playerID) {
        var missingID = 0;
        /*foreach (var pp in players) {
            if (pp.playerID == -1)
                missingID = pp.playerID;
        }
        for (var i = 0; i < players.Count; i++) {
            if (players[i].playerID == missingID) {
                players.RemoveAt(i);
            }
        }*/
        if (PlayerHUDHandler.instance != null)
            PlayerHUDHandler.instance.loadList();
    }

    public void resetGame() {
        /*foreach (var p in players) {
            p.resetPlayer();
        }*/
    }

    /*public PlayerData addPlayerData(int playerID) {
        string name = AirConsole.instance.GetNickname(playerID) + " : " + playerID;
        //PlayerData d = new PlayerData(playerID, name, (PlayerData.PlayerColors)players.Count);
        //players.Add(d);
        if (PlayerHUDHandler.instance != null)
            PlayerHUDHandler.instance.loadList();
        return d;
    }*/

    public Color getNewPlayerColor() {
        var lowestIndex = 0;
        for (int i = 1; i < playerColorsCount.Length; i++) {
            if (playerColorsCount[i - 1] > playerColorsCount[i])
                lowestIndex = i;
        }
        playerColorsCount[lowestIndex]++;
        float constSat = 170f / 255f;
        switch (lowestIndex) {
            case 0: return Color.HSVToRGB(300 / 360f, constSat, 1, true);
            case 1: return Color.HSVToRGB(265 / 360f, constSat, 1, true);
            case 2: return Color.HSVToRGB(233 / 360f, constSat, 1, true);
            case 3: return Color.HSVToRGB(200 / 360f, constSat, 1, true);
            case 4: return Color.HSVToRGB(166 / 360f, constSat, 1, true);
            case 5: return Color.HSVToRGB(133 / 360f, constSat, 1, true);
            case 6: return Color.HSVToRGB(66 / 360f, constSat, 1, true);
            case 7: return Color.HSVToRGB(33 / 360f, constSat, 1, true);
            case 8: return Color.HSVToRGB(0 / 360f, constSat, 1, true);
            case 9: return Color.HSVToRGB(0 / 360f, 0, 1, true);
        }
        //should never return this.
        return Color.HSVToRGB(0, 0, 0, true);
    }

    void OnMessage( int controllerID, JToken data ) {
        Dictionary<string, string> result = data.ToObject<Dictionary<string, string>>();
        foreach (var item in result) {
            EnactAction(controllerID, item.Key, item.Value);
        }
    }

    void EnactAction(int id, string key, string value) {
        /*var playerID = AirConsole.instance.ConvertDeviceIdToPlayerNumber(id);
        switch (GameDataManager.instance.GameState) {
            case GameState.InGame:
                #region inGame Control Ship
                var player = GameDataManager.instance.GetPlayer(playerID);
                if (player != null) {
                    var playerController = player.GetComponent<shipController>();
                    if (playerController != null) {
                        //Left Side
                        var joystickleft = data["joystick-left"];
                        if (joystickleft != null) {
                            var joystickPressed = joystickleft["pressed"] != null ? (bool)joystickleft["pressed"] : false;
                            var message = joystickleft["message"];
                            if (message != null && joystickPressed) {
                                var horizontal = (float)joystickleft["message"]["x"];
                                var vertical = (float)joystickleft["message"]["y"];
                                playerController.rotateTowards(horizontal, vertical);
                            } else {
                                playerController.rotateTowards(0, 0);
                            }
                        }

                        //Right side
                        var dpad = data["dpad-right"];
                        if (dpad != null) {
                            var message = dpad["message"];
                            if (message != null) {
                                var dPadDirection = (string)message["direction"];
                                if (oldDpadDir != dPadDirection) {
                                    oldDpadDir = dPadDirection;
                                    switch (dPadDirection) {
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
                                } else {
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
                        var moveBoat = GameObject.Find("Main Camera").GetComponent<MoveToBoat>();
                        if (!BoatManager.instance.InTeamSelectMode) {
                            moveBoat.MoveToNewPosition();
                            BoatManager.instance.InTeamSelectMode = true;
                        } else {
                            moveBoat.StartGame();
                        }
                    }
                }

                var dpadmenu = data["dpad-right"];
                if (dpadmenu != null) {
                    var message = dpadmenu["message"];
                    if (message != null) {
                        var dPadDirection = (string)message["direction"];
                        if (oldDpadDir != dPadDirection) {
                            oldDpadDir = dPadDirection;
                            switch (dPadDirection) {
                                case "left":
                                case "right":
                                    //Make it have a "Team One", "Free For All" and "Team Two" menu buttons
                                    BoatManager.instance.SetPlayerTeamSelection(playerID, dPadDirection == "left" ? -1 : 1);
                                    break;
                                case "up":
                                    BoatManager.instance.SetPlayerTeamSelection(playerID, 0);
                                    break;
                            }
                        } else {
                            oldDpadDir = "";
                        }
                    }
                }

                if (AirConsole.instance.GetMasterControllerDeviceId() == controllerID) {

                }
                break;
        }
        */
    }
   
}
