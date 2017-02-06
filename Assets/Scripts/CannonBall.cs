using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Direction { Left, Right }
public enum Location { Front, Back }

public class CannonBall : MonoBehaviour {
    public Transform CannonBallFiredSmoke;
    public Transform Explosion;
    public GameObject Owner;
    public Direction Direction = Direction.Left;
	public Location location = Location.Front;
    public int Power = 5;
    private float timePassed = 0f;
    public const float deathTime = 8f; //destroy after 5 seconds.


	// Use this for initialization
	void Start () {
        AudioManager.instance.playSound( AudioManager.SFXID.CANNONFIRE );
        var smoke = Instantiate( CannonBallFiredSmoke, Owner.transform );
		if (Direction == Direction.Left && location == Location.Front) {
			smoke.transform.position = Owner.GetComponent<shipController> ().LFsmoke.transform.position;
		}
		else if (Direction == Direction.Right && location == Location.Front) {
			smoke.transform.position = Owner.GetComponent<shipController> ().RFsmoke.transform.position;
		}
		else if (Direction == Direction.Left && location == Location.Back) {
			smoke.transform.position = Owner.GetComponent<shipController> ().LBsmoke.transform.position;
		}
		else if (Direction == Direction.Right && location == Location.Back) {
			smoke.transform.position = Owner.GetComponent<shipController> ().RBsmoke.transform.position;
		}

        GetComponent<Rigidbody>().AddForce( ((Direction == Direction.Right ? -1 : 1) * Owner.transform.right + Vector3.up / 10) * Power );
    }
	
	// Update is called once per frame
	void Update () {
        timePassed += Time.deltaTime;
		if (transform.position.y < -20 || timePassed >= deathTime) {
            Destroy( gameObject );
        }
	}

    void OnCollisionEnter( Collision collision ) {
        if ( collision.collider.tag == "Wall" ) {
            var collidedPoint = collision.contacts.First().point;
            Instantiate( Explosion, collidedPoint, Quaternion.identity );
			Destroy( gameObject );
        }
    }

    public void SetColor( Color color ) {
        GetComponent<Renderer>().material.color = color;
        GetComponent<Renderer>().material.SetColor("_Color", color);
        GetComponent<TrailRenderer>().material.SetColor( "_TintColor", color );
    }
}
