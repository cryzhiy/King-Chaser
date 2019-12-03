using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public int maxAp = 6;
    public float ap = 4;
    public List<GameObject> abilities;

    public void clampAP() {
        ap = Mathf.Clamp(ap, 0, maxAp);
    }

    public void addTurnAP() {
        clampAP();
        ap += 6;
        clampAP();
    }

    public void useAP(float apCost) {
        ap -= apCost;
    }
}
