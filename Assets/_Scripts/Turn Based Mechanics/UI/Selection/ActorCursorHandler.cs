using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace BattleUI {
    public class ActorCursorHandler : CursorHandler {

        [SerializeField] private float animationLength;
        [SerializeField] private float idleCursorOffset;
        [SerializeField] private GameObject cursorPrefab;

        private Transform _activeCursor;

        private UIAnimatorState state;

        private void Awake() {
            _activeCursor = Instantiate(cursorPrefab, transform).transform;
        }

        public override void FocusEntity(Transform target) {
            if (_activeCursor == null) {
                state = UIAnimatorState.Loading;
            }
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
                        state = UIAnimatorState.Idle;
                        break;
                } yield return null;
            }
        }

        private IEnumerator Load() {
            _activeCursor.DOScale(Vector3.zero, 0);
            _activeCursor.DOScale(Vector2.one, animationLength).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(animationLength);
        }

        private IEnumerator Idle() {
            _activeCursor.localPosition = new Vector2(transform.localPosition.x,
                                                  Mathf.Sin(Time.time) * idleCursorOffset);
            yield return null;
        }

        private IEnumerator Unload() {
            _activeCursor.DOScale(Vector3.zero, 0.2f);
            yield return new WaitForSeconds(0.2f);
        }
    }
}