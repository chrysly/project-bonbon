using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ActorMovementHandler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform actorContainer;
    [SerializeField] private int maxActionTurns = 6;
    [SerializeField] private float delayBetweenWaypoints = 0.5f;
    private CharacterActor[] actorList;

    Sequence pathSequence = DOTween.Sequence();

    void Start()
    {
        actorList = new CharacterActor[actorContainer.childCount];
        ExtractActors();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ExtractActors() {
        for (int i = 0; i < actorContainer.childCount; i++) {
            actorList[i] = actorContainer.GetChild(i).GetComponent<CharacterActor>();
        }
    } 

    public void RunAllMoveSequences() {
        foreach (CharacterActor actor in actorList) {
        }
    }

    public float getDelay() {
        return delayBetweenWaypoints * maxActionTurns;
    }
}
