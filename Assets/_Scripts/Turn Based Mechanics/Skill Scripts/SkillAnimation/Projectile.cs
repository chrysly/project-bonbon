using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Activate(Vector3 target, float speed) {
        Debug.Log("going");
        transform.LookAt(target);
        gameObject.GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }


}
