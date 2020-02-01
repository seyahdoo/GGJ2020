﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goo : MonoBehaviour {

    public AudioSource win;
    public AudioSource collision;
    public AudioSource windup;
    public AudioSource windupLoop;
    public AudioSource dash;
    public AudioSource doorCollision;

    public Transform winPos;
    public Vector2 winVelocity;
    
    public Game game;
    public Goo otherGoo;
    
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
    private bool _punching = false;

    public List<Touch> myTouches = new List<Touch>();
    private void Awake() {
        Reset();
    }
    public void Reset() {
        gameObject.SetActive(true);
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
                timePass = Mathf.Clamp(timePass, 0f, maxWindupTime);
                Dash(-windupLastDirection.normalized * timePass * timePass);
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
            otherGoo.gameObject.SetActive(false);
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
            collision.Stop(); 
            collision.Play();
            g.Cancel();
            rigid.velocity = Vector2.zero;
        }
        var d = other.gameObject.GetComponent<Door>();
        if (d != null) {
            if (d.Open()) {
                StartCoroutine(nameof(WinSequence));
            }
            else {
                if (!_punching) {
                    doorCollision.Stop();
                    doorCollision.Play();
                    StartCoroutine(nameof(DoorPunch));
                }
            }
        }
    }
    private IEnumerator WinSequence() {
        //go down
        while (
            Vector3.Distance(transform.position, winPos.position) < .1f 
            && 
            Quaternion.Angle(transform.rotation, winPos.rotation) < 2f
            ) {
            transform.position = Vector3.Lerp(transform.position, winPos.position, .1f * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, winPos.rotation, .1f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        var tr = transform;
        tr.position = winPos.position;
        tr.rotation = winPos.rotation;
        //walk
        rigid.velocity = winVelocity;
        
        yield return new WaitForSeconds(3f);
        //fade out
        // while (spriteRenderer.color.a > .002f) {
        //     Color c = spriteRenderer.color;
        //     // c.a 
        //
        // }
        
        
    }
    private IEnumerator DoorPunch() {
        _punching = true;
        yield return new WaitForSeconds(1f);
        game.GotoStart();
        _punching = false;
    }
    public void WindupStart(Vector2 pos) {
        windupStartPos = pos;
        windupStarted = true;
        windupStartTime = Time.time;
        rigid.velocity = Vector2.zero;
        anim.SetTrigger("windup_start");
        windup.Stop();
        windup.Play();
        windupLoop.Stop();
        windupLoop.Play();
    }
    public void WindupDash(Vector2 direction) {
        windupLastDirection = direction;
        animTransform.right = -direction;
    }
    public void Dash(Vector2 direction) {
        rigid.velocity = direction * dashSpeed;
        windupStarted = false;
        anim.SetTrigger("dash");
        windup.Stop();
        windupLoop.Stop();
        dash.Stop();
        dash.Play();
    }
    public void Cancel() {
        if (vulnerable) {
            Reset();
            anim.SetTrigger("dashcancel");
        }
    }
}
