using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderDisplay : MonoBehaviour {

    [SerializeField] private GameObject portraitPrefab;
    private BattleStateMachine battleStateMachine => BattleStateMachine.Instance;

    private List<Actor> currQueue;
    private List<Actor> oldQueue;

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
        oldQueue = new List<Actor>();
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
}