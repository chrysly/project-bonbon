using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUI.TurnOrder {

    public enum State { Idle, Cleanup, Relocate, Select }

    public class TurnOrderDisplay : ScreenSpaceElement {

        public event System.Action<State> OnPortraitUpdate;
        [HideInInspector] public RectTransform rectTransform;

        [SerializeField] private GameObject portraitPrefab;
        [SerializeField] private float spawnDuration;
        public float SpawnDuration => spawnDuration;
        [SerializeField] private float graphicHeight;
        public float GraphicHeight => graphicHeight;

        private List<TurnPortrait> currOrder = new();
        private List<TurnPortrait> oldOrder = new();

        private State state;

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
            StartCoroutine(CoreCoroutine());
        }

        private IEnumerator CoreCoroutine() {
            while (true) {
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
                        IEnumerator populateCoroutine = Select();
                        while (populateCoroutine.MoveNext()) {
                            yield return populateCoroutine.Current;
                        } break;
                } yield return null;
            }
        }

        public override void Init(ScreenSpaceHandler handler) {
            this.handler = handler;
            handler.Input.OnTurnChange += UpdateTurnDisplay;
        }

        private void UpdateTurnDisplay(List<Actor> actorList) {
            oldOrder = currOrder;
            List<TurnPortrait> portraitList = new();
            string content = "| ";
            foreach (Actor actor in actorList) content += actor.Data.DisplayName + " | ";
            Debug.LogError(content);
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
            state = State.Cleanup;
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

        private IEnumerator Cleanup() {
            OnPortraitUpdate?.Invoke(state);
            yield return new WaitForSeconds(spawnDuration);
            state = State.Relocate;
        }

        private IEnumerator Relocate() {
            OnPortraitUpdate?.Invoke(state);
            yield return new WaitForSeconds(spawnDuration);
            state = State.Select;
        }

        private IEnumerator Select() {
            OnPortraitUpdate?.Invoke(state);
            yield return new WaitForSeconds(spawnDuration * 2);
            state = State.Idle;
        }

        /*
        private List<Portrait> portraitList;
        private float portraitSpace;

        public struct Portrait {
            public Actor actor;
            public GameObject gameObject;

            public Portrait(Actor actor, GameObject gameObject) {
                this.actor = actor;
                this.gameObject = gameObject;
            }
        }

        void Awake() {
            oldQueue = new List<Actor>();x
            portraitList = new List<Portrait>();

            var portraitTransform = portraitPrefab.GetComponent<Image>().rectTransform;
            portraitSpace = portraitTransform.rect.height * .5f;
        }

        void Start() => battleStateMachine.CurrInput.OnTurnChange += TurnOrderDisplay_OnTurnChange;

        void TurnOrderDisplay_OnTurnChange(List<Actor> queue) {
            currQueue = queue;
            string content = "| ";
            foreach (Actor actor in currQueue) content += actor.Data.DisplayName + " | ";
            Debug.LogWarning(content);
            if (!CompareActorList(currQueue, oldQueue)) UpdateQueue();
        }

        private void UpdateQueue() {
            StopAllCoroutines();
            if (oldQueue.Count < currQueue.Count) oldQueue = ResizeQueue(oldQueue, currQueue);
            List<Portrait> oldPortraitList = new List<Portrait>(portraitList);
            for (int i = 0; i < currQueue.Count; i++) {
                Vector2 destination = new Vector2(transform.position.x,
                                                    transform.position.y - i * portraitSpace);

                GameObject portrait = FindPortrait(currQueue[i], oldPortraitList);
                if (portrait) {
                    MovePortrait(portrait, destination);
                    RemovePortrait(oldPortraitList, portrait);
                } else SpawnPortrait(currQueue[i], destination);
            } for (int i = 0; i < oldPortraitList.Count; i++) {
                RemovePortrait(oldPortraitList[i].gameObject);
            } oldQueue = currQueue;
        }

        /// Animations ///

        private void SpawnPortrait(Actor actor, Vector2 destination) {
            GameObject portraitGO = Instantiate(portraitPrefab, destination, transform.rotation, transform);
            portraitGO.GetComponent<Image>().sprite = actor.Data.Icon;
            portraitGO.transform.position = destination;
            Portrait portrait = new Portrait(actor, portraitGO);
            portraitList.Add(portrait);
            StartCoroutine(_SpawnPortrait(portraitGO));
        }

        private void RemovePortrait(GameObject portrait) {
            RemovePortrait(portraitList, portrait);
            StartCoroutine(_RemovePortrait(portrait.gameObject));
        }

        private void MovePortrait(GameObject portrait, Vector2 destination) => StartCoroutine(_MovePortrait(portrait, destination));

        private IEnumerator _SpawnPortrait(GameObject portrait) {
            portrait.transform.localScale = Vector2.zero;
            while ((Vector2) portrait.transform.localScale != Vector2.one) {
                portrait.transform.localScale = Vector2.MoveTowards(portrait.transform.localScale, Vector2.one, Time.deltaTime * 3);
                yield return null;
            }
        }

        private IEnumerator _RemovePortrait(GameObject portrait) {
            while ((Vector2) portrait.transform.localScale != Vector2.zero) {
                portrait.transform.localScale = Vector2.MoveTowards(portrait.transform.localScale, Vector2.zero, Time.deltaTime * 3);
                yield return null;
            } Destroy(portrait);
        }

        private IEnumerator _MovePortrait(GameObject portrait, Vector2 destination) {
            while ((Vector2) portrait.transform.position != destination) {
                portrait.transform.position = Vector2.MoveTowards(portrait.transform.position, destination, Time.deltaTime * 600);
                yield return null;
            }
        }

        /// Helpers ///

        private GameObject FindPortrait(Actor actor, List<Portrait> portraitList) {
            for (int i = 1; i < portraitList.Count; i++) {
                if (portraitList[i].actor == actor) return portraitList[i].gameObject;
            } return null;
        }

        private void RemovePortrait(List<Portrait> list, GameObject portraitGO) {
            foreach (Portrait portrait in list) {
                if (portrait.gameObject == portraitGO) {
                    list.Remove(portrait);
                    break;
                }
            }
        }

        private List<Actor> ResizeQueue(List<Actor> oldQueue, List<Actor> referenceQueue) {
            List<Actor> resList = new List<Actor>(new Actor[referenceQueue.Count]);
            for (int i = 0; i < oldQueue.Count; i++) {
                resList[i] = oldQueue[i];
            } return resList;
        }

        private bool CompareActorList(List<Actor> list1, List<Actor> list2) => list1.Count == list2.Count
                                                                               && Enumerable.Range(0, list1.Count).ToList().TrueForAll(i => list1[i].Equals(list2[i]));
        */
    }
}