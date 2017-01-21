using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;

public class cameraController : MonoBehaviour {

	private float offsetX;
	private float offsetZ;
	private float fieldOfView = 14f;
	private List<Transform> valuesTransform = new List<Transform> ();

	private float largeX = -10000000;
	private float largeZ = -10000000;
	private float smallX = 10000000;
	private float smallZ = 10000000;
	private float distZ;
	private float distX;
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
			
		smallX -= 60 + (largeX - smallX)/2;
		smallZ -= 180 + (largeZ - smallZ)/2;
		largeX += 60 + (largeX - smallX)/2;
		largeZ += 60 + (largeZ - smallZ)/2;

		Vector3 temp = this.transform.position;
		temp.x = ((largeX - smallX) / 2) + smallX + offsetX;
		temp.z = ((largeZ - smallZ) / 2.3f) + smallZ + offsetZ;
		this.transform.position = temp;

	//	if (distZ * distX > (largeZ - smallZ) * (largeX - smallX) ){
	//		if(fieldOfView < 10)
	//			fieldOfView += 0.01f;
	//	} else if (distZ * distX < (largeZ - smallZ) * (largeX - smallX) ){
	//		if (fieldOfView > 5)
	//			fieldOfView -= 0.01f;
	//	}

		distX = largeX - smallX;
		distZ = largeZ - smallZ;

		if (distX > distZ) {
			Camera.main.fieldOfView = distX / fieldOfView;
		} else {
			Camera.main.fieldOfView = distZ / fieldOfView;
		}
	}

	public void updateValues(){
		valuesTransform.Clear ();

		foreach (var id in AirConsole.instance.GetActivePlayerDeviceIds) {
			var playerID = AirConsole.instance.ConvertDeviceIdToPlayerNumber (id);
			var boat = GameDataManager.instance.GetPlayer (playerID);
			valuesTransform.Add (boat.transform);
		}
			
	}
}
