using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCamera : MonoBehaviour {
    public Transform followee;
    public Transform cameraCenter;
    public float maxx;
    public float minx;
    void Update() {
        float x = followee.position.x;
        float myx = transform.position.x;
        float newx = Mathf.Lerp(myx, x, .8f * Time.deltaTime);
        newx = Mathf.Clamp(newx, minx, maxx);
        var pos = transform.position;
        pos.x = newx;
        transform.position = pos;
    }
    public void Follow(Transform tr) {
        followee = tr;
    }
    public void StopFollow() {
        followee = cameraCenter;
        transform.position = cameraCenter.position;
    }
}