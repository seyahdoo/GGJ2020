using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    
    public List<Goo> goos = new List<Goo>();
    public List<Door> doors = new List<Door>();
    public NormalCamera cam;
    
    
    public void Reset() {
        foreach (var goo in goos) {
            goo.Reset();
        }
        foreach (var door in doors) {
            door.Reset();
        }
        cam.StopFollow();
    }

    public void GotoStart() {
        foreach (var goo in goos) {
            goo.Reset();
        }
    }
    
}
