using UnityEngine;
using System.Collections;

public class TopDown3DMovement : MonoBehaviour {
    public float Speed = 5f;
    public float DashPower = 200f;
    public float DashDelay = 1.5f;
    public string HorizontalInputName = "Horizontal";
    public string VerticalInputName = "Vertical";
    public string DashInputName = "Dash";
    private Rigidbody m_rigidBody;
    private bool m_canDash;

    void Start() {
        m_canDash = true;
        m_rigidBody = GetComponent<Rigidbody>();
    }

    void Update() {
        //Horizontal/Vertical movement
        var horizontal = Input.GetAxis( HorizontalInputName ) * Speed;
        var vertical = Input.GetAxis( VerticalInputName ) * Speed;
        m_rigidBody.AddForce( Vector3.zero.WithX( horizontal ).WithZ( vertical ) );

        //Dash
        var dash = Input.GetAxis( DashInputName );
        if (m_canDash && dash > 0) {
            Dash();
        }

        //Look where going
        transform.LookAt( transform.position + m_rigidBody.velocity );
    }

    void Dash() {
        m_rigidBody.AddForce( transform.forward * DashPower );
        m_canDash = false;
        StartCoroutine( CanDashTimer( DashDelay ) );
    }

    IEnumerator CanDashTimer(float seconds) {
        yield return new WaitForSeconds( seconds );
        m_canDash = true;
    } 
}
