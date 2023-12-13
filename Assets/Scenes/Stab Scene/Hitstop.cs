using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class HitStop : MonoBehaviour {
    bool waiting = false;
    [SerializeField] private List<Animator> _animators;
    [SerializeField] private float hitDuration;
    [SerializeField] private float hitDelay;
    public void Stop(float duration, float timeScale){
        if (waiting)
            return;
        foreach(Animator animator in _animators) {
            animator.speed = 0;
        }
        StartCoroutine(Wait(duration));
    }
    public void Stop(float duration){
        Stop(duration, 0.0f);
    }
    IEnumerator Wait(float duration){
        waiting = true;
        yield return new WaitForSeconds(0.06f);
        yield return new WaitForSeconds(0.06f);
        yield return new WaitForSeconds(0.06f);
        yield return new WaitForSeconds(duration);
        _animators[0].transform.DOMoveY(_animators[0].transform.position.y - .5f, 0f);
        _animators[0].transform.DOMoveX(_animators[0].transform.position.x - 2f, 0f);
        //_animators[0].transform.DOScale(new Vector3(0, 0, 0), 0.1f);
        foreach (Animator animator in _animators) {
            animator.speed = 1;
        }
        waiting = false;
    }

    public void Start() {
        StartCoroutine(StartAction());
    }

    IEnumerator StartAction() {
        _animators[0].speed = 0.5f;
        yield return new WaitForSeconds(hitDelay);
        Stop(hitDuration);
    }
}
