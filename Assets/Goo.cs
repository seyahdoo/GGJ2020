using System.Collections.Generic;
using UnityEngine;

public class Goo : MonoBehaviour {

    public Game game;
    
    public Rigidbody2D rigid;
    public SpriteRenderer spriteRenderer;
    public Collider2D otherVulnerableTrigger;
    public Collider2D winTrigger;
    public Vector2 defaultPosition;
    
    public bool RightSide = false;
    public float dashSpeed;
    public float maxWindupTime = 5f;
    public Color windupStartColor = Color.white;
    public Color windupChargedColor = Color.red;
    
    public bool windupStarted = false;
    public float windupStartTime;
    public Vector2 windupStartPos;
    public Vector2 windupLastDirection;
    public bool vulnerable = false;
    
    
    public List<Touch> myTouches = new List<Touch>();

    private void Awake() {
        game.goos.Add(this);
    }

    public void Reset() {
        transform.position = defaultPosition;
        windupStarted = false;
        vulnerable = false;
        rigid.velocity = Vector2.zero;
    }

    // Update is called once per frame
    private void Update()
    {
        myTouches.Clear();
        if (Input.GetMouseButton(0) && RightSide && (Input.mousePosition.x > (Screen.width / 2))) {
            var t = new Touch();
            t.position = Input.mousePosition;
            myTouches.Add(t);
        }
        foreach (var touch in Input.touches) {
            if (touch.position.x > (Screen.width / 2) && RightSide) {
                myTouches.Add(touch);
            }

            if (touch.position.x < (Screen.width / 2) && !RightSide) { 
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
                Dash(-windupLastDirection * timePass * timePass);
            }
        }
        
    }
    private void OnTriggerEnter2D(Collider2D other) {
        //if enter opposite side, be vulnerable
        if (other == otherVulnerableTrigger)
            vulnerable = true;

        if (other == winTrigger) {
            game.Reset();
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        //if exit opposite side, not be vulnerable
        if (other == otherVulnerableTrigger)
            vulnerable = false;
    }
    private void OnCollisionEnter2D(Collision2D other) {
        Goo g = other.gameObject.GetComponent<Goo>();
        if (g != null) {
            g.Cancel();
            rigid.velocity = Vector2.zero;
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
        if (vulnerable) {
            Reset();
        }
    }
    
}
