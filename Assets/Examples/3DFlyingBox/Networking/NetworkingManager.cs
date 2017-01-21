using UnityEngine;
using System.Collections;

public class NetworkingManager : MonoBehaviour {
    private const string VERSION = "v0.0.1";
    public string RoomName = "VVR";
    public string PlayerPrefabName = "Player";

	void Start () {
        //PhotonNetwork.ConnectUsingSettings(VERSION);
	}

    void OnJoinedLobby() {
        //RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 4 };
        //PhotonNetwork.JoinOrCreateRoom( RoomName, roomOptions, TypedLobby.Default );
    }

    void OnJoinedRoom() {
    }
}
