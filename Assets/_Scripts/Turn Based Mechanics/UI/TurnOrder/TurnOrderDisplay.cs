using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderDisplay : MonoBehaviour {

    [SerializeField] private GameObject portraitPrefab;
    [SerializeField] private BattleStateMachine bsm;

    private List<Actor> turnQueue => bsm.CurrInput.TurnQueue;
    private List<Actor> currQueue;

    private Dictionary<Actor, GameObject> portraitMap;
    private float portraitSpace;

    private Dictionary<Actor, Coroutine> animMap;

    void Awake() {
        currQueue = new List<Actor>();
        portraitMap = new Dictionary<Actor, GameObject>();
        animMap = new Dictionary<Actor, Coroutine>();

        var portraitTransform = portraitPrefab.GetComponent<Image>().rectTransform;
        portraitSpace = portraitTransform.rect.width * 1.1f;
    }

    void Update() {
        if (turnQueue != null) {
            string content = "| ";
            foreach (Actor actor in turnQueue) content += actor.Data.DisplayName + " | ";
            //Debug.LogWarning(content);
            if (!CompareActorList(turnQueue, currQueue)) UpdateQueue();
        }
    }

    private void UpdateQueue() {
        IEnumerable<Actor> newActors = turnQueue.Where(actor => !currQueue.Contains(actor));
        IEnumerable<Actor> removedActors = currQueue.Where(actor => !turnQueue.Contains(actor));
        IEnumerable<Actor> survivingActors = turnQueue.Except(newActors);

        Dictionary<Actor, Vector2> posMap = MapActorPositions(turnQueue);
        float currQueueEndX = ComputeListEnd(currQueue);

        foreach (Actor actor in newActors) InterpolateActor(actor, posMap[actor], Anim.Add);
        foreach (Actor actor in removedActors) InterpolateActor(actor, posMap[actor], Anim.Remove);
        foreach (Actor actor in survivingActors) InterpolateActor(actor, posMap[actor], Anim.Move);

        currQueue = turnQueue;
    }

    private Dictionary<Actor, Vector2> MapActorPositions(List<Actor> to) {
        Dictionary<Actor, Vector2> posMap = new Dictionary<Actor, Vector2>();
        for (int i = 0; i < to.Count; i++) {
            posMap[to[i]] = new Vector2(transform.position.x + i * portraitSpace, transform.position.y);
        } return posMap;
    }

    private float ComputeListEnd(List<Actor> to) => transform.position.x + to.Count * portraitSpace;

    private bool CompareActorList(List<Actor> list1, List<Actor> list2) => list1.Count == list2.Count
                                                                           && Enumerable.Range(0, list1.Count).ToList().TrueForAll(i => list1[i].Equals(list2[i]));

    /// Animations ///
    
    private enum Anim { Add, Remove, Move }

    private void InterpolateActor(Actor actor, Vector2 destination, Anim animationType) {
        try {
            Coroutine coroutine = animMap[actor];
            StopCoroutine(coroutine);
        } catch (KeyNotFoundException) {
            /// It's ok;
        } finally {
            IEnumerator animSeq;
            switch (animationType) {
                case Anim.Add:
                    animSeq = _SpawnPortrait(actor, destination);
                    break;
                case Anim.Remove:
                    animSeq = _RemovePortrait(actor, destination);
                    break;
                default:
                    animSeq = _MovePortrait(actor, destination);
                    break;
            } Coroutine sequence = StartCoroutine(animSeq);
            animMap[actor] = sequence;
        }
    }

    private IEnumerator _SpawnPortrait(Actor actor, Vector2 destination) {
        GameObject portrait;
        try {
            portrait = portraitMap[actor];
            portrait.transform.position = destination;
        } catch (KeyNotFoundException) {
            portrait = Instantiate(portraitPrefab, destination, transform.rotation, transform);
            portrait.GetComponent<Image>().sprite = actor.Data.Portrait;
            portraitMap[actor] = portrait;
        } portrait.transform.localScale = Vector2.zero;
        while ((Vector2) portrait.transform.localScale != Vector2.one) {
            portrait.transform.localScale = Vector2.MoveTowards(portrait.transform.localScale, Vector2.one, Time.deltaTime * 3);
            yield return null;
        } animMap.Remove(actor);
    }

    private IEnumerator _RemovePortrait(Actor actor, Vector2 destination) {
        GameObject portrait;
        try {
            portrait = portraitMap[actor];
        } catch (KeyNotFoundException) {
            yield break;
        } while ((Vector2) portrait.transform.localScale != Vector2.zero) {
            portrait.transform.localScale = Vector2.MoveTowards(portrait.transform.localScale, Vector2.zero, Time.deltaTime * 3);
            yield return null;
        } Destroy(portrait);
        portraitMap.Remove(actor);
        animMap.Remove(actor);
    }

    private IEnumerator _MovePortrait(Actor actor, Vector2 destination) {
        GameObject portrait;
        try {
            portrait = portraitMap[actor];
        } catch (KeyNotFoundException) {
            yield break;
        } while ((Vector2) portrait.transform.position != destination) {
            portrait.transform.position = Vector2.MoveTowards(portrait.transform.position, destination, Time.deltaTime);
            yield return null;
        } animMap.Remove(actor);
    }
}
