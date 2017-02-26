using UnityEngine;
using System.Collections;
using NDream.AirConsole;

public class RoundStart : MonoBehaviour {
    public Vector3[] Spawns;
    public GameObject Ship;
    public Material[] ShipMaterials;

    [InspectorButton( "Start")]
	public bool start;

	void Start () {
        AirConsoleManager.instance.setController(-1, true);
        AudioManager.instance.playGameMusic( AudioManager.MusicID.GAME );
        GameDataManager.instance.SetGameState (GameState.InGame);
        Spawns.Shuffle();
        UserHandler.getInstance().resetPlayers();
        var i = 0;
	    foreach(UserHandler.Player p in UserHandler.getInstance().players) {
            GameObject ship = Instantiate<GameObject>( Ship, Spawns[i], Quaternion.identity );
            if ( ShipMaterials.Length > 0 ) {
                foreach ( var renderer in ship.GetComponentsInChildren<Renderer>() ) {
                    //Name of piece with renderer. Change after gamejam cause hack
                    if ( renderer.gameObject.name == "polySurface1" ) {
                        var materials = renderer.materials;
                        renderer.materials = new Material[] { ShipMaterials[i], ShipMaterials[i], ShipMaterials[i], ShipMaterials[i] };
                        print( "Material change" );
                    }
                }
            }
            ship.GetComponent<shipController>().PlayerID = p.deviceID;
            p.playerObject = ship;
            GameDataManager.instance.SetPlayer( p.deviceID, ship.gameObject );
            i++;
        }
		GameObject.Find("Main Camera").GetComponent<cameraController> ().updateValues ();
	}
}
