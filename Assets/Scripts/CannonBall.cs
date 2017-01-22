using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Left, Right }
public enum Location { Front, Back }

public class CannonBall : MonoBehaviour {
    public Transform CannonBallFiredSmoke;
    public GameObject Owner;
    public Direction Direction = Direction.Left;
	public Location location = Location.Front;
    public int Power = 10;

	// Use this for initialization
	void Start () {
        var smoke = Instantiate( CannonBallFiredSmoke, Owner.transform );
		if (Direction == Direction.Left && location == Location.Front) {
			smoke.transform.position = Owner.GetComponent<shipController> ().LFsmoke.transform.position;
		}
		if (Direction == Direction.Right && location == Location.Front) {
			smoke.transform.position = Owner.GetComponent<shipController> ().RFsmoke.transform.position;
		}
		if (Direction == Direction.Left && location == Location.Back) {
			smoke.transform.position = Owner.GetComponent<shipController> ().LBsmoke.transform.position;
		}
		if (Direction == Direction.Right && location == Location.Back) {
			smoke.transform.position = Owner.GetComponent<shipController> ().RBsmoke.transform.position;
		}

        GetComponent<Rigidbody>().AddForce( ((Direction == Direction.Right ? -1 : 1) * Owner.transform.right + Vector3.up / 10) * Power );
    }
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < -20) {
            Destroy( gameObject );
        }
	}
}
