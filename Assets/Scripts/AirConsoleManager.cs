﻿using UnityEngine;
using System.Collections;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class AirConsoleManager : MonoBehaviour {
    public static AirConsoleManager instance;
    private string oldDpadDir;

    //used for determining player colors
    //public enum PlayerColors { PINK, PURPLE, BLUE, TEAL, GREEN, LIME, YELLOW, ORANGE, RED, WHITE };
    private int[] playerColorsCount = new int[8];

    public const int MAX_PLAYERS = 8;

    void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this);
            return;
        }
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onDisconnect += OnDisconnect;
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            var moveBoat = GameObject.Find("Main Camera").GetComponent<MoveToBoat>();
            moveBoat.StartGame();
        }
    }

    void OnConnect( int controllerID ) {
        if(UserHandler.getInstance().players.Count >= MAX_PLAYERS)
            return;
        UserHandler.Player player = UserHandler.getInstance().addPlayer(controllerID, AirConsole.instance.GetUID(controllerID), getNewPlayerColor());
        if(player != null) {
            setController(controllerID);
            sendMessage(controllerID, "setControllerColor", colorToHex(player.color));
            var cameraController = GameObject.Find("Main Camera").GetComponent<cameraController>();
            if(cameraController != null) {
                cameraController.updateValues();
            }

            if(GameDataManager.instance.GameState == GameState.InGame) {
                BoatManager.instance.SetPlayerTeamSelection(controllerID, GameDataManager.instance.GameType == GameTeamType.FFA ? TeamSelection.FreeForAll : TeamSelection.One);
                GameDataManager.instance.ReadyPlayers(BoatManager.instance.PlayersSelection);
            }
        } else {
            print(string.Format("Could not create player-controller ID {0}", controllerID));
        }
    }

    void OnDisconnect( int controllerID ) {
        print(string.Format("On AirConsoleManager->OnDisconnect, controllerID {0}", controllerID));
        //Remove them from menu data
        BoatManager.instance.RemovePlayerFromTeamSelection(AirConsole.instance.ConvertDeviceIdToPlayerNumber(controllerID));
        var userHandler = UserHandler.getInstance();
        if(userHandler != null) {
            var player = userHandler.getPlayerByID(controllerID);
            if(player != null) {
                removePlayerColor(player.color);

                //If we are in a game, kill the players boat
                if(GameDataManager.instance.GameState == GameState.InGame) {
                    player.playerObject.GetComponent<shipController>().Health = 0;
                }
            } else {
                print(string.Format("Error: AirConsoleManager->OnDisconnect, player controller {0} is null", controllerID));
            }

            //Delete player from UserHandler
            if(!userHandler.deletePlayer(controllerID)) {
                print(string.Format("Error: AirConsoleManager->OnDisconnect, cannot delete controller {0}", controllerID));
            }

            var cameraController = GameObject.Find("Main Camera").GetComponent<cameraController>();
            if(cameraController != null) {
                cameraController.updateValues();
            }
        }

    }

    public void setController( int deviceID, bool broadcast = false, string control = null ) {
        string sceneName = SceneManager.GetActiveScene().name;
        string controller = "splash";
        if(control == null) {
            if(sceneName.Equals("MainMenu")) {
                if(!BoatManager.instance.InTeamSelectMode)
                    controller = "splash";
                else
                    controller = "menu";
            } else if(sceneName.Equals("GameScene")) {
                controller = "game";
            }
        } else {
            controller = control;
        }

        if(broadcast)
            broadcastMessage("setControllerType", controller);
        else
            sendMessage(deviceID, "setControllerType", controller);
    }

    public void removePlayerData( int playerID ) {
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
        if(PlayerHUDHandler.instance != null)
            PlayerHUDHandler.instance.loadList();
    }

    private string colorToHex( Color c ) {
        return ((byte)(c.r * 255f)).ToString("X2") + ((byte)(c.g * 255f)).ToString("X2") + ((byte)(c.b * 255f)).ToString("X2");
    }

    public void updateHealth( int id, int hp ) {
        JObject data = JObject.Parse(@"{ 'updateHealth': " + hp + "}");
        AirConsole.instance.Message(id, data);
    }

    public void resetGame() {
        /*foreach (var p in players) {
            p.resetPlayer();
        }*/
    }

    public void broadcastMessage( string key, string value ) {
        JObject data = JObject.Parse(@"{ '" + key + "': '" + value + "'}");
        AirConsole.instance.Broadcast(data);
    }

    public void sendMessage( int ID, string key, string value ) {
        JObject data = JObject.Parse(@"{ '" + key + "': '" + value + "'}");
        AirConsole.instance.Message(ID, data);
    }

    /*public PlayerData addPlayerData(int playerID) {
        string name = AirConsole.instance.GetNickname(playerID) + " : " + playerID;
        if (PlayerHUDHandler.instance != null)
            PlayerHUDHandler.instance.loadList();
    }*/

    public void removePlayerColor( Color color ) {
        var colorIndex = getIndexByColor(color);
        playerColorsCount[colorIndex]--;
        if(playerColorsCount[colorIndex] < 0) {
            playerColorsCount[colorIndex] = 0;
        }
    }

    public Color getNewPlayerColor() {
        var lowestIndex = 0;
        for(int i = 1; i < playerColorsCount.Length; i++) {
            if(playerColorsCount[i - 1] > playerColorsCount[i])
                lowestIndex = i;
        }
        playerColorsCount[lowestIndex]++;
        float constSat = 170f / 255f;
        switch(lowestIndex) {
            case 0:
                return new Color(0.0f, 0.8f, 0.0f);//Color.HSVToRGB(300 / 360f, constSat, 1, true);
            case 1:
                return new Color(85f / 255f, 26 / 255f, 139 / 255f);//Color.HSVToRGB(265 / 360f, constSat, 1, true);
            case 2:
                return new Color(1.0f, 116 / 256f, 0.0f);//Color.HSVToRGB(233 / 360f, constSat, 1, true);
            case 3:
                return new Color(0.0f, 1.0f, 1.0f);//Color.HSVToRGB(200 / 360f, constSat, 1, true);
            case 4:
                return new Color(216f / 255f, 0.0f, 0.0f);//Color.HSVToRGB(166 / 360f, constSat, 1, true);
            case 5:
                return new Color(1.0f, 215f / 255f, 0.0f);//Color.HSVToRGB(133 / 360f, constSat, 1, true);
            case 6:
                return new Color(201f / 255f, 201f / 255f, 201f / 255f);//Color.HSVToRGB(66 / 360f, constSat, 1, true);
            case 7:
                return new Color(72f / 255f, 72f / 255f, 72f / 255f);//Color.HSVToRGB(33 / 360f, constSat, 1, true);
            case 8:
                return Color.HSVToRGB(0 / 360f, constSat, 1, true);
            case 9:
                return Color.HSVToRGB(0 / 360f, 0, 1, true);
        }
        //should never return this.
        return Color.HSVToRGB(0, 0, 0, true);
    }

    public int getIndexByColor( Color color ) {
        if(color == new Color(0.0f, 0.8f, 0.0f)) {
            return 0;
        } else if(color == new Color(85f / 255f, 26 / 255f, 139 / 255f)) {
            return 1;
        } else if(color == new Color(1.0f, 116 / 256f, 0.0f)) {
            return 2;
        } else if(color == new Color(0.0f, 1.0f, 1.0f)) {
            return 3;
        } else if(color == new Color(216f / 255f, 0.0f, 0.0f)) {
            return 4;
        } else if(color == new Color(1.0f, 215f / 255f, 0.0f)) {
            return 5;
        } else if(color == new Color(201f / 255f, 201f / 255f, 201f / 255f)) {
            return 6;
        } else if(color == new Color(72f / 255f, 72f / 255f, 72f / 255f)) {
            return 7;
        } else if(color == Color.HSVToRGB(0 / 360f, 170f / 255f, 1, true)) {
            return 8;
        } else if(color == Color.HSVToRGB(0 / 360f, 0, 1, true)) {
            return 9;
        }
        return 10;
    }

    void OnMessage( int controllerID, JToken data ) {
        Dictionary<string, string> result = data.ToObject<Dictionary<string, string>>();
        foreach(var item in result) {
            EnactAction(controllerID, item.Key, item.Value);
        }
    }

    void EnactAction( int id, string key, string value ) {
        //Debug.Log( "Recieved Message: " + key + " : " + value );

        if(key.Equals("readySplash") && value.Equals("True")) {
            var moveBoat = GameObject.Find("Main Camera").GetComponent<MoveToBoat>();
            if(!BoatManager.instance.InTeamSelectMode) {
                moveBoat.MoveToNewPosition();
                BoatManager.instance.InTeamSelectMode = true;
                setController(-1, true);
                broadcastMessage("forceEnable", "ready");
            }
        }

        if((key.Equals("ready") || key.Equals("readyLeft") || key.Equals("readyRight")) && value.Equals("True")) {
            var moveBoat = GameObject.Find("Main Camera").GetComponent<MoveToBoat>();
            UserHandler.Player p = UserHandler.getInstance().getPlayerByID(id);
            if(p == null)
                return;
            switch(key) {
                case "ready":
                    p.teamType = UserHandler.TeamType.FFA;
                    break;
                case "readyLeft":
                    p.teamType = UserHandler.TeamType.LEFT;
                    break;
                case "readyRight":
                    p.teamType = UserHandler.TeamType.RIGHT;
                    break;
            }
            if(UserHandler.getInstance().allPlayersReady() /*&& UserHandler.getInstance().gameType != UserHandler.GameType.NONE_SET*/) { //remove this comment for tournaments.
                int gameType = -1; //-1 = none set yet, 1 = ffa, 2 = left & right teams. if two conflicting values (ignoring -1) found, do not start game.
                foreach(UserHandler.Player pp in UserHandler.getInstance().players) {
                    if(pp.teamType == UserHandler.TeamType.FFA) {
                        if(gameType == -1)
                            gameType = 1;
                        else if(gameType == 2)
                            return;
                    } else {
                        if(gameType == -1)
                            gameType = 2;
                        else if(gameType == 1)
                            return;
                    }
                }
                moveBoat.StartGame();
            }
        }

        if(key.Equals("readyTeam") && value.Equals("True")) {
            UserHandler.Player p = UserHandler.getInstance().getPlayerByID(id);
            if(p == null)
                return;
            p.readyToPlay = true;
            if(UserHandler.getInstance().allPlayersReady() /*&& UserHandler.getInstance().gameType != UserHandler.GameType.NONE_SET*/) { //remove this comment for tournaments.
                int gameType = -1; //-1 = none set yet, 1 = ffa, 2 = left & right teams. if two conflicting values (ignoring -1) found, do not start game.
                foreach(UserHandler.Player pp in UserHandler.getInstance().players) {
                    if(pp.teamType == UserHandler.TeamType.FFA) {
                        if(gameType == -1)
                            gameType = 1;
                        else if(gameType == 2)
                            return;
                    } else {
                        if(gameType == -1)
                            gameType = 2;
                        else if(gameType == 1)
                            return;
                    }
                }
                var moveBoat = GameObject.Find("Main Camera").GetComponent<MoveToBoat>();
                moveBoat.StartGame();
            }
        }

        if((key.Equals("gameMode0") || key.Equals("gameMode1") || key.Equals("gameMode2")) && value.Equals("True")) {
            var moveBoat = GameObject.Find("Main Camera").GetComponent<MoveToBoat>();
            switch(key) {
                case "gameMode0":
                    UserHandler.getInstance().gameType = UserHandler.GameType.ONE_GAME;
                    broadcastMessage("forceEnable", "gameMode0");
                    break;
                case "gameMode1":
                    UserHandler.getInstance().gameType = UserHandler.GameType.THREE_GAME;
                    broadcastMessage("forceEnable", "gameMode1");
                    break;
                case "gameMode2":
                default:
                    UserHandler.getInstance().gameType = UserHandler.GameType.FIVE_GAME;
                    broadcastMessage("forceEnable", "gameMode2");
                    break;
            }
            if(UserHandler.getInstance().allPlayersReady()) {
                moveBoat.StartGame();
            }
        }

        if(key.Equals("gameMoveLeft") || key.Equals("gameMoveRight") || key.Equals("gameShootLeft") || key.Equals("gameShootRight")) {
            UserHandler.Player p = UserHandler.getInstance().getPlayerByID(id);
            if(p == null || p.playerObject == null)
                return;
            var control = p.playerObject.GetComponent<shipController>();
            if(control == null)
                return;
            if(key.Equals("gameMoveLeft")) {
                control.isRotateLeft = value.Equals("True");
                control.isRotateRight = false;
            }
            if(key.Equals("gameMoveRight")) {
                control.isRotateLeft = false;
                control.isRotateRight = value.Equals("True");
            }
            if(key.Equals("gameShootLeft")) {
                control.fireLeft();
            } else if(key.Equals("gameShootRight")) {
                control.fireRight();
            }
        }


        //used for when game is ready
        /*
         
         else {
                moveBoat.StartGame();
            }

        */
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
