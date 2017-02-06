using UnityEngine;
using System.Collections;

public class TopDown3DCamera : MonoBehaviour {
    public Transform Following;
    public float DelayMS = 250;
    private Vector3 m_offset;

    public void SetFollowing(Transform newFollow) {
        Following = newFollow;
        m_offset = newFollow.position - transform.position;
    }

    void Update() {
        if (Following != null ) {
            transform.position = Following.position - m_offset;
        }
    }
}
