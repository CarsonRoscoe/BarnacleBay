using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NDream.AirConsole;
using UnityEngine;
using UnityEngine.Experimental.Director;

public class shipController : MonoBehaviour {
    public bool isRotateLeft = false;
    public bool isRotateRight = false;

    private const float toDegrees = 57.2958f;
    public int PlayerID;
    public float targetAngle = -1;
    public Rigidbody rb;
    public float speed = 30;
    public float maxspeed = 40;
    public float minspeed = 20;
    public float turnRate = 3f;
    private int amountLeaned = 0;
    private int maxLeaned = 30;
    private float timeTilNextShot = 2.0f;
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
    public bool canShoot;
    private Dictionary<int, int> damageByPlayer;
    private int lastHitByPlayer;
    public int Health {
        get {
            return _health;
        }
        private set {
            _health = value;
            AirConsoleManager.instance.updateHealth( PlayerID, value );
            if ( _health == 0 ) {
                Die();
            }
        }
    }

    void Start() {
        canShoot = true;
        rb = GetComponent<Rigidbody>();
        anim = this.GetComponent<Animation>();
        _health = 5;
        damageByPlayer = new Dictionary<int, int>();

        if (GameDataManager.instance.GameMode == GameMode.SuddenDeath) {
            speed *= 1.5f;
            timeTilNextShot *= 0.75f;
            Health = 1;
        }
    }

    public void rotateTowards( float x, float y ) {
        //find angle from air console return point

        //airconsole is stupid
        x = -x;
        if ( x >= 0 && y >= 0 ) {
            targetAngle = 1;
        } else if ( x >= 0 && y <= 0 ) {
            targetAngle = 1;
        } else if ( x <= 0 && y <= 0 ) {
            targetAngle = 0;
        } else if ( x <= 0 && y >= 0 ) {
            targetAngle = 0;
        }
        if ( x == 0 && y == 0 ) {
            targetAngle = -1;
        }

    }

    void rotateLeft() {
        this.transform.Rotate( new Vector3( 0, -turnRate, 0 ) );
    }

    void rotateRight() {
        this.transform.Rotate( new Vector3( 0, turnRate, 0 ) );
    }

    public void moveShip() {
        rb.velocity = -transform.forward * speed;
    }

    public void fireLeft() {
        if ( canShoot ) {
            //shoot 2 bullets
            //if ( cb == null ) {
            cb = Instantiate( CannonBall, LB.transform.position, Quaternion.identity ).GetComponent<CannonBall>();
            cb.Direction = Direction.Left;
            cb.location = Location.Back;
            //cb.SetColor( PlayerData.getColorData( AirConsoleManager.instance.players.First( x => x.playerID == PlayerID ).color ) );
            cb.Owner = gameObject;
            //}
            //if ( cb2 == null ) {
            cb2 = Instantiate( CannonBall, LF.transform.position, Quaternion.identity ).GetComponent<CannonBall>();
            cb2.Direction = Direction.Left;
            cb2.location = Location.Front;
            //cb.SetColor( PlayerData.getColorData( AirConsoleManager.instance.players.First( x => x.playerID == PlayerID ).color ) );
            cb2.Owner = gameObject;
            //}

            canShoot = false;
            StartCoroutine( DelayShooting() );
        }
    }

    public void fireRight() {
        if ( canShoot ) {
            //shoot 2 bullets
            //if ( cb == null ) {
            cb = Instantiate( CannonBall, RF.transform.position, Quaternion.identity ).GetComponent<CannonBall>();
            cb.Direction = Direction.Right;
            cb.location = Location.Front;
            cb.Owner = gameObject;
            //}
            //if ( cb2 == null ) {
            cb2 = Instantiate( CannonBall, RB.transform.position, Quaternion.identity ).GetComponent<CannonBall>();
            cb2.Direction = Direction.Right;
            cb2.location = Location.Back;
            cb2.Owner = gameObject;
            //}

            canShoot = false;
            StartCoroutine( DelayShooting() );
        }
    }

    IEnumerator DelayShooting() {
        yield return new WaitForSeconds( timeTilNextShot );
        canShoot = true;
    }


    public void boost() {
        speed += 5;
        if ( speed > maxspeed )
            speed = maxspeed;
    }

    public void brake() {
        speed -= 5;
        if ( speed < minspeed )
            speed = minspeed;
    }

    void Update() {
        //find shortest rotation to the angle and keep rotating till user stops touching joystick
        moveShip();
        if ( isRotateLeft )
            rotateLeft();
        if ( isRotateRight )
            rotateRight();
        /*
		if ( targetAngle != -1 ) {
			if ( targetAngle == 1 ) {
				rotateLeft();
			}
			else if ( targetAngle == 0 ) {
				rotateRight();
			}
		}*/
    }

    void OnCollisionEnter( Collision collision ) {
        if ( collision.collider.tag == "CannonBall" ) {
            //  AudioManager.instance.playSound( AudioManager.SFXID.CANNONIMPACT );
            var cannonBall = collision.gameObject.GetComponent<CannonBall>();
            var attackingPlayer = cannonBall.Owner.GetComponent<shipController>().PlayerID;
            lastHitByPlayer = attackingPlayer;
            if (!damageByPlayer.ContainsKey(lastHitByPlayer)) {
                damageByPlayer.Add(lastHitByPlayer, 0);
            }
            damageByPlayer[lastHitByPlayer]++;
            Health--;
            Instantiate( explosion, collision.contacts.First().point, Quaternion.identity );
            Destroy( collision.gameObject );

        } else if ( collision.collider.tag == "Wall" ) {
            var collidedPoint = collision.contacts.First().point;
            Vector3 heading = collidedPoint - transform.position;
            var dirNum = AngleDir( transform.forward, heading, transform.up );
            //print( dirNum );
            //if (dirNum > 0) {
            //    rotateLeft();
            //    rotateLeft();
            //} else {
            //    rotateRight();
            //    rotateRight();
            //}
        }
    }

    float AngleDir( Vector3 fwd, Vector3 targetDir, Vector3 up ) {
        Vector3 perp = Vector3.Cross( fwd, targetDir );
        float dir = Vector3.Dot( perp, up );
        return dir;
    }

    void Die() {
        //Update scores
        var lastHitter = UserHandler.getInstance().getPlayerByID(lastHitByPlayer);
        var mostDamager = UserHandler.getInstance().getPlayerByID(damageByPlayer.Aggregate((l, r) => l.Value > r.Value ? l : r).Key);
        
        if (lastHitter == mostDamager) {
            lastHitter.addToScore(3);
            PlayerHUDHandler.instance.CreateScore(3, lastHitter.playerObject);
            //Visually make +3 appear
        } else {
            lastHitter.addToScore(1);
            PlayerHUDHandler.instance.CreateScore(1, lastHitter.playerObject);
            mostDamager.addToScore(1);
            PlayerHUDHandler.instance.CreateScore(1, mostDamager.playerObject);
        }

        //Kill self
        GameDataManager.instance.RemovePlayer( PlayerID );
        testing.SetBool( "isDead", true );
        speed = 0;
        Instantiate( explosion, this.transform.position, Quaternion.identity );
        GetComponent<CapsuleCollider>().height = 0f;
        var boxCollider = GetComponent<BoxCollider>();
        boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y, boxCollider.size.z / 4);
        Camera.main.GetComponent<cameraController>().endGame();
        Destroy( gameObject, 6f );
        Camera.main.GetComponent<cameraController>().updateValues();
    }

}
