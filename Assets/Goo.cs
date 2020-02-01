using System;
using System.Collections.Generic;
using UnityEngine;

public class Goo : MonoBehaviour {

    public Game game;
    
    public Rigidbody2D rigid;
    public SpriteRenderer spriteRenderer;
    public Animator anim;
    public NormalCamera cam;
    public Collider2D otherVulnerableTrigger;
    public Collider2D winTrigger;
    public Vector2 defaultPosition;
    public Vector2 defaultVelocity;
    public Transform animTransform;
    
    public bool rightSide = false;
    public float dashSpeed;
    public float maxWindupTime = 5f;
    
    public bool windupStarted = false;
    public float windupStartTime;
    public Vector2 windupStartPos;
    public Vector2 windupLastDirection;
    public bool vulnerable = false;
    
    public List<Touch> myTouches = new List<Touch>();

    private void Awake() {
        Reset();
    }

    public void Reset() {
        transform.position = defaultPosition;
        rigid.velocity = defaultVelocity;
        animTransform.right = rigid.velocity;
        windupStarted = false;
        vulnerable = false;
    }

    private void Update()
    {
        
        myTouches.Clear();
        foreach (var touch in Input.touches) {
            if (touch.position.x > (Screen.width / 2) && rightSide) {
                myTouches.Add(touch);
            }

            if (touch.position.x < (Screen.width / 2) && !rightSide) { 
                myTouches.Add(touch);
            }
        }
        
        foreach (var touch in myTouches) {
            if (!windupStarted) {
                WindupStart(touch.position);
            }
            Vector2 windupDirection = touch.position - windupStartPos;
            windupDirection = windupDirection.normalized;
            WindupDash(windupDirection); 
        }

        if (myTouches.Count <= 0) {
            if (windupStarted) {
                float timePass = Time.time - windupStartTime;
                timePass = Mathf.Max(timePass, maxWindupTime);
                Dash(-windupLastDirection * timePass * timePass);
            }
        }

        if (!windupStarted) {
            animTransform.right = rigid.velocity;
        }

    }
    private void OnTriggerEnter2D(Collider2D other) {
        //if enter opposite side, be vulnerable
        if (other == otherVulnerableTrigger)
            vulnerable = true;

        if (other == winTrigger) {
            cam.Follow(transform);
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        //if exit opposite side, not be vulnerable
        if (other == otherVulnerableTrigger)
            vulnerable = false;
    }
    private void OnCollisionEnter2D(Collision2D other) {
        var g = other.gameObject.GetComponent<Goo>();
        if (g != null) {
            g.Cancel();
            rigid.velocity = Vector2.zero;
        }

        var d = other.gameObject.GetComponent<Door>();
        if (d != null) {
            if (d.Open()) {
                //Win                
            }
            else {
                //Goto Start
            }
        }

    }
    public void WindupStart(Vector2 pos) {
        windupStartPos = pos;
        windupStarted = true;
        windupStartTime = Time.time;
        rigid.velocity = Vector2.zero;
        anim.SetTrigger("windup_start");
    }
    public void WindupDash(Vector2 direction) {
        windupLastDirection = direction;
        animTransform.right = -direction;
    }
    public void Dash(Vector2 direction) {
        rigid.velocity = direction * dashSpeed;
        windupStarted = false;
        anim.SetTrigger("dash");
    }
    public void Cancel() {
        if (vulnerable) {
            Reset();
            anim.SetTrigger("dashcancel");
        }
    }
}
