using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

	void Awake() {
		if (instance != null) {
			Destroy (instance);
		}
		instance = this;
	}

    public Dictionary<int, TeamSelection> PlayersSelection = new Dictionary<int, TeamSelection>();

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
		UpdateBoatPositions ();
	}

	public void UpdateBoatPositions() {
		var slot = 0;
		var deviceIDs = AirConsole.instance.GetActivePlayerDeviceIds;
		SetActiveBoatSprites (false);
        var playerCount = UserHandler.getInstance().players.Count;
		var topHeight = top.anchoredPosition.y;
		var bottomHeight = bottom.anchoredPosition.y;
		var height = Mathf.Abs (topHeight - bottomHeight);
		var centerHeight = center.transform.position.y;
		var increase = height / (MaxPlayers - 1);
		int[] positions = new int[playerCount];
		var actualHeight = increase * playerCount;

        var i = 0;
		foreach (UserHandler.Player p in UserHandler.getInstance().players) {
			var zeroIndex = (float)playerCount / 2f;
			if (zeroIndex % 2 == 0)
				zeroIndex -= .5f;
			var myRelativeIndex = i - zeroIndex;
			var yPos = myRelativeIndex * increase;
			var xPos = 0f;
			switch (p.teamType) {
			case UserHandler.TeamType.LEFT:
				xPos = left.transform.position.x;
                SetPlayerTeamSelection(AirConsole.instance.ConvertDeviceIdToPlayerNumber(p.deviceID), TeamSelection.One);
				break;
                case UserHandler.TeamType.FFA:
                    xPos = center.transform.position.x;
                SetPlayerTeamSelection(AirConsole.instance.ConvertDeviceIdToPlayerNumber(p.deviceID), TeamSelection.FreeForAll);
				break;
                case UserHandler.TeamType.RIGHT:
                    xPos = right.transform.position.x;
                SetPlayerTeamSelection(AirConsole.instance.ConvertDeviceIdToPlayerNumber(p.deviceID), TeamSelection.Two);
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

    public bool CanStart() {
        var IsTeam = false;
        var IsSolo = false;
        foreach(var teamSelection in PlayersSelection.Values) {
            if (teamSelection == TeamSelection.FreeForAll) {
                IsSolo = true;
            } else {
                IsTeam = true;
            }
        }
        return !(IsTeam && IsSolo);
    }

	public void SetPlayerTeamSelection(int playerID, int teamSelectionDirection) {
		SetPlayerTeamSelection (playerID, (TeamSelection)teamSelectionDirection);
	}

    public void SetPlayerTeamSelection(int playerID, TeamSelection teamSelection) {
        if (!PlayersSelection.ContainsKey(playerID)) {
            PlayersSelection.Add( playerID, teamSelection );
        } else {
            PlayersSelection[playerID] = teamSelection;
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

}
