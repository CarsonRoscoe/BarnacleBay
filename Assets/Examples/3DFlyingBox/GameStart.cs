using UnityEngine;
using System.Collections;

public class GameStart : MonoBehaviour {
    public Camera Camera;
    public Transform PlayerObjects;

	// Use this for initialization
	void Start () {
        CreatePlayer( new Vector3( 0, -1, 0 ), Color.red, true );
        CreatePlayer( new Vector3( 2, -1, 2 ), Color.blue, false );
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    Transform CreatePlayer(Vector3 position, Color color, bool isPlayer) {
        var player = Instantiate( PlayerObjects );
        player.transform.position = position;
        player.GetComponent<Renderer>().material.color = color;
        var tail = player.GetComponent<TrailRenderer>();
        tail.material.SetColor( "_TintColor", color );
        if (isPlayer) {
            var movement = player.gameObject.AddComponent<TopDown3DMovement>();
            movement.DashPower = 10000;
            movement.Speed = 125;
            //movement.
            //Camera.GetComponent<TopDown3DCamera>().SetFollowing(player);
        }
        return player;
    }

}
