using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Director;

public class shipController : MonoBehaviour {
	private const float toDegrees = 57.2958f;
    public int PlayerID;
	public float targetAngle = -1; 
	public Rigidbody rb;
	public float speed = 10;
	public Transform CannonBall;
	public Transform explosion;
	public Animation anim;
	public GameObject LB;
	public GameObject LF;
	public GameObject RB;
	public GameObject RF;
	public GameObject LBsmoke;
	public GameObject LFsmoke;
	public GameObject RBsmoke;
	public GameObject RFsmoke;
	private int _health;
	public CannonBall cb;
	public CannonBall cb2;
	public Animator testing;
	public int Health { 
		get { 
			return _health; 
		}
		private set {
			_health = value;
			if (_health == 0) {
				Die ();
			}
		}
	}

	void Start(){
		rb = GetComponent<Rigidbody> ();
		anim = this.GetComponent<Animation> ();
		Health = 5;
	}

	public void rotateTowards(float x, float y){
		//find angle from air console return point

		//airconsole is stupid
		x = -x;
		if (!AirConsoleManager.instance.newControls) {
			if (x >= 0 && y >= 0) {
				targetAngle = Mathf.Asin (x / (Mathf.Sqrt (x * x + y * y)));
				targetAngle *= toDegrees;
			} else if (x >= 0 && y <= 0) {
				targetAngle = Mathf.Asin (y / (Mathf.Sqrt (x * x + y * y)));
				targetAngle *= toDegrees;
				targetAngle = Mathf.Abs (targetAngle) + 90;
			} else if (x <= 0 && y <= 0) {
				targetAngle = Mathf.Asin (x / (Mathf.Sqrt (x * x + y * y)));
				targetAngle *= toDegrees;
				targetAngle = Mathf.Abs (targetAngle) + 180;
			} else if (x <= 0 && y >= 0) {
				targetAngle = Mathf.Asin (y / (Mathf.Sqrt (x * x + y * y)));
				targetAngle *= toDegrees;
				targetAngle += 270;
			}
		} else {
			if (x >= 0 && y >= 0) {
				targetAngle = 1;
			} else if (x >= 0 && y <= 0) {
				targetAngle = 1;
			} else if (x <= 0 && y <= 0) {
				targetAngle = 0;
			} else if (x <= 0 && y >= 0) {
				targetAngle = 0;
			}
		}
		if(x == 0 && y == 0){
			targetAngle = -1;
		}

	}

	void rotateLeft(){
		this.transform.Rotate (new Vector3 (0, -1.5f, 0));
	}

	void rotateRight(){
		this.transform.Rotate (new Vector3 (0, 1.5f, 0));
	}

	public void moveShip(){
		rb.velocity = -transform.forward * speed;
	}

	public void fireLeft(){
		
	//	testing.Play ("FireOne");
		if (cb == null) {
			cb = Instantiate (CannonBall, LB.transform.position, Quaternion.identity).GetComponent<CannonBall> ();
			cb.Direction = Direction.Left;
			cb.location = Location.Back;
			cb.Owner = gameObject;
		}
		if (cb2 == null) {
			cb2 = Instantiate (CannonBall, LF.transform.position, Quaternion.identity).GetComponent<CannonBall> ();
			cb2.Direction = Direction.Left;
			cb2.location = Location.Front;
			cb2.Owner = gameObject;
		}
		//spawn cannon balls on left side of the ship
		//set angle and velocity
		//wont slow down, but slowly drop
	}

	public void fireRight(){
	//	testing.Play ("FireTwo");

		if (cb == null) {
			cb = Instantiate (CannonBall, RF.transform.position, Quaternion.identity).GetComponent<CannonBall> ();
			cb.Direction = Direction.Right;
			cb.location = Location.Front;
			cb.Owner = gameObject;
		}
		if (cb2 == null) {
			cb2 = Instantiate (CannonBall, RB.transform.position, Quaternion.identity).GetComponent<CannonBall> ();
			cb2.Direction = Direction.Right;
			cb2.location = Location.Back;
			cb2.Owner = gameObject;
		}
		//spawn cannon balls on right side of the ship
		//set angle and velocity
		//wont slow down, but slowly drop
	}

	public void boost(){
		if (speed < 28)
			speed *= 1.2f;
	}

	public void brake(){
		if (speed > 14)
		speed *= 0.83f;
	}

	void Update(){
		//find shortest rotation to the angle and keep rotating till user stops touching joystick
		moveShip();
		if (!AirConsoleManager.instance.newControls) {
			if (targetAngle != -1) {
				if (this.transform.rotation.eulerAngles.y < targetAngle) {
					if (Mathf.Abs (this.transform.rotation.eulerAngles.y - targetAngle) < 180)
						rotateRight ();
					else
						rotateLeft ();
				} else if (this.transform.rotation.eulerAngles.y > targetAngle) {
					if (Mathf.Abs (this.transform.rotation.eulerAngles.y - targetAngle) < 180)
						rotateLeft ();
					else
						rotateRight ();
				}
			}
		} else {
			if (targetAngle != -1) {
				if (targetAngle == 1) {
					rotateLeft ();
				} else if (targetAngle == 0) {
					rotateRight ();
				}
			}
		}
	}

    void OnCollisionEnter( Collision collision ) {
        if (collision.collider.tag == "CannonBall") {
          //  AudioManager.instance.playSound( AudioManager.SFXID.CANNONIMPACT );
            var cannonBall = collision.gameObject.GetComponent<CannonBall>();
			Health--;
			Instantiate (explosion, collision.contacts.First().point, Quaternion.identity);
            Destroy( collision.gameObject );
        }
    }

	void Die() {
        GameDataManager.instance.RemovePlayer( PlayerID );
	testing.SetBool ("isDead", true);
	speed = 0;
        Instantiate( explosion, this.transform.position, Quaternion.identity );
        Camera.main.GetComponent<cameraController>().endGame();
        Destroy(gameObject, 6f);
        Camera.main.GetComponent<cameraController>().updateValues();
    }
}
