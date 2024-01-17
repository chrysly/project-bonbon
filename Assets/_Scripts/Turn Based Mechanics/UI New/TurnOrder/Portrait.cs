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
        private Vector3 baseScale;

        private float selectLerp;
        private Vector2 reloVelocity;
        private Vector2 selectVelocity;

        private Vector2 bgSelectedPos;
        private Vector2 pfSelectedPos;
        private Vector2 selectorDelta;

        private bool softEnabled = true;

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
            bgRect.anchoredPosition = new Vector2(selector.rectTransform.anchoredPosition.x - 6,
                                                  selector.rectTransform.anchoredPosition.y);
            baseScale = rectTransform.localScale;
        }

        public void Init(TurnOrderDisplay tod) {
            this.tod = tod;
            StartCoroutine(CoreCoroutine());
            tod.OnSoftToggle += (toggle) => softEnabled = toggle;
        }

        public void UpdateActor(Actor actor) => profile.sprite = actor.Data.Icon;
        public void UpdatePos(int position) {
            this.position = position;
            targetYPos = tod.rectTransform.anchoredPosition.y - tod.GraphicHeight * position;
            targetYPos += position == 0 ? tod.GraphicHeight * 0.1f : 0;
        }

        private IEnumerator CoreCoroutine() {
            RectTransform bgRect = background.rectTransform;
            RectTransform pfRect = profile.rectTransform;
            RectTransform slRect = selector.rectTransform;

            while (true) {
                if (position < 0) {

                    if (Mathf.Approximately(bgRect.anchoredPosition.x, bgSelectedPos.x)) {
                        float offset = Vector2.Distance(bgRect.anchoredPosition, slRect.anchoredPosition);
                        rectTransform.DOScale(baseScale, tod.SpawnDuration * (2/3));
                        pfRect.DOAnchorPos(new Vector2(pfRect.anchoredPosition.x - offset, pfRect.anchoredPosition.y), tod.SpawnDuration * (2/3));
                        bgRect.DOAnchorPos(slRect.anchoredPosition, tod.SpawnDuration * (2/3));
                        slRect.DOSizeDelta(new Vector2(selectorDelta.x, 0), tod.SpawnDuration * (2/3));
                    } profile.DOFade(0, tod.SpawnDuration * (2/3));
                    yield return new WaitForSeconds(tod.SpawnDuration * (2/3));
                    bgRect.DOSizeDelta(new Vector2(0, 95), tod.SpawnDuration * 1.2f);
                    yield return new WaitForSeconds(tod.SpawnDuration * 1.2f);
                    Destroy(gameObject);
                    break;
                } else {
                    bool selected = position == 0 && Mathf.Abs(rectTransform.anchoredPosition.y - targetYPos) < 15;

                    /// Relocate Portrait;
                    rectTransform.anchoredPosition = Vector2.SmoothDamp(rectTransform.anchoredPosition,
                                                                        new Vector2(rectTransform.anchoredPosition.x, targetYPos),
                                                                        ref reloVelocity, tod.SpawnDuration * 2);

                    /// Expand Portrait;
                    bgRect.sizeDelta = Vector2.SmoothDamp(bgRect.sizeDelta, new Vector2(softEnabled ? bgExtendedWidth : 0, 95),
                                                                            ref selectVelocity, tod.SpawnDuration * 1.5f);
                    background.color = AlphaColor(background.color, Mathf.MoveTowards(background.color.a,
                                                                                      softEnabled ? (selected ? 1 : baseAlpha) : 0, 
                                                                                      Time.deltaTime * 2));
                    profile.color = AlphaColor(profile.color, Mathf.MoveTowards(profile.color.a, 
                                                                                softEnabled ? (selected ? 1 : baseAlpha) : 0, 
                                                                                Time.deltaTime * 2));
                    if (selected) {
                        selectLerp = Mathf.MoveTowards(selectLerp, softEnabled ? 1 : 0, Time.deltaTime * 1.5f);

                        rectTransform.localScale = Vector2.Lerp(rectTransform.localScale, Vector2.one * selectScale, selectLerp);
                        pfRect.anchoredPosition = Vector2.Lerp(pfRect.anchoredPosition, pfSelectedPos, selectLerp);
                        bgRect.anchoredPosition = Vector2.Lerp(bgRect.anchoredPosition, bgSelectedPos, selectLerp);
                        slRect.sizeDelta = Vector2.Lerp(slRect.sizeDelta, selectorDelta, selectLerp);
                    }
                } yield return null;
            }
        }

        private Color AlphaColor(Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);
    }
}