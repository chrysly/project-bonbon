using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace BattleUI {
    public class GUICursorAnimator : CursorAnimator {

        [SerializeField] private float animationLength;

        protected override IEnumerator _Spawn(Transform target) {
            transform.parent = target;
            transform.DOScale(Vector3.zero, 0);
            transform.DOScale(Vector2.one, animationLength).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(animationLength);
            activeAnimation = null;
            AdvanceQueueIfValid();
        }
    }
}
