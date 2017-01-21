using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipPilot : MonoBehaviour {

	public GameObject ship;
	public shipController shipControl;
	// Use this for initialization
	void Start () {
		shipControl = ship.GetComponent<shipController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey("w")){
			shipControl.rotateTowards(0.6f,0.8f);
		}
		if (Input.GetKey("a")){
			shipControl.rotateTowards(-1,0);
		}
		if (Input.GetKey("s")){
			shipControl.rotateTowards(0,-1);
		}
		if (Input.GetKey("d")){
			shipControl.rotateTowards(1,0);
		}

	}
}
