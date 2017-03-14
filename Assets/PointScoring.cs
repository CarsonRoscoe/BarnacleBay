using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointScoring : MonoBehaviour {
	void Update () {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
	}
}
