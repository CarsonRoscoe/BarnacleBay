using UnityEngine;
using System.Collections;
using System;

public class OnCollisionSwap : MonoBehaviour {
    public Transform Dead;
    private Rigidbody m_rigidBody;

    void Start() {
        m_rigidBody = GetComponent<Rigidbody>();
    }

    /*
        OnCollisionEnter - The most beautiful method I've ever written.
    */
    void OnCollisionEnter( Collision collision ) {
        if ( collision.gameObject.HasComponent<OnCollisionSwap>() && gameObject.GetInstanceID() > collision.gameObject.GetInstanceID() ) {
            //print( m_rigidBody.velocity.magnitude );

            //Now this is how you formula. Make extension method transform.AmIMoreFacingYou(collision.transform)
            var playerP1 = transform.position;
            var playerP2 = transform.position + transform.forward;
            var enemyP1 = collision.gameObject.transform.position;
            var enemyP2 = collision.gameObject.transform.position + collision.gameObject.transform.position.normalized;
            var baseDistance = playerP1.DistanceToNoY( enemyP1 );
            var pP2toeP1 = playerP2.DistanceToNoY( enemyP1 );
            var eP2topP1 = enemyP2.DistanceToNoY( playerP1 );

            //Determine variables
            var amFacingAway = pP2toeP1 >= eP2topP1;
            var similar = pP2toeP1 < baseDistance && eP2topP1 < baseDistance;
            var amSlower = m_rigidBody.velocity.magnitude < collision.rigidbody.velocity.magnitude;
            var bothSlow = m_rigidBody.velocity.magnitude < 10 && collision.rigidbody.velocity.magnitude < 10;
            var otherIsFast = collision.rigidbody.velocity.magnitude >= 10;

            Action<GameObject, GameObject> KillAndLive = ( oneWhoDies, oneWhoLives ) =>
            {
                var death = Instantiate( Dead );
                death.position = oneWhoDies.transform.position;
                var color = oneWhoDies.GetComponent<Renderer>().material.color;
                foreach ( var renderer in death.GetComponentsInChildren<Renderer>() ) {
                    renderer.material.color = color;
                }
                foreach ( var rigidBody in death.GetComponentsInChildren<Rigidbody>() ) {
                    rigidBody.AddExplosionForce( 100, oneWhoDies.transform.position, 3 );
                }
                Destroy( oneWhoDies );
                oneWhoLives.GetComponent<Rigidbody>().AddForce( Vector3.forward * 10000 );
            };

            Action IDie = () =>
            {
                KillAndLive( gameObject, collision.gameObject );
            };

            Action ILive = () =>
            {
                KillAndLive( collision.gameObject, gameObject );
            };
            if (!bothSlow ) {
                if (similar) {
                    if (amSlower) {
                        IDie();
                    }
                    else {
                        ILive();
                    }
                }  else {
                    if (amSlower) {
                        if (otherIsFast) {
                            IDie();
                        } else {
                            ILive();
                        }
                    } else {
                        ILive();
                    }
                }
            }
        }
    }
}
