using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class BattleTargetWindow : MonoBehaviour {
    private IEnumerator _activeAnimation;

    [SerializeField] private GameObject entityCursor;
    [SerializeField] private float moveDuration;
    private Transform activeCursor;
    private List<Actor> targets;
    private int activeIndex;
    private bool active;
    
    public void Initialize(List<Actor> actors) {
        activeIndex = 0;
        active = true;
        targets = new List<Actor>(actors);
        targets.Remove(gameObject.GetComponent<Actor>());
        activeCursor = Instantiate(entityCursor, targets[0].transform.position, targets[0].transform.rotation).transform;
    }

    private void LateUpdate() {
        if (active) {
            activeCursor.LookAt(Camera.main.transform);
        }
    }

    public void Select(bool down) {
        if (_activeAnimation == null) {
            _activeAnimation = SelectAction(down);
            StartCoroutine(_activeAnimation);
        }
    }
    
    public IEnumerator SelectAction(bool down) {
        if (down) {
            if (activeIndex >= targets.Count - 1) activeIndex = 0;
            else activeIndex++;
        }
        else {
            if (activeIndex <= 0) activeIndex = targets.Count - 1;
            else activeIndex--;
        }

        activeCursor.DOMove(targets[activeIndex].transform.position, moveDuration);
        yield return new WaitForSeconds(moveDuration);
        _activeAnimation = null;
        yield return null;
    }

    public Actor Confirm() {
        return targets[activeIndex];
    }

    public void Disable() {
        targets = null;
        active = false;
        Destroy(activeCursor.gameObject);
    }
}
