using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipController : MonoBehaviour {
	private const float toDegrees = 57.2958f;
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
		y = -y;

		if (x >= 0 && y >= 0) {
			targetAngle = Mathf.Asin (x / (Mathf.Sqrt (x * x + y * y)));
			targetAngle *= toDegrees;
		} else if (x >= 0 && y <= 0) {
			targetAngle = Mathf.Asin (y / (Mathf.Sqrt (x * x + y * y)));
			targetAngle *= toDegrees;
			targetAngle = Mathf.Abs(targetAngle) + 90;
		} else if (x <= 0 && y <= 0) {
			targetAngle = Mathf.Asin (x / (Mathf.Sqrt (x * x + y * y)));
			targetAngle *= toDegrees;
			targetAngle = Mathf.Abs(targetAngle) + 180;
		} else if (x <= 0 && y >= 0) {
			targetAngle = Mathf.Asin (y / (Mathf.Sqrt (x * x + y * y)));
			targetAngle *= toDegrees;
			targetAngle += 270;
		}

		if(x == 0 && y == 0){
			targetAngle = -1;
		}

	}

	void rotateLeft(){
		this.transform.Rotate (new Vector3 (0, -1, 0));
	}

	void rotateRight(){
		this.transform.Rotate (new Vector3 (0, 1, 0));
	}

	public void moveShip(){
		anim.Play ("Boyancy");
		rb.velocity = transform.forward * speed;
	}

	public void fireLeft(){
		//anim.Play("CannonToLeft");
		var cb = Instantiate (CannonBall, LB.transform.position, Quaternion.identity).GetComponent<CannonBall>();
		cb.Direction = Direction.Left;
		cb.location = Location.Back;
		cb.Owner = gameObject;
		var cb2 = Instantiate (CannonBall, LF.transform.position, Quaternion.identity).GetComponent<CannonBall>();
		cb2.Direction = Direction.Left;
		cb2.location = Location.Front;
		cb2.Owner = gameObject;

		//spawn cannon balls on left side of the ship
		//set angle and velocity
		//wont slow down, but slowly drop
	}

	public void fireRight(){
		//anim ["CannonToLeft"].speed = -1;
		//anim.Play ("CannonToLeft");

		var cb = Instantiate (CannonBall, RF.transform.position, Quaternion.identity).GetComponent<CannonBall>();
		cb.Direction = Direction.Right;
		cb.location = Location.Front;
		cb.Owner = gameObject;
		var cb2 = Instantiate (CannonBall, RB.transform.position, Quaternion.identity).GetComponent<CannonBall>();
		cb2.Direction = Direction.Right;
		cb2.location = Location.Back;
		cb2.Owner = gameObject;
		//spawn cannon balls on right side of the ship
		//set angle and velocity
		//wont slow down, but slowly drop
	}

	public void boost(){
		speed *= 2;

		//advanced functionality would be so that it starts increasing slowly 
		//but then speeds up till max X speed
	}

	public void brake(){
		speed *= 0.5f;
		//decrease ship velocity by X amount

		//advanced functionality would be so that it starts slowing down slowly
		//but then faster and faster
	}

	void Update(){
		//find shortest rotation to the angle and keep rotating till user stops touching joystick
		moveShip();
		if (targetAngle != -1) {
			if(this.transform.rotation.eulerAngles.y < targetAngle) {
				if(Mathf.Abs(this.transform.rotation.eulerAngles.y - targetAngle)<180)
					rotateRight();
				else rotateLeft();
			}  else if (this.transform.rotation.eulerAngles.y > targetAngle){
				if (Mathf.Abs (this.transform.rotation.eulerAngles.y - targetAngle) < 180)
					rotateLeft ();
				else rotateRight();
			}
		}
	}

    void OnCollisionEnter( Collision collision ) {
        if (collision.collider.tag == "CannonBall") {
            var cannonBall = collision.gameObject.GetComponent<CannonBall>();
			print ("hit");
			Health--;
			Instantiate (explosion, this.transform.position, Quaternion.identity); 
        }
    }

	void Die() {
		transform.position = transform.position.WithY (-20);
	}
}
