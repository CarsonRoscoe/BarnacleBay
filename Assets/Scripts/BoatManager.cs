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

    Dictionary<int, TeamSelection> PlayersSelection = new Dictionary<int, TeamSelection>();

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
			SetPlayerTeamSelection (i, TeamSelection.FreeForAll);
		}
    }

	void Update() {
		UpdateBoatPositions ();
	}

	public void UpdateBoatPositions() {
		var slot = 0;
		var deviceIDs = AirConsole.instance.GetActivePlayerDeviceIds;
		foreach (var boat in Ships) {
			boat.gameObject.SetActive (false);
		}
		var playerCount = AirConsole.instance.GetActivePlayerDeviceIds.Count;
		var topHeight = top.anchoredPosition.y;
		var bottomHeight = bottom.anchoredPosition.y;
		var height = Mathf.Abs (topHeight - bottomHeight);
		var centerHeight = center.transform.position.y;
		var increase = height / (MaxPlayers - 1);
		int[] positions = new int[playerCount];
		var actualHeight = increase * playerCount;

		foreach (var controllerID in AirConsole.instance.GetActivePlayerDeviceIds) {
			var playerID = AirConsole.instance.ConvertDeviceIdToPlayerNumber (controllerID);
			var oldPos = Ships [playerID].transform.position;
			var zeroIndex = (float)playerCount / 2f;
			if (zeroIndex % 2 == 0)
				zeroIndex -= .5f;
			var myRelativeIndex = playerID - zeroIndex;
			var yPos = myRelativeIndex * increase;
			var xPos = 0f;
			var ship = Ships [playerID];
			if (!PlayersSelection.ContainsKey (playerID)) {
				PlayersSelection.Add(playerID, TeamSelection.FreeForAll);
			}
			switch (PlayersSelection[playerID]) {
			case TeamSelection.One:
				xPos = left.transform.position.x;
				break;
			case TeamSelection.FreeForAll:
				xPos = center.transform.position.x;
				break;
			case TeamSelection.Two:
				xPos = right.transform.position.x;
				break;
			default:
				break;
			}

			var newPos = new Vector3 (xPos, centerHeight + yPos, oldPos.z);
			ship.gameObject.SetActive (true);
			shipRects[playerID].position = newPos;
		}
	}

    bool CanStart() {
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

    public void StartGame() {
        GameDataManager.instance.ReadyPlayers(PlayersSelection);
        SceneManager.LoadScene( "Enviroment_Final" );
    }
}
