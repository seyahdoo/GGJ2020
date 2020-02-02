using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goo : MonoBehaviour {

    public AudioSource win;
    public AudioSource collision;
    public AudioSource windup;
    public AudioSource windupLoop;
    public AudioSource dash;
    public AudioSource doorCollision;

    public Splash splash;
    
    public Transform winPos;
    public Vector2 winVelocity;
    public SpriteRenderer halo;
    public float winApproachSpeed = 1f;
    public Door otherDoor;
    
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
    public float fadeSpeed = .5f;
    
    public bool windupStarted = false;
    public float windupStartTime;
    public Vector2 windupStartPos;
    public Vector2 windupLastDirection;
    public bool vulnerable = false;
    private bool _punching = false;
    private bool winsequence = false;

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
        winsequence = false;
        anim.Play("idle");
        spriteRenderer.color = Color.white;
        crashing = false;
    }
    private void Update()
    {
        anim.SetFloat("current_speed", rigid.velocity.magnitude);
        if(winsequence) return;
        
        if (!windupStarted) {
            animTransform.right = rigid.velocity;
        }
        
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
        
    }
    private void OnTriggerEnter2D(Collider2D other) {
        //if enter opposite side, be vulnerable
        if (other == otherVulnerableTrigger)
            vulnerable = true;
        if (other == winTrigger && !crashing) {
            cam.Follow(transform);
            otherGoo.gameObject.SetActive(false);
            Vector2 vel = rigid.velocity;
            vel.x = rightSide ? -winApproachSpeed : winApproachSpeed;
            rigid.velocity = vel;
            if (otherDoor.WillFinal()) {
                win.Stop();
                win.Play();
                StartCoroutine(nameof(WinSoundFadeIn));
            }
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
            // rigid.velocity = Vector2.zero;
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
    private IEnumerator WinSoundFadeIn() {
        var a = 0f;
        while (true) {
            a += Time.deltaTime*2;
            win.volume = a;
            if (a >= 1f) {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator WinSequence() {
        yield return new WaitForSeconds(.5f);
        rigid.velocity = Vector2.zero;
        winsequence = true;
        //go down
        while (
            Vector3.Distance(transform.position, winPos.position) > .1f 
            && 
            Quaternion.Angle(transform.rotation, winPos.rotation) > 2f
            ) {
            
            transform.position = Vector3.Lerp(transform.position, winPos.position, .6f * Time.deltaTime);
            animTransform.rotation = Quaternion.RotateTowards(animTransform.rotation, winPos.rotation, 40f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        anim.SetTrigger("win");
        var tr = transform;
        tr.position = winPos.position;
        animTransform.rotation = winPos.rotation;
        //walk
        rigid.velocity = winVelocity;
        
        //Show Halo
        var a = 0f;
        while (true) {
            a += Time.deltaTime*2;
            var haloColor = halo.color;
            haloColor.a = a;
            halo.color = haloColor;
            if (a >= 1f) {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        
        yield return new WaitForSeconds(3f);
        //fade out
        while (spriteRenderer.color.a > 0f) {
            Color c = spriteRenderer.color;
            c.a -= fadeSpeed * Time.deltaTime;
            spriteRenderer.color = c;
            halo.color = c;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(2f);
        // SceneManager.LoadScene("SplashScreen");
        splash.DoSplash();
        // winsequence = false;
        // win.Stop();
        // game.Reset();
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
        rigid.velocity = Vector2.zero;
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
            StartCoroutine(nameof(CrashSequence));
        }
    }

    private bool crashing = false;
    IEnumerator CrashSequence() {
        crashing = true;
        anim.SetTrigger("crashed");
        rigid.velocity = rigid.velocity.normalized * 2f;
        yield return new WaitForSeconds(.5f);
        Reset();
    }
    
}
