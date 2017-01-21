using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour {

	private float offsetX;
	private float offsetZ;


	// Use this for initialization
	void Start () {
		offsetX = 15 - this.transform.position.x;
		offsetZ = 19 - this.transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
