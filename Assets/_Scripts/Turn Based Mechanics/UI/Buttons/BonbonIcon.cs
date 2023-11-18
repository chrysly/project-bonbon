using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BonbonIcon : BattleButton {
    private BonbonObject _bonbonObject;
    private Vector3 _position;

    public void Start() {
        Disable();
        _position = transform.position;
    }

    public void Initialize(BonbonObject bonbonObject) {
        if (bonbonObject == null) return;
        gameObject.SetActive(true);
        _bonbonObject = bonbonObject;
        GetComponent<RawImage>().texture = _bonbonObject.Texture;
        transform.DOScale(1f, 0.1f).SetEase(Ease.OutBounce);
    }

    public void Disable() {
        transform.DOScale(0f, 0);
        gameObject.SetActive(false);
        GetComponent<RawImage>().texture = null;
        _bonbonObject = null;
    }

    public void Select() {
        if (_bonbonObject != null) transform.DOScale(1.1f, 0.25f);
    }

    public void Deselect() {
        if (_bonbonObject != null) transform.DOScale(1f, 0.25f);
    }

    public void Confirm(Vector3 selectPos) {
        if (_bonbonObject != null) transform.DOMove(selectPos, 1f);
    }

    public void Deconfirm() {
        if (_bonbonObject != null) {
            transform.DOMove(_position, 0.5f);
            transform.DOScale(1f, 0.5f);
        }
    }
}
