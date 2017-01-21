using UnityEngine;
using System.Collections;
using NDream.AirConsole;

public class RoundStart : MonoBehaviour {
    public Vector3[] Spawns;
    public Transform Ship;

	[InspectorButton( "Start")]
	public bool start;

	void Start () {
        Spawns.Shuffle();
        GameDataManager.instance.ResetPlayerData();
	    foreach(var id in AirConsole.instance.GetActivePlayerDeviceIds) {
            var playerID = AirConsole.instance.ConvertDeviceIdToPlayerNumber( id );
            var ship = (Transform)Instantiate( Ship, Spawns[playerID], Quaternion.identity );
            GameDataManager.instance.SetPlayer( playerID, ship.gameObject );
        }
	}
}
