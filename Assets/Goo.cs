using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class Goo : MonoBehaviour {
    
    public Rigidbody2D rigid;
    public SpriteRenderer spriteRenderer;
    public bool RightSide = false;
    public float dashSpeed;
    public float maxWindupTime = 5f;
    public Color windupStartColor = Color.white;
    public Color windupChargedColor = Color.red;
    
    public bool windupStarted = false;
    public float windupStartTime;
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
                windupStartTime = Time.time;
            }
            Vector2 windupDirection = touch.position - windupStartPos;
            windupDirection = windupDirection.normalized;
            WindupDash(windupDirection); 
        }

        if (myTouches.Count <= 0) {
            if (windupStarted) {
                float timePass = Time.time - windupStartTime;
                timePass = Mathf.Min(timePass, 5f);
                Dash(-windupLastDirection * timePass);
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Goo g = other.gameObject.GetComponent<Goo>();
        if (g != null) {
            g.Cancel();
        }
    }

    public void WindupDash(Vector2 direction) {
        var timePass = Time.time - windupStartTime;
        timePass = Mathf.Min(timePass, maxWindupTime);
        timePass /= maxWindupTime;
        var c = Color.Lerp(windupStartColor, windupChargedColor, timePass);
        spriteRenderer.color = c;
        windupLastDirection = direction;
    }
    
    public void Dash(Vector2 direction) {
        rigid.velocity = direction * dashSpeed;
        windupStarted = false;
    }

    public void Cancel() {
                
    }
    
}
