using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BattleUI {

    public abstract class CursorAnimator : MonoBehaviour {

        [SerializeField] protected float idleCursorOffset;

        protected Queue<IEnumerator> AnimQueue = new();
        protected IEnumerator activeAnimation;

        protected void OnEnable() => StartCoroutine(CoreCoroutine());
        protected void OnDisable() => activeAnimation = null;

        protected IEnumerator CoreCoroutine() {
            while (true) {
                if (activeAnimation == null) {
                    yield return _Idle();
                } else {
                    yield return activeAnimation;
                }
            }
        }

        public void SpawnAt(Transform target) {
            AnimQueue.Enqueue(_Spawn(target));
            AdvanceQueueIfValid();
        }

        public void Despawn() {
            AnimQueue.Enqueue(_Despawn());
            AdvanceQueueIfValid();
        }

        protected void AdvanceQueueIfValid() {
            if (activeAnimation != null || AnimQueue.Count == 0) return;
            activeAnimation = AnimQueue.Dequeue();
        }

        protected abstract IEnumerator _Spawn(Transform target);

        protected virtual IEnumerator _Idle() {
            transform.localPosition = new Vector2(transform.localPosition.x,
                                                    Mathf.Sin(Time.time) * idleCursorOffset);
            yield return null;
        }

        protected virtual IEnumerator _Despawn() {
            transform.DOScale(Vector3.zero, 0.2f);
            yield return new WaitForSeconds(0.2f);
            activeAnimation = null;
            AdvanceQueueIfValid();
        }
    }
}
