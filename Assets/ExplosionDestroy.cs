using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine(KillSelf(5f));
	}
	
	IEnumerator KillSelf(float timer) {
        var currentTime = 0f;
        while(currentTime < timer) {
            currentTime += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
