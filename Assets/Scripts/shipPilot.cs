using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipPilot : MonoBehaviour {

	public GameObject ship;
	public shipController shipControl;
	public Animation am;
	// Use this for initialization
	void Start () {
		shipControl = ship.GetComponent<shipController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey("w")){
		//	am ["CannonToLeft"].speed = -1;
			am.Play ("CannonToLeft");
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
