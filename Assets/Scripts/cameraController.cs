using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;

public class cameraController : MonoBehaviour {

	private float offsetX;
	private float offsetZ;
	private float fieldOfView = 14f;
	private List<Transform> valuesTransform = new List<Transform> ();
	private bool endG;
	private float largeX = -10000000;
	private float largeZ = -10000000;
	private float smallX = 10000000;
	private float smallZ = 10000000;
	private float distZ;
	private float distX;
	private float startTime;

	private Transform startPos;
	public float speed = 0.5f;
	private float journeyLength;

	private float startFOV;
	private float targetFOV;
	public float speedFOV = 2f;
	private float journeyFOV;

	// Use this for initialization
	void Start () {
		offsetX = this.transform.position.x - 15;
		offsetZ = this.transform.position.z + 130f;
		updateValues ();
	}
	
	// Update is called once per frame
	void Update () {
		largeX = -10000000;
		largeZ = -10000000;
		smallX = 10000000;
		smallZ = 10000000;

		for (int i = 0; i < valuesTransform.Count; i++) {
			if (valuesTransform [i].position.x > largeX)
				largeX = valuesTransform [i].position.x;
			if (valuesTransform [i].position.x < smallX)
				smallX = valuesTransform [i].position.x;
			if (valuesTransform [i].position.z > largeZ)
				largeZ = valuesTransform [i].position.z;
			if (valuesTransform [i].position.z < smallZ)
				smallZ = valuesTransform [i].position.z;
		}
			
		smallX -= 40 + (largeX - smallX)/2;
		smallZ -= 140 + (largeZ - smallZ)/2;
		largeX += 40 + (largeX - smallX)/2;
		largeZ += 40 + (largeZ - smallZ)/2;

		Vector3 temp = this.transform.position;
		temp.x = ((largeX - smallX) / 2) + smallX + offsetX;
		temp.z = ((largeZ - smallZ) / 2.3f) + smallZ + offsetZ;
		if (endG) {
			float distCov = (Time.time - startTime) * speed;
			journeyLength = Vector3.Distance (startPos.position, temp);
			float fracJourney = distCov / journeyLength;
			this.transform.position = Vector3.Lerp (startPos.position, temp, fracJourney);

			distX = largeX - smallX;
			distZ = largeZ - smallZ;

			if (distX > distZ) {
				Camera.main.fieldOfView = distX / fieldOfView;
				targetFOV = Camera.main.fieldOfView;
			} else {
				Camera.main.fieldOfView = distZ / fieldOfView;
				targetFOV = Camera.main.fieldOfView;
			}
			journeyFOV = Mathf.Abs (targetFOV - startFOV);
			distCov = (Time.time - startTime) * speedFOV;
			fracJourney = distCov / journeyFOV;
			Camera.main.fieldOfView = Mathf.Lerp (startFOV, targetFOV, fracJourney);

		} else {
			this.transform.position = temp;

			distX = largeX - smallX;
			distZ = largeZ - smallZ;

			if (distX > distZ) {
				Camera.main.fieldOfView = distX / fieldOfView;
				targetFOV = Camera.main.fieldOfView;
			} else {
				Camera.main.fieldOfView = distZ / fieldOfView;
				targetFOV = Camera.main.fieldOfView;
			}
		}
	}

	public void updateValues(){
		valuesTransform.Clear ();
		foreach (UserHandler.Player p in UserHandler.getInstance().players) {
            if (p != null && p.playerObject != null) {
                valuesTransform.Add(p.playerObject.transform );
            }
		}
	}

	public void endGame(){
		endG = true;
		startTime = Time.time;
		startPos = this.transform;
		startFOV = Camera.main.fieldOfView;
	

	}
}
