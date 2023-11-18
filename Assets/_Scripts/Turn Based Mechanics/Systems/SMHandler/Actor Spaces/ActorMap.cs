using System.Collections.Generic;
using UnityEngine;
using PseudoDataStructures;

public class ActorMap : ScriptableObject {

    [SerializeField] private PseudoDictionary<ActorData, GameObject> pseudoActorMap;

    private Dictionary<ActorData, GameObject> actorMap;
    public Dictionary<ActorData, GameObject> ActorPrefabMap {
        get {
            if (actorMap == null) actorMap = pseudoActorMap.ToDictionary();
            return actorMap;
        }
    }

    #if UNITY_EDITOR

    public Dictionary<ActorData, GameObject> PseudoActorMap {
        get {
            if (pseudoActorMap == null) pseudoActorMap = new PseudoDictionary<ActorData, GameObject>();
            return pseudoActorMap.ToDictionary();
        } set => UpdatePseudoActorMap(value);
    }

    public void UpdatePseudoActorMap(Dictionary<ActorData, GameObject> prefabMap) {
        PseudoDictionary<ActorData, GameObject> pseudoMap = null;
        if (prefabMap != null) pseudoMap = new PseudoDictionary<ActorData, GameObject>(prefabMap);
        pseudoActorMap = pseudoMap;
    }

    #endif
}
