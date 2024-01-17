using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BattleUI.TurnOrder {
    public class Portrait : MonoBehaviour {

        private TurnOrderDisplay tod;
        private RectTransform rectTransform;
        private int position;

        [SerializeField] private Image background;
        [SerializeField] private Image profile;
        [SerializeField] private Image selector;

        [SerializeField] private float bgExtendedWidth;
        [SerializeField] private float selectScale;
        [SerializeField] private float baseAlpha;
        
        private float targetYPos;

        private float selectLerp;
        private Vector2 reloVelocity;
        private Vector2 selectVelocity;

        private Vector2 bgSelectedPos;
        private Vector2 pfSelectedPos;
        private Vector2 selectorDelta;

        void Awake() {
            rectTransform = GetComponent<RectTransform>();
            selectorDelta = selector.rectTransform.sizeDelta;
            RectTransform bgRect = background.rectTransform;
            bgRect.sizeDelta = new Vector2(0, bgRect.sizeDelta.y);
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0);;
            profile.color = new Color(profile.color.r, profile.color.g, profile.color.b, 0);
            selector.rectTransform.sizeDelta = new Vector2(selectorDelta.x, 0);
            bgSelectedPos = background.rectTransform.anchoredPosition;
            pfSelectedPos = profile.rectTransform.anchoredPosition;
            bgRect.anchoredPosition = selector.rectTransform.anchoredPosition;
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
            targetYPos += position == 0 ? tod.GraphicHeight * 0.1f : 0;
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
            RectTransform bgRect = background.rectTransform;
            RectTransform pfRect = profile.rectTransform;
            RectTransform slRect = selector.rectTransform;

            while (true) {
                if (position < 0) {

                    if (Mathf.Approximately(bgRect.anchoredPosition.x, bgSelectedPos.x)) {
                        float offset = Vector2.Distance(bgRect.anchoredPosition, slRect.anchoredPosition);
                        pfRect.DOAnchorPos(new Vector2(pfRect.anchoredPosition.x - offset, pfRect.anchoredPosition.y), tod.SpawnDuration * (2 / 3));
                        bgRect.DOAnchorPos(slRect.anchoredPosition, tod.SpawnDuration * (2 / 3));
                        slRect.DOSizeDelta(new Vector2(selectorDelta.x, 0), tod.SpawnDuration * (2 / 3));
                    } profile.DOFade(0, tod.SpawnDuration * (2 / 3));
                    yield return new WaitForSeconds(tod.SpawnDuration * (2 / 3));
                    bgRect.DOSizeDelta(new Vector2(0, 95), tod.SpawnDuration);
                    yield return new WaitForSeconds(tod.SpawnDuration);
                    Destroy(gameObject);
                    break;
                } else {
                    bool selected = position == 0 && Mathf.Abs(rectTransform.anchoredPosition.y - targetYPos) < 15;

                    /// Relocate Portrait;
                    rectTransform.anchoredPosition = Vector2.SmoothDamp(rectTransform.anchoredPosition,
                                                                        new Vector2(rectTransform.anchoredPosition.x, targetYPos),
                                                                        ref reloVelocity, tod.SpawnDuration * 2);

                    /// Expand Portrait;
                    bgRect.sizeDelta = Vector2.SmoothDamp(bgRect.sizeDelta, new Vector2(bgExtendedWidth, 95), ref selectVelocity, tod.SpawnDuration * 1.5f);
                    background.color = AlphaColor(background.color, Mathf.MoveTowards(background.color.a, baseAlpha, Time.deltaTime * 2));
                    profile.color = AlphaColor(profile.color, Mathf.MoveTowards(profile.color.a, 
                                                                                selected ? 1 : baseAlpha, Time.deltaTime * 2));
                    if (selected) {
                        selectLerp = Mathf.MoveTowards(selectLerp, 1, Time.deltaTime * 1.5f);

                        rectTransform.localScale = Vector2.Lerp(rectTransform.localScale, Vector2.one * selectScale, selectLerp);
                        pfRect.anchoredPosition = Vector2.Lerp(pfRect.anchoredPosition, pfSelectedPos, selectLerp);
                        bgRect.anchoredPosition = Vector2.Lerp(bgRect.anchoredPosition, bgSelectedPos, selectLerp);
                        slRect.sizeDelta = Vector2.Lerp(slRect.sizeDelta, selectorDelta, selectLerp);
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

        private Color AlphaColor(Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);

        /*
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
        }*/
    }
}