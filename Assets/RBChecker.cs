using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBChecker : MonoBehaviour
{
    public Rigidbody cc;
    public float x;

    void Start () {
        //cc = GetComponentInChildren<Rigidbody>();
    }
    void Update () {
        x = cc.velocity.x;
    }
}
