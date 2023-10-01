using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAnim : MonoBehaviour {

    [SerializeField] private float moveSpeed;
    [SerializeField] private CollCheck innerRB;
    private Animator anim;
    private Rigidbody outerRB;
    private float timer;
    private bool begin;
    private bool jumping => innerRB.jumping;

    void Start() {
        outerRB = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        anim.enabled = false;
        timer = Random.Range(0f, 3.5f);
    }

    void Update() {
        if (timer > 0) timer -= Time.deltaTime;
        else anim.enabled = true;
        if (Input.GetKeyDown(KeyCode.A)) begin = true;
        if (begin && jumping) Move();
    }

    private void Move() => outerRB.Move(transform.position + Vector3.forward * moveSpeed, transform.rotation);
}
