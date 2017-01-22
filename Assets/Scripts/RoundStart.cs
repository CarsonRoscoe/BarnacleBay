using UnityEngine;
using System.Collections;
using NDream.AirConsole;

public class RoundStart : MonoBehaviour {
    public Vector3[] Spawns;
    public Transform Ship;
    public Material[] ShipMaterials;

    [InspectorButton( "Start")]
	public bool start;

	void Start () {
        //AudioManager.instance.playGameMusic( AudioManager.MusicID.GAME );
        GameDataManager.instance.SetGameState (GameState.InGame);
        Spawns.Shuffle();
        GameDataManager.instance.ResetPlayerData();
	    foreach(var id in AirConsole.instance.GetActivePlayerDeviceIds) {
            var playerID = AirConsole.instance.ConvertDeviceIdToPlayerNumber( id );
            var ship = (Transform)Instantiate( Ship, Spawns[playerID], Quaternion.identity );
            if ( ShipMaterials.Length > 0 ) {
                foreach ( var renderer in ship.GetComponentsInChildren<Renderer>() ) {
                    if (renderer.gameObject.name == "DoneBoat")
                        renderer.material = ShipMaterials[playerID];
                }
            }
            ship.GetComponent<shipController>().PlayerID = playerID;
            GameDataManager.instance.SetPlayer( playerID, ship.gameObject );
        }
		GameObject.Find ("Main Camera").GetComponent<cameraController> ().updateValues ();
	}
}
