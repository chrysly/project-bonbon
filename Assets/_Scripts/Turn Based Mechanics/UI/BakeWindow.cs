using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BakeWindow : MonoBehaviour {
    [SerializeField] private Transform startPoint;
    [SerializeField] private BonbonWindow bonbonWindow;
    private Vector3 startPos;
    private Vector3 endPoint;

    private void Start() {
        endPoint = transform.position;
        startPos = startPoint.position;
        transform.DOMove(startPos, 0);
        transform.DOScale(new Vector3(0, 1, 1), 0);
    }

    public void Activate(CharacterActor actor) {
        bonbonWindow.ReloadActor(actor);
        transform.DOScale(1, 0.6f);
        transform.DOMove(endPoint, 0.5f);
    }

    public void Deactivate() {
        transform.DOScale(new Vector3(1, 1, 0), 0.6f);
        transform.DOMove(startPos, 0.5f);
    }
}
