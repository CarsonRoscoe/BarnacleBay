using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Left, Right }

public class CannonBall : MonoBehaviour {
    public Transform CannonBallFiredSmoke;
    public GameObject Owner;
    public Direction Direction = Direction.Left;
    public int Power = 10;

	// Use this for initialization
	void Start () {
        var smoke = Instantiate( CannonBallFiredSmoke, Owner.transform );
        smoke.transform.position = transform.position;
        GetComponent<Rigidbody>().AddForce( ((Direction == Direction.Left ? -1 : 1) * Owner.transform.right + Vector3.up / 10) * Power );
    }
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < -20) {
            Destroy( gameObject );
        }
	}
}
