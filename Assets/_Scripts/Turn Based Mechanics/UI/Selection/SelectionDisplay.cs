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

    [SerializeField] private float moveDuration = 0.3f;

    private bool _firstSpawn = true;
    private bool _active = false;
    private bool _innerTransition = false;
    private bool _outerTransition = false;
    private Transform _actor;
    
    private IEnumerator _activeAnim = null;

    private Transform inner;
    private Transform outer;
    
    //DEBUGGING
    [SerializeField] public List<Transform> focus;

    public void Start() {
        Init(cursorPrefab);
    }

    public void Init(GameObject reticlePrefab) {
        _cursorInstance = Instantiate(reticlePrefab);
        inner = _cursorInstance.transform.GetChild(1);
        outer = _cursorInstance.transform.GetChild(0);
        _cursorInstance.SetActive(false);
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            FocusEntity(focus[0]);
        } else if (Input.GetKeyDown(KeyCode.B)) {
            FocusEntity(focus[1]);
        }

        if (_active && !_innerTransition) {
            inner.position = _actor.GetComponentInChildren<CursorIdentifier>().transform.position;
            inner.Rotate(Vector3.forward * (Time.deltaTime * 20));
        }

        if (_active && !_outerTransition) {
            outer.position = _actor.GetComponentInChildren<CursorIdentifier>().transform.position;
            outer.Rotate(Vector3.forward * (Time.deltaTime * -20));
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
    }
    
    private IEnumerator ActivateAction(Transform target) {
        if (_firstSpawn) {
            _actor = target;
            _cursorInstance.SetActive(true);
            _cursorInstance.transform.position = target.GetChild(0).position;
            inner.DOScale(new Vector3(0, 1f, 1f), 0f);
            outer.DOScale(new Vector3(1f, 0f, 1f), 0f);
            outer.DOScale(1f, expandDuration).SetEase(Ease.OutBounce);
            outer.DOLocalRotate(new Vector3(0, 0, 0f), expandDuration + 0.1f);
            yield return new WaitForSeconds(innerRingDelay);
            inner.DOScale(1f, expandDuration).SetEase(Ease.OutBounce);
            inner.DOLocalRotate(new Vector3(0, 0, 0), expandDuration + 0.1f);
            _firstSpawn = false;
        }
        else {
            ApplySelectShader(_actor, false);
            _actor = target;
            _innerTransition = _outerTransition = true;
            inner.DOMove(target.GetComponentInChildren<CursorIdentifier>().transform.position, moveDuration);
            outer.DOScale(new Vector3(1f, 0f, 1f), 0f);
            outer.DOScale(1f, expandDuration).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(innerRingDelay);
            _innerTransition = false;
            inner.DOScale(new Vector3(0, 1f, 1f), 0f);
            inner.DOScale(1f, expandDuration).SetEase(Ease.OutBounce);
            outer.DOMove(target.GetComponentInChildren<CursorIdentifier>().transform.position, moveDuration);
            yield return new WaitForSeconds(moveDuration);
            _outerTransition = false;
        }
        ApplySelectShader(_actor, true);
        
        _activeAnim = null;
        yield return null;
        _active = true;
    }

    private void ApplySelectShader(Transform target, bool apply) {
        SkinnedMeshRenderer[] skins = target.GetComponentsInChildren<SkinnedMeshRenderer>();
        MeshRenderer[] meshSkin = target.GetComponentsInChildren<MeshRenderer>();
        foreach (SkinnedMeshRenderer skin in skins) {
            if (apply) skin.gameObject.layer = LayerMask.NameToLayer("Select");
            else skin.gameObject.layer = LayerMask.NameToLayer("Default");
        }

        foreach (MeshRenderer mesh in meshSkin) {
            if (apply) mesh.gameObject.layer = LayerMask.NameToLayer("Select");
            else mesh.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
    
    public void Deactivate() {
        if (_activeAnim == null) {
            _activeAnim = DeactivateAction();
            StartCoroutine(_activeAnim);
        }

        _actor = null;
    }

    private IEnumerator DeactivateAction() {
        _active = false;
        _firstSpawn = true;
        inner.DOScale(1f, expandDuration).SetEase(Ease.OutBounce);
        inner.DOLocalRotate(new Vector3(0, 0, -90f), expandDuration + 0.1f);
        inner.DOScale(new Vector3(0.5f, 0f, 0.5f), expandDuration);
        yield return new WaitForSeconds(innerRingDelay / 2);
        outer.DOScale(1f, expandDuration).SetEase(Ease.OutBounce);
        outer.DOLocalRotate(new Vector3(0, 0, 90f), expandDuration + 0.1f);
        outer.DOScale(new Vector3(0.5f, 0f, 0.5f), expandDuration);
        yield return new WaitForSeconds(expandDuration);
        _cursorInstance.SetActive(false);
        _activeAnim = null;
        yield return null;
    }
}
