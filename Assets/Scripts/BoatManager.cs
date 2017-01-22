using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoatManager : MonoBehaviour {
    public Transform Ship;
    Dictionary<int, BoatAnimation> PlayersBoats = new Dictionary<int, BoatAnimation>();
    Dictionary<int, TeamSelection> PlayersSelection = new Dictionary<int, TeamSelection>();
    public Button StartButton;

    void Start() {
        AirConsole.instance.SetActivePlayers();
        var startPos = new Vector3( 1.5f, 0, 25 );
        foreach(var deviceID in AirConsole.instance.GetActivePlayerDeviceIds) {
            var playerID = AirConsole.instance.ConvertDeviceIdToPlayerNumber( deviceID );
            var ship = Instantiate( Ship );
            ship.position = startPos;
            PlayersBoats.Add( playerID, ship.GetComponent<BoatAnimation>() );
            startPos = startPos.WithZ( startPos.z + 9f );
            SetPlayerTeamSelection( playerID, TeamSelection.FreeForAll );
        }
    }

    void Update() {
        StartButton.interactable = CanStart();
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

    void SetPlayer(int playerID, BoatAnimation boatAnimation) {
        if (!PlayersBoats.ContainsKey(playerID)) {
            PlayersBoats.Add( playerID, boatAnimation );
        }
    }

    public void PlayerSwitchedSelection(int playerID, int direction) {
        switch ( direction ) {
            case -1:
                switch ( PlayersSelection[playerID] ) {
                    case TeamSelection.One:
                        break;
                    case TeamSelection.FreeForAll:
                        SetPlayerTeamSelection( playerID, TeamSelection.One );
                        break;
                    case TeamSelection.Two:
                        SetPlayerTeamSelection( playerID, TeamSelection.FreeForAll );
                        break;
                    default:
                        break;
                }
                break;
            case 1:
                switch ( PlayersSelection[playerID] ) {
                    case TeamSelection.One:
                        SetPlayerTeamSelection( playerID, TeamSelection.FreeForAll );
                        break;
                    case TeamSelection.FreeForAll:
                        SetPlayerTeamSelection( playerID, TeamSelection.Two );
                        break;
                    case TeamSelection.Two:
                        break;
                    default:
                        break;
                }
                break;
        }
    }

    void SetPlayerTeamSelection(int playerID, TeamSelection teamSelection) {
        if (!PlayersSelection.ContainsKey(playerID)) {
            PlayersSelection.Add( playerID, teamSelection );
        } else {
            PlayersSelection[playerID] = teamSelection;
        }
        PlayersBoats[playerID].SetSelectedTeam( teamSelection );
    }

    public void StartGame() {
        GameDataManager.instance.ReadyPlayers(PlayersSelection);
        SceneManager.LoadScene( "TestScene" );
    }
}
