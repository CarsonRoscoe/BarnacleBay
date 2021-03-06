﻿using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
2 = 0/1 == 0.5
3 = 0/1/2 == 1 
4 = 0/1/2/3 == 1.5
5 = 0/1/2/3/4 == 2
6 = 0/1/2/3/4/5 == 2.5
7 = 0/1/2/3/4/5/6 == 3
8 = 0/1/2/3/4/5/6/7 == 3.5
*/


public class BoatManager : MonoBehaviour {
	public static BoatManager instance;
	public int MaxPlayers = 8;
    public Transform[] Ships;
	public Transform Center;
	public Transform Left;
	public Transform Right;
	public Transform Top;
	public Transform Bottom;
	public bool InTeamSelectMode = false;
	private RectTransform left;
	private RectTransform right;
	private RectTransform top;
	private RectTransform bottom;
	private RectTransform center;
	private RectTransform[] shipRects;

    public enum CanStartResult { NotEnoughPlayers, TooManyPlayers, NoPlayersTeamOne, NoPlayersTeamTwo, FFAAndTeam, CanStart }

	void Awake() {
		if (instance != null) {
			Destroy (instance);
		}
		instance = this;
	}

    public Dictionary<int, TeamSelection> PlayersSelection = new Dictionary<int, TeamSelection>();
    private bool m_updateBoats = true;

	void Start() {
		AudioManager.instance.playGameMusic( AudioManager.MusicID.MENULOOP );
		left = Left.GetComponent<RectTransform> ();
		right = Right.GetComponent<RectTransform> ();
		top = Top.GetComponent<RectTransform> ();
		bottom = Bottom.GetComponent<RectTransform> ();
		center = Center.GetComponent<RectTransform> ();
		shipRects = new RectTransform[Ships.Length];
		for (int i = 0; i < Ships.Length; i++) {
			shipRects [i] = Ships [i].GetComponent<RectTransform> ();
		}
    }

	void Update() {
        if (m_updateBoats)
		    UpdateBoatPositions ();
	}

	public void UpdateBoatPositions() {
		SetActiveBoatSprites (false);
        var playerCount = UserHandler.getInstance().players.Count;
		var height = Mathf.Abs (top.anchoredPosition.y - bottom.anchoredPosition.y);
		var centerHeight = center.transform.position.y;
		int[] positions = new int[playerCount];

        var i = 0;
		foreach (UserHandler.Player player in UserHandler.getInstance().players) {
			var zeroIndex = Mathf.Floor(playerCount / 2f);
			if (playerCount % 2 == 0)
				zeroIndex -= .5f;
			var myRelativeIndex = i - zeroIndex;
			var yPos = myRelativeIndex * height / playerCount;
			var xPos = 0f;
			switch (player.teamType) {
			case UserHandler.TeamType.LEFT:
				xPos = left.transform.position.x;
                SetPlayerTeamSelection(AirConsole.instance.ConvertDeviceIdToPlayerNumber(player.deviceID), TeamSelection.One);
				break;
                case UserHandler.TeamType.FFA:
                xPos = center.transform.position.x;
                SetPlayerTeamSelection(AirConsole.instance.ConvertDeviceIdToPlayerNumber(player.deviceID), TeamSelection.FreeForAll);
				break;
                case UserHandler.TeamType.RIGHT:
                xPos = right.transform.position.x;
                SetPlayerTeamSelection(AirConsole.instance.ConvertDeviceIdToPlayerNumber(player.deviceID), TeamSelection.Two);
				break;
			default:
				break;
			}

			var newPos = new Vector3 (xPos, centerHeight + yPos, -1);
			shipRects[i].gameObject.SetActive (true);
			shipRects[i].position = newPos;
            i++;
		}
	}

    public CanStartResult CanStart() {
        var IsTeam = false;
        var IsSolo = false;
        var teamOneCount = 0;
        var teamTwoCount = 0;
        
        //If less than 2 players or more than 8, don't start
        if (AirConsole.instance.GetActivePlayerDeviceIds.Count < 2) {
            return CanStartResult.NotEnoughPlayers;
        }

        if (AirConsole.instance.GetActivePlayerDeviceIds.Count > 8) {
            return CanStartResult.TooManyPlayers;
        }

        foreach(var teamSelection in PlayersSelection.Values) {
            if (teamSelection == TeamSelection.FreeForAll) {
                IsSolo = true;
            } else {
                if (teamSelection == TeamSelection.One) {
                    teamOneCount++;
                } else {
                    teamTwoCount++;
                }
            }
        }
        IsTeam = (teamOneCount + teamTwoCount) > 0;

        //If we have either a team or a solo game
        if (!(IsTeam && IsSolo)) {
            GameDataManager.instance.GameType = IsTeam ? GameTeamType.Team : GameTeamType.FFA;
        } else {
            return CanStartResult.FFAAndTeam;
        }

        if (IsTeam) {
            if (teamOneCount == 0) {
                return CanStartResult.NoPlayersTeamOne;
            } else if (teamTwoCount == 0) {
                return CanStartResult.NoPlayersTeamTwo;
            }
        }


        return CanStartResult.CanStart;
    }

	public void SetPlayerTeamSelection(int deviceID, int teamSelectionDirection) {
		SetPlayerTeamSelection (deviceID, (TeamSelection)teamSelectionDirection);
	}

    public void SetPlayerTeamSelection(int deviceID, TeamSelection teamSelection) {
        if (!PlayersSelection.ContainsKey(deviceID)) {
            PlayersSelection.Add( deviceID, teamSelection );
        } else {
            PlayersSelection[deviceID] = teamSelection;
        }
    }

    public void RemovePlayerFromTeamSelection(int playerID) {
        if (PlayersSelection.ContainsKey(playerID)) {
            PlayersSelection.Remove(playerID);
        }
    }

    public void StartGame() {
        GameDataManager.instance.ReadyPlayers(PlayersSelection);
        SceneManager.LoadScene( "Enviroment_Final" );
    }

	public void SetActiveBoatSprites(bool isActive) {
		foreach (var ship in Ships) {
			ship.gameObject.SetActive (isActive);
		}
	}

    public bool IsActiveUpdatingBoats() {
        return m_updateBoats;
    }

    public void SetActiveUpdatingBoats(bool keepUpdatingBoats) {
        m_updateBoats = keepUpdatingBoats;
    }

}
