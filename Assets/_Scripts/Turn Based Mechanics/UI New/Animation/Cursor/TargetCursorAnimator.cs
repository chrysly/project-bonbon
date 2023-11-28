using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace BattleUI {
    public class TargetCursorAnimator : CursorAnimator {

        [SerializeField] private float expandDuration = 0.3f;
        [SerializeField] private float innerRingDelay = 0.2f;

        protected override IEnumerator _Spawn(Transform target) {
            transform.parent = target;
            Transform inner = transform.GetChild(1);
            Transform outer = transform.GetChild(0);
            inner.DOScale(new Vector3(0, 1f, 1f), 0f);
            outer.DOScale(new Vector3(1f, 0f, 1f), 0f);
            outer.DOScale(1f, expandDuration).SetEase(Ease.OutBounce);
            outer.DOLocalRotate(Vector3.zero, expandDuration + 0.1f);
            yield return new WaitForSeconds(innerRingDelay);
            inner.DOScale(1f, expandDuration).SetEase(Ease.OutBounce);
            inner.DOLocalRotate(Vector3.zero, expandDuration + 0.1f);
            activeAnimation = null;
            AdvanceQueueIfValid();
        }
    }
}
