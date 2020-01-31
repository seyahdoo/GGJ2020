using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goo : MonoBehaviour {
    
    public Rigidbody2D rb;
    public bool RightSide = false;
    public float DashSpeedMultiplier;

    public bool windupStarted = false;
    public Vector2 windupStartPos;
    public Vector2 windupLastDirection;
    
    public List<Touch> myTouches = new List<Touch>();
    
    // Update is called once per frame
    private void Update()
    {
        myTouches.Clear();
        foreach (var touch in Input.touches) {
            if ((touch.position.x > (Screen.width / 2) && RightSide) || (touch.position.x < (Screen.width / 2) && !RightSide)) {
                myTouches.Add(touch);
            }
        }
        
        foreach (var touch in myTouches) {
            if (!windupStarted) {
                windupStartPos = touch.position;
                windupStarted = true;
            }
            Vector2 windupDirection = touch.position - windupStartPos;
            windupDirection.x /= Screen.width;
            windupDirection.y /= Screen.height;
                
            WindupDash(windupDirection); 
        }

        if (myTouches.Count <= 0) {
            if (windupStarted) {
                Dash(-windupLastDirection);
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        other.gameObject.GetComponent<Goo>().Cancel();
    }

    public void WindupDash(Vector2 direction) {
        windupLastDirection = direction;
    }
    
    public void Dash(Vector2 direction) {
        rb.velocity = direction * DashSpeedMultiplier;
    }

    public void Cancel() {
                
    }
    
}
