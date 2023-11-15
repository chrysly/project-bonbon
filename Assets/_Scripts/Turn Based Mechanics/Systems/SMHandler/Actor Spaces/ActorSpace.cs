using System.Collections.Generic;
using UnityEngine;

public abstract class ActorSpace : MonoBehaviour {

    [SerializeField] protected ActorHandler handler;
    protected Dictionary<ActorData, GameObject> prefabMap => handler.PrefabMap.ActorPrefabMap;

    [SerializeField] private ActorData initialActor;
    public ActorData CurrActor { get; private set; }
    protected GameObject actorPrefab;

    void Awake() {
        if (initialActor != null && actorPrefab == null) SpawnActor(initialActor);
    }

    public void SpawnActor(ActorData actorData) {
        if (actorPrefab != null) throw new System.Exception("There was already an actor here;");
        CurrActor = actorData;
        Debug.Log(prefabMap[actorData]);
        actorPrefab = Instantiate(prefabMap[actorData], transform.position, transform.rotation, transform);
        handler.InitializePrefab(actorPrefab);
    }

    public void DespawnActor() {
        Destroy(actorPrefab);
        actorPrefab = null;
        CurrActor = null;
    }

    #if UNITY_EDITOR

    public void Initialize(ActorHandler handler) => this.handler = handler;
    public ActorData InitialActor { get => initialActor; set => initialActor = value; }
    public GameObject ActorPrefab => actorPrefab;

    public void EditorDespawnActor() {
        DestroyImmediate(actorPrefab);
        actorPrefab = null;
        CurrActor = null;
    }

    #endif
}