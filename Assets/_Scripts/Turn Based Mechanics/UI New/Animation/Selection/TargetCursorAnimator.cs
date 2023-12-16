using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace BattleUI {
    public class TargetCursorAnimator : UICursorAnimator {

        [SerializeField] private float expandDuration = 0.3f;
        [SerializeField] private float innerRingDelay = 0.2f;
        [SerializeField] private float moveDuration = 0.3f;

        [SerializeField] private float rotationSpeed = 20f;

        [SerializeField] private Transform inner;
        [SerializeField] private Transform outer;
        [SerializeField] private bool augmented = false;

        private IEnumerator _activeAnim;

        public override void Init() {
            inner.DOScale(new Vector3(0, 1f, 1f), 0f);
            outer.DOScale(new Vector3(1f, 0f, 1f), 0f);
        }

        protected override void Attach(UIButtonAnimator target) {
            base.Attach(target);
            ApplySelectShader(target, true);
        }

        protected override void Detach(UIButtonAnimator target) {
            base.Detach(target);
            ApplySelectShader(target, false);
        }

        protected override IEnumerator Load() {
            outer.DOScale(Vector3.one, expandDuration).SetEase(Ease.OutBounce);
            outer.DOLocalRotate(new Vector3(0, 0, 0f), expandDuration + 0.1f);
            inner.DOScale(Vector3.one, expandDuration).SetEase(Ease.OutBounce);
            inner.DOLocalRotate(new Vector3(0, 0, 0), expandDuration + 0.1f);
            yield return new WaitForSeconds(expandDuration);
        }

        private void ApplySelectShader(UIButtonAnimator target, bool apply) {
            Actor actor = target.GetComponentInParent<Actor>();
            SkinnedMeshRenderer[] skins = actor.GetComponentsInChildren<SkinnedMeshRenderer>();
            MeshRenderer[] meshSkin = actor.GetComponentsInChildren<MeshRenderer>();
            foreach (SkinnedMeshRenderer skin in skins) {
                if (apply) skin.gameObject.layer = LayerMask.NameToLayer("Select");
                else skin.gameObject.layer = LayerMask.NameToLayer("Default");
            }

            foreach (MeshRenderer mesh in meshSkin) {
                if (apply) mesh.gameObject.layer = LayerMask.NameToLayer("Select");
                else mesh.gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }

        protected override IEnumerator Idle() {
            while (state == UIAnimatorState.Idle) {
                if (target != null) {
                    inner.Rotate(Vector3.forward * (Time.deltaTime * rotationSpeed));
                    outer.Rotate(Vector3.forward * (Time.deltaTime * -rotationSpeed));

                    if (augmented && _activeAnim == null) {
                        _activeAnim = Shake();
                        StartCoroutine(_activeAnim);
                    }
                } yield return null;
            }
        }

        private IEnumerator Shake() {
            inner.DOShakeScale(0.2f, new Vector3(1, 0.7f, 1), 20);
            yield return new WaitForSeconds(0.1f);
            outer.DOShakeScale(0.2f, new Vector3(0.6f, 0.8f, 1), 20);
            yield return new WaitForSeconds(1f);
            _activeAnim = null;
            yield return null;
        }

        protected override IEnumerator Unload() {
            inner.DOLocalRotate(new Vector3(0, 0, -90f), expandDuration);
            inner.DOScale(0, innerRingDelay);
            yield return new WaitForSeconds(innerRingDelay / 2);
            outer.DOLocalRotate(new Vector3(0, 0, 90f), expandDuration);
            outer.DOScale(0, expandDuration);
            yield return new WaitForSeconds(expandDuration);
            Detach(target);
            Destroy(target.gameObject);
        }
    }
}