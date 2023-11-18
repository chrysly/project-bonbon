using System.Collections.Generic;
using UnityEngine;

public abstract class ActorSpace : MonoBehaviour {

    [SerializeField] protected ActorHandler handler;
    protected Dictionary<ActorData, GameObject> prefabMap => handler.PrefabMap.ActorPrefabMap;

    [SerializeField] private ActorData initialActor;
    public Actor CurrActor { get; private set; }
    [SerializeField] protected GameObject actorPrefab;

    void Awake() {
        if (initialActor != null) {
            if (actorPrefab == null) {
                SpawnActor(initialActor);
            } else CurrActor = actorPrefab.GetComponentInChildren<Actor>(true);
        }
    }

    public void SpawnActor(ActorData actorData) {
        if (actorPrefab != null) throw new System.Exception("There was already an actor here;");
        actorPrefab = Instantiate(prefabMap[actorData], transform.position, transform.rotation, transform);
        CurrActor = actorPrefab.GetComponentInChildren<Actor>(true);
        handler.InitializePrefab(actorPrefab);
    }

    public void DespawnActor() {
        Destroy(actorPrefab);
        actorPrefab = null;
        CurrActor = null;
    }

    #if UNITY_EDITOR

    float offset = 1;

    public void OnDrawGizmosSelected() {
        
        offset = Mathf.Abs(Mathf.Sin((float) UnityEditor.EditorApplication.timeSinceStartup * 2f)) * 0.3f;

        UnityEditor.Handles.color = this is CharacterSpace ? Color.green : Color.red;
        UnityEditor.Handles.DrawSolidDisc(transform.position, transform.up,
                                         0.7f + offset);
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up,
                                         0.85f + offset * 1.5f);
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up,
                                         0.86f + offset * 1.5f);
        Color color = this is CharacterSpace ? new Color(0, 0.31f, 0.2f, 1) : new Color(0.3f, 0, 0, 1);
        GUIStyle style = new GUIStyle(CJUtils.UIStyles.CenteredLabelBold) { fontSize = 48 };
        style.normal.textColor = color;
        UnityEditor.Handles.Label(transform.position, gameObject.name.IsolatePathEnd(" "), style);
    }

    public void Initialize(ActorHandler handler) {
        this.handler = handler;
        UnityEditor.EditorUtility.SetDirty(this);
    }
    public ActorData InitialActor { get => initialActor; set { initialActor = value; UnityEditor.EditorUtility.SetDirty(this); } }
    public GameObject ActorPrefab => actorPrefab;

    public void SpawnActorEditor(ActorData actorData) {
        if (actorPrefab != null) throw new System.Exception("There was already an actor here;");
        actorPrefab = UnityEditor.PrefabUtility.InstantiatePrefab(handler.PrefabMap.PseudoActorMap[actorData]) as GameObject;
        actorPrefab.transform.position = transform.position;
        actorPrefab.transform.rotation = transform.rotation;
        actorPrefab.transform.parent = transform;
        handler.InitializePrefab(actorPrefab);
        UnityEditor.EditorUtility.SetDirty(this);
    }

    public void EditorDespawnActor() {
        DestroyImmediate(actorPrefab);
        actorPrefab = null;
        UnityEditor.EditorUtility.SetDirty(this);
    }

    #endif
}