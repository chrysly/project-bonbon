using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScaleTester : MonoBehaviour {
    // Update is called once per frame
    [SerializeField] private Material lit;
    [SerializeField] private Material unlit;
    private void Start() {
        transform.DOScale(new Vector3(0f, 0f, 1f), 1f);
        transform.GetComponent<SpriteRenderer>().DOFade(0f, 0f);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            StartCoroutine(delayed());
        }
    }

    private IEnumerator delayed() {
        transform.DOScale(new Vector3(1f, 1f, 1f), 1f);
        transform.GetComponent<SpriteRenderer>().DOFade(1f, 1f);
        Material[] material = { unlit };
        Material[] litMaterial = { lit };
        GetComponent<SpriteRenderer>().materials = material;
        yield return new WaitForSeconds(1f);
        GetComponent<SpriteRenderer>().materials = litMaterial; 
    }
}
