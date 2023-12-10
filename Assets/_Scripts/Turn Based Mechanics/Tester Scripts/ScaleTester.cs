using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScaleTester : MonoBehaviour
{
    // Update is called once per frame
    private void Start() {
        transform.DOScale(new Vector3(0f, 0f, 1f), 1f);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            transform.DOScale(new Vector3(1f, 1f, 1f), 1f);
        }
    }
}
