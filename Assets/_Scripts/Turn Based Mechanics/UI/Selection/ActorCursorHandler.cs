using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace BattleUI {
    public class ActorCursorHandler : CursorHandler {

        [SerializeField] private float animationLength;
        [SerializeField] private float idleCursorOffset;

        private UIAnimatorState state;

        public override void FocusEntity(Transform target) {
            
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
            transform.DOScale(Vector3.zero, 0);
            transform.DOScale(Vector2.one, animationLength).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(animationLength);
        }

        private IEnumerator Idle() {
            transform.localPosition = new Vector2(transform.localPosition.x,
                                                  Mathf.Sin(Time.time) * idleCursorOffset);
            yield return null;
        }

        private IEnumerator Unload() {
            transform.DOScale(Vector3.zero, 0.2f);
            yield return new WaitForSeconds(0.2f);
        }
    }
}