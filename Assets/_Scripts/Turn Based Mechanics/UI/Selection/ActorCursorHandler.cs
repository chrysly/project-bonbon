using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace BattleUI {
    public class ActorCursorHandler : CursorHandler {

        [SerializeField] private float animationLength = 0.2f;
        [SerializeField] private float idleCursorOffset;
        [SerializeField] private GameObject cursorPrefab;

        private Transform _activeCursor;
        private Transform _target;

        private UIAnimatorState state;

        private void Awake() {
            _activeCursor = Instantiate(cursorPrefab, transform).transform;
            _activeCursor.gameObject.SetActive(false);
            StartCoroutine(CoreCoroutine());
        }

        public override void FocusEntity(Transform target) {
            _target = target;
            if (_activeCursor != null) {
                state = UIAnimatorState.Loading;
            }
        }

        public override void Deactivate() {
            state = UIAnimatorState.Unloading;
        }

        protected IEnumerator CoreCoroutine() {
            while (true) {
                switch (state) {
                    case UIAnimatorState.Loading:
                        IEnumerator loadCoroutine = Load();
                        while (loadCoroutine.MoveNext())
                            yield return loadCoroutine.Current;
                        state = UIAnimatorState.Idle;
                        break;
                    case UIAnimatorState.Idle:
                        IEnumerator idleCoroutine = Idle();
                        while (idleCoroutine.MoveNext())
                            yield return idleCoroutine.Current;
                        break;
                    case UIAnimatorState.Unloading:
                        IEnumerator unloadCoroutine = Unload();
                        while (unloadCoroutine.MoveNext())
                            yield return unloadCoroutine.Current;
                        break;
                } yield return null;
            }
        }

        private IEnumerator Load() {
            Debug.Log("Loading cursor");
            if (!_activeCursor.gameObject.activeSelf) {
                Debug.Log("Loading active cursor");
                _activeCursor.gameObject.SetActive(true);
                _activeCursor.position = _target.position;
            } else {
                _activeCursor.DOMove(_target.position, animationLength);
            }
            
            _activeCursor.DOScale(Vector3.zero, 0);
            _activeCursor.DOScale(Vector2.one, animationLength).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(animationLength);
            state = UIAnimatorState.Idle;
        }

        private IEnumerator Idle() {
            // _activeCursor.localPosition = new Vector2(_activeCursor.localPosition.x,
            //                                       Mathf.Sin(Time.time) * idleCursorOffset);
            yield return null;
        }

        private IEnumerator Unload() {
            _activeCursor.gameObject.SetActive(false);
            _activeCursor.DOScale(Vector3.zero, 0.2f);
            yield return new WaitForSeconds(0.2f);
        }
    }
}