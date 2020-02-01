using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game", menuName = "Game")]
public class Game : MonoBehaviour {
    
    public List<Goo> goos = new List<Goo>();
    
    public void Reset() {
        foreach (var goo in goos) {
            goo.Reset();
        }
    }
    
}
