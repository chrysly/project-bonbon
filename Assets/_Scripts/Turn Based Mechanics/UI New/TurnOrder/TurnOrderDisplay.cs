using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUI.TurnOrder {

    public class TurnOrderDisplay : ScreenSpaceElement {

        public event System.Action<bool> OnSoftToggle;
        [HideInInspector] public RectTransform rectTransform;

        [SerializeField] private GameObject portraitPrefab;
        [SerializeField] private float spawnDuration;
        public float SpawnDuration => spawnDuration;
        [SerializeField] private float graphicHeight;
        public float GraphicHeight => graphicHeight;

        private List<TurnPortrait> currOrder = new();
        private List<TurnPortrait> oldOrder = new();

        private class TurnPortrait {
            public Actor actor;
            public Portrait portrait;

            public TurnPortrait(Actor actor, Portrait portrait) {
                this.actor = actor;
                this.portrait = portrait;
            }
        }

        void Awake() {
            rectTransform = GetComponent<RectTransform>();
        }

        public override void Init(ScreenSpaceHandler handler) {
            this.handler = handler;
            handler.Input.OnTurnChange += UpdateTurnDisplay;
        }

        private void UpdateTurnDisplay(List<Actor> actorList) {
            oldOrder = currOrder;
            List<TurnPortrait> portraitList = new();
            for (int i = 0; i < actorList.Count; i++) {
                (int, TurnPortrait) ptt = FindPortrait(actorList[i]);
                if (ptt.Item1 < 0) {
                    portraitList.Add(SummonPortrait(i, actorList[i]));
                } else {
                    ptt.Item2.portrait.UpdatePos(i);
                    oldOrder.RemoveAt(ptt.Item1);
                    portraitList.Add(ptt.Item2);
                }
            }

            foreach (TurnPortrait tp in oldOrder) tp.portrait.UpdatePos(-1);
            currOrder = portraitList;
        }

        private TurnPortrait SummonPortrait(int index, Actor actor) {
            GameObject portraitGO = Instantiate(portraitPrefab, transform, false);
            portraitGO.GetComponent<RectTransform>().anchoredPosition = new Vector2(rectTransform.anchoredPosition.x,
                                                                                    rectTransform.anchoredPosition.y 
                                                                                    - graphicHeight * index - graphicHeight * 1.5f);
            Portrait portrait = portraitGO.GetComponent<Portrait>();
            portrait.Init(this);
            portrait.UpdateActor(actor);
            portrait.UpdatePos(index);
            return new TurnPortrait(actor, portrait);
        }

        private (int, TurnPortrait) FindPortrait(Actor actor) {
            for (int i = 1; i < oldOrder.Count; i++) {
                if (oldOrder[i].actor == actor) return (i, oldOrder[i]);
            } return (-1, null);
        }
    }
}