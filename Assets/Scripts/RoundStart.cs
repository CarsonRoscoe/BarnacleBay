using UnityEngine;
using System.Collections.Generic;
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
        var tiedPlayerIDs = GameDataManager.instance.GameMode == GameMode.SuddenDeath ? GameDataManager.instance.TiedWinners : new List<UserHandler.Player>();
        
	    foreach(UserHandler.Player p in UserHandler.getInstance().players) {
            var notATieSoEveryonePlays = tiedPlayerIDs.Count == 0;
            var pIsATiedPlayerToPlay = tiedPlayerIDs.Contains(p);
    
            if (notATieSoEveryonePlays || pIsATiedPlayerToPlay) {
                GameObject ship = Instantiate<GameObject>( Ship, Spawns[i], Quaternion.identity );
                ship.transform.position = new Vector3(ship.transform.position.x, 1.1f, ship.transform.position.z);
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
            }
            i++;
        }

		GameObject.Find("Main Camera").GetComponent<cameraController> ().updateValues ();
	}
}
