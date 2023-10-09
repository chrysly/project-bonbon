using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollCheck : MonoBehaviour {

    [SerializeField] bool hey;
    public bool jumping = true;

    void OnCollisionEnter(Collision collision) {
        if (hey) Debug.Log("brusky");
        if (collision.collider is MeshCollider) jumping = false;
    }

    void OnCollisionExit(Collision collision) {
        if (hey) Debug.Log("bruh");
        if (collision.collider is MeshCollider) jumping = true;
    }
}