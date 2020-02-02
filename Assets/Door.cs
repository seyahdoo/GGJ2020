using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    public List<GameObject> doorStates;
    public bool Open() {
        for (int i = 0; i < doorStates.Count; i++) {
            if (doorStates[i].activeSelf) {
                doorStates[i].SetActive(false);
                doorStates[i+1].SetActive(true);
                return i >= doorStates.Count - 2;
            }
        }
        return false;
    }
    public bool WillFinal(){
        for (int i = 0; i < doorStates.Count; i++) {
            if (doorStates[i].activeSelf) {
                return i >= doorStates.Count - 2;
            }
        }
        return false;
    }
    
    public void Reset(){
        foreach (var d in doorStates) {
            d.SetActive(false);
        }
        doorStates[0].SetActive(true);
    }
}
