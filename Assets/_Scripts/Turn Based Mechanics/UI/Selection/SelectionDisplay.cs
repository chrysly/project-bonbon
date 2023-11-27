using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SelectionDisplay : MonoBehaviour {
    [SerializeField] private GameObject cursorPrefab;
    private GameObject _cursorInstance;

    [SerializeField] private float expandDuration = 0.3f;
    [SerializeField] private float innerRingDelay = 0.2f;

    private bool _firstSpawn = true;
    private bool _active = false;
    private Transform _actor;
    
    private IEnumerator _activeAnim = null;
    
    //DEBUGGING
    [SerializeField] public List<Transform> focus;

    public void Start() {
        _cursorInstance = Instantiate(cursorPrefab);
        _cursorInstance.SetActive(false);
    }

    public void Init(GameObject reticlePrefab) {
        _cursorInstance = Instantiate(reticlePrefab);
        _cursorInstance.SetActive(false);
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            _active = true;
            FocusEntity(focus[0]);
        }

        if (_active) {
            _cursorInstance.transform.position = _actor.GetComponentInChildren<CursorIdentifier>().transform.position;
        }
    }

    /// <summary>
    /// Sets the cursor position to the target position. Spawns cursor if inactive.
    /// </summary>
    /// <param name="target"> Target actor to lock to. </param>
    public void FocusEntity(Transform target) {
        if (_activeAnim == null) {
            _activeAnim = ActivateAction(target);
            StartCoroutine(_activeAnim);
        }

        _actor = target;
    }

    public void Deactivate() {
        
    }
    
    private IEnumerator ActivateAction(Transform target) {
        if (_firstSpawn) {
            _cursorInstance.SetActive(true);
            _cursorInstance.transform.position = target.GetChild(0).position;
            Transform inner = _cursorInstance.transform.GetChild(1);
            Transform outer = _cursorInstance.transform.GetChild(0);
            inner.DOScale(new Vector3(0, 1f, 1f), 0f);
            outer.DOScale(new Vector3(1f, 0f, 1f), 0f);
            outer.DOScale(1f, expandDuration).SetEase(Ease.OutBounce);
            outer.DOLocalRotate(new Vector3(0, 0, 0f), expandDuration + 0.1f);
            yield return new WaitForSeconds(innerRingDelay);
            inner.DOScale(1f, expandDuration).SetEase(Ease.OutBounce);
            inner.DOLocalRotate(new Vector3(0, 0, 0), expandDuration + 0.1f);
            _firstSpawn = false;
        }
    }
}
