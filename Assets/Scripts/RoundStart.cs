using UnityEngine;
using System.Collections;
using NDream.AirConsole;

public class RoundStart : MonoBehaviour {
    public Vector3[] Spawns;
    public Transform[] Ship;

	[InspectorButton( "Start")]
	public bool start;

	void Start () {
		GameDataManager.instance.SetGameState (GameState.InGame);
        Spawns.Shuffle();
        GameDataManager.instance.ResetPlayerData();
	    foreach(var id in AirConsole.instance.GetActivePlayerDeviceIds) {
            var playerID = AirConsole.instance.ConvertDeviceIdToPlayerNumber( id );
            var ship = (Transform)Instantiate( Ship[playerID], Spawns[playerID], Quaternion.identity );
            ship.GetComponent<shipController>().PlayerID = playerID;
            GameDataManager.instance.SetPlayer( playerID, ship.gameObject );
        }
		GameObject.Find ("Main Camera").GetComponent<cameraController> ().updateValues ();
	}
}
