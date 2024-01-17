using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BattleUI.TurnOrder {
    public class Portrait : MonoBehaviour {

        private TurnOrderDisplay tod;
        private RectTransform rectTransform;

        private int position;
        ///private State state;
        private bool expanded;

        [SerializeField] private Image background;
        [SerializeField] private Image profile;
        [SerializeField] private Image selector;

        [SerializeField] private float bgExtendedWidth;
        [SerializeField] private float selectedScale;
        [SerializeField] private RectTransform selectedPosition;

        private float targetYPos;
        private float selectLerp;
        private Vector2 reloVelocity;
        private Vector2 selectVelocity;

        private Vector2 selectorDelta;

        void Awake() {
            rectTransform = GetComponent<RectTransform>();
            selectorDelta = selector.rectTransform.sizeDelta;
            RectTransform bgRect = background.rectTransform;
            bgRect.sizeDelta = new Vector2(0, bgRect.sizeDelta.y);
            profile.color = new Color(profile.color.r, profile.color.g, profile.color.b, 0);
            selector.rectTransform.sizeDelta = new Vector2(selectorDelta.x, 0);
        }

        public void Init(TurnOrderDisplay tod) {
            this.tod = tod;
            StartCoroutine(CoreCoroutine());
            ///tod.OnPortraitUpdate += OnPortraitUpdate;
        }

        public void UpdateActor(Actor actor) => profile.sprite = actor.Data.Icon;
        public void UpdatePos(int position) {
            this.position = position;
            targetYPos = tod.rectTransform.anchoredPosition.y - tod.GraphicHeight * position;
        }
        /*
        private void OnPortraitUpdate(State state) {
            bool deprecated = position < 0;
            switch (state) {
                case State.Cleanup:
                    if (deprecated) this.state = State.Cleanup;
                    break;
                case State.Relocate:
                    if (!deprecated) this.state = State.Relocate;
                    break;
                case State.Select:
                    if (!deprecated) this.state = State.Select;
                    break;
            }
        }*/

        private IEnumerator CoreCoroutine() {
            while (true) {
                if (position < 0) {
                    RectTransform bgRect = background.rectTransform;
                    if (Mathf.Approximately(bgRect.anchoredPosition.x, selectedPosition.position.x)) {
                        RectTransform pfRect = profile.rectTransform;
                        RectTransform slRect = selector.rectTransform;
                        float offset = Vector2.Distance(bgRect.anchoredPosition, selectedPosition.anchoredPosition);
                        pfRect.DOAnchorPos(new Vector2(pfRect.anchoredPosition.x - offset, pfRect.anchoredPosition.y), tod.SpawnDuration * (2 / 3));
                        bgRect.DOAnchorPos(slRect.anchoredPosition, tod.SpawnDuration * (2 / 3));
                        slRect.DOSizeDelta(new Vector2(selectorDelta.x, 0), tod.SpawnDuration * (2 / 3));
                    } profile.DOFade(0, tod.SpawnDuration * (2 / 3));
                    yield return new WaitForSeconds(tod.SpawnDuration * (2 / 5));
                    bgRect.DOSizeDelta(new Vector2(0, 95), tod.SpawnDuration);
                    yield return new WaitForSeconds(tod.SpawnDuration * (3 / 5));
                    Destroy(gameObject);
                    break;
                } else {
                    /// Relocate Portrait;
                    rectTransform.anchoredPosition = Vector2.SmoothDamp(rectTransform.anchoredPosition,
                                                                        new Vector2(tod.rectTransform.anchoredPosition.x, targetYPos),
                                                                        ref reloVelocity, tod.SpawnDuration * 2);
                    /// Expand Portrait;
                    RectTransform bgRect = background.rectTransform;
                    bgRect.sizeDelta = Vector2.SmoothDamp(bgRect.sizeDelta, new Vector2(bgExtendedWidth, 95), ref selectVelocity, tod.SpawnDuration * (2 / 3));
                    profile.color = new Color(profile.color.r, profile.color.g, profile.color.b,
                                              Mathf.MoveTowards(profile.color.a, 1, Time.deltaTime * 2));
                    if (position == 0 && Mathf.Approximately(rectTransform.anchoredPosition.y, targetYPos)) {
                        RectTransform pfRect = profile.rectTransform;
                        float offset = Vector2.Distance(bgRect.anchoredPosition, selectedPosition.anchoredPosition);
                        pfRect.DOAnchorPos(new Vector2(pfRect.anchoredPosition.x + offset, pfRect.anchoredPosition.y),
                                                       tod.SpawnDuration).SetEase(Ease.OutBounce);
                        bgRect.DOAnchorPos(selectedPosition.anchoredPosition, tod.SpawnDuration / 2).SetEase(Ease.OutBounce);
                        yield return new WaitForSeconds(tod.SpawnDuration * 0.75f);
                        selector.rectTransform.DOSizeDelta(selectorDelta, tod.SpawnDuration).SetEase(Ease.OutBounce);
                        yield return new WaitForSeconds(tod.SpawnDuration * 0.75f);
                    }
                } yield return null;
                /*
                switch (state) {
                    case State.Cleanup:
                        IEnumerator cleanupCoroutine = Cleanup();
                        while (cleanupCoroutine.MoveNext()) {
                            yield return cleanupCoroutine.Current;
                        } break;
                    case State.Relocate:
                        IEnumerator relocateCoroutine = Relocate();
                        while (relocateCoroutine.MoveNext()) {
                            yield return relocateCoroutine.Current;
                        } break;
                    case State.Select:
                        IEnumerator selectCoroutine = Select();
                        while (selectCoroutine.MoveNext()) {
                            yield return selectCoroutine.Current;
                        } break;
                } yield return null;*/
            }
        }

        private IEnumerator Cleanup() {
            RectTransform bgRect = background.rectTransform;
            if (Mathf.Approximately(bgRect.anchoredPosition.x, selectedPosition.position.x)) {
                RectTransform pfRect = profile.rectTransform;
                RectTransform slRect = selector.rectTransform;
                float offset = Vector2.Distance(bgRect.anchoredPosition, selectedPosition.anchoredPosition);
                pfRect.DOAnchorPos(new Vector2(pfRect.anchoredPosition.x - offset, pfRect.anchoredPosition.y), tod.SpawnDuration * (2/3));
                bgRect.DOAnchorPos(slRect.anchoredPosition, tod.SpawnDuration * (2/3));
                slRect.DOSizeDelta(new Vector2(selectorDelta.x, 0), tod.SpawnDuration * (2/3));
            } profile.DOFade(0, tod.SpawnDuration * (2/3));
            yield return new WaitForSeconds(tod.SpawnDuration * (2/5));
            bgRect.DOSizeDelta(new Vector2(0, 95), tod.SpawnDuration);
            yield return new WaitForSeconds(tod.SpawnDuration * (3/5));
            Destroy(gameObject);
        }

        private IEnumerator Relocate() {
            if (expanded) {
                rectTransform.DOAnchorPos(new Vector2(tod.rectTransform.anchoredPosition.x,
                                                      tod.rectTransform.anchoredPosition.y - tod.GraphicHeight * position), tod.SpawnDuration * 2);
            } else {
                RectTransform bgRect = background.rectTransform;
                bgRect.DOSizeDelta(new Vector2(bgExtendedWidth, 95),tod.SpawnDuration * (2/3)).SetEase(Ease.OutBounce);
                yield return new WaitForSeconds(tod.SpawnDuration * (2/5));
                profile.DOFade(1, tod.SpawnDuration * (1/3));
                yield return new WaitForSeconds(tod.SpawnDuration * (1/5));
                expanded = true;
            }
        }

        private IEnumerator Select() {
            if (position == 0) {
                RectTransform bgRect = background.rectTransform;
                RectTransform pfRect = profile.rectTransform;
                float offset = Vector2.Distance(bgRect.anchoredPosition, selectedPosition.anchoredPosition);
                pfRect.DOAnchorPos(new Vector2(pfRect.anchoredPosition.x + offset, pfRect.anchoredPosition.y),
                                               tod.SpawnDuration).SetEase(Ease.OutBounce);
                bgRect.DOAnchorPos(selectedPosition.anchoredPosition, tod.SpawnDuration / 2).SetEase(Ease.OutBounce);
                yield return new WaitForSeconds(tod.SpawnDuration * 0.75f);
                selector.rectTransform.DOSizeDelta(selectorDelta, tod.SpawnDuration).SetEase(Ease.OutBounce);
                yield return new WaitForSeconds(tod.SpawnDuration * 0.75f);
            }
        }
    }
}