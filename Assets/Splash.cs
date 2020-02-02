using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Splash : MonoBehaviour {
    public Game game;
    public RawImage black;
    public RawImage image1;
    public RawImage image2;

    private void Awake() {
        DoSplash();
    }
    public void DoSplash() {
        StartCoroutine(nameof(FadeStuff));
    }
    IEnumerator FadeStuff() {
        var a = 0f;
        while (true) {
            a += Time.deltaTime*4;
            var c = image1.color;
            c.a = a;
            image1.color = c;
            if (a >= 1f) {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        black.color = Color.black;
        yield return new WaitForSeconds(2f);
        while (true) {
            a -= Time.deltaTime*4;
            var c = image1.color;
            c.a = a;
            image1.color = c;
            if (a <= 0f) {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        
        while (true) {
            a += Time.deltaTime*4;
            var c = image2.color;
            c.a = a;
            image2.color = c;
            if (a >= 1f) {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        black.color = Color.clear;
        yield return new WaitForSeconds(2f);
        game.Reset();
        while (true) {
            a -= Time.deltaTime*4;
            var c = image2.color;
            c.a = a;
            image2.color = c;
            if (a <= 0f) {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    
}
