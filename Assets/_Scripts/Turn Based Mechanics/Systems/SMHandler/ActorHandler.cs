using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;

public class ActorHandler : StateMachineHandler {

    [SerializeField] private ActorMap prefabMap;
    public ActorMap PrefabMap => prefabMap;

    /// <summary> A convenient way to pass down Screen Canvas references to Actors; </summary>
    [System.Serializable]
    public class SSCanvasRefs {
        public BattleUIStateMachine stateMachine;
        public BattleSkillWindow skillWindow;
        public IngredientSelectWindow ingredientWindow;

        public void SetupRefs(ref BattleUIStateMachine stateMachine, ref BattleSkillWindow skillWindow, 
                              ref IngredientSelectWindow ingredientWindow) {
            stateMachine = this.stateMachine;
            skillWindow = this.skillWindow;
            ingredientWindow = this.ingredientWindow;
        }
    } private SSCanvasRefs screenCanvasRefs;
    public SSCanvasRefs ScreenCanvasRefs => screenCanvasRefs;

    private ActorSpace[] characterSpaces;
    private ActorSpace[] enemySpaces;

    private Actor[] actorList;

    public void InitializePrefab(GameObject actorPrefab) {
        Actor actor = actorPrefab.GetComponentInChildren<Actor>(true);
        if (actor is CharacterActor) {
            UIAnimationHandler uHandler = actorPrefab.GetComponentInChildren<UIAnimationHandler>(true);
            ScreenCanvasRefs.SetupRefs(ref uHandler._stateMachine, ref uHandler.skillWindow, ref uHandler.ingredientWindow);
        } actor.InjectHandler(this);
    }

    #if UNITY_EDITOR

    public ActorSpace[] EditorCharacterSpaces {
        get {
            if (characterSpaces == null) characterSpaces = new ActorSpace[0];
            return characterSpaces;
        } set => characterSpaces = value;
    }
    public ActorSpace[] EditorEnemySpaces {
        get {
            if (enemySpaces == null) enemySpaces = new ActorSpace[0];
            return enemySpaces;
        } set => enemySpaces = value;
    }

    public void SetupCanvasReferences() {
        screenCanvasRefs.stateMachine = transform.parent.GetComponentInChildren<BattleUIStateMachine>(true);
        screenCanvasRefs.skillWindow = transform.parent.GetComponentInChildren<BattleSkillWindow>(true);
        screenCanvasRefs.ingredientWindow = transform.parent.GetComponentInChildren<IngredientSelectWindow>(true);
    }

    #endif
}

public class ActorSpace : MonoBehaviour {

    [SerializeField] private ActorHandler handler;
    private Dictionary<ActorData, GameObject> prefabMap => handler.PrefabMap.ActorPrefabMap;

    [SerializeField] private ActorData initialActor;
    public ActorData CurrActor { get; private set; }
    private GameObject actorPrefab;

    void Awake() {
        if (initialActor != null && actorPrefab == null) SpawnActor(initialActor);
    }

    public void SpawnActor(ActorData actorData) {
        if (actorPrefab != null) throw new System.Exception("There was already an actor here;");
        actorPrefab = Instantiate(prefabMap[actorData], transform.position, transform.rotation, transform);
        handler.InitializePrefab(actorPrefab);
    }

    public void DespawnActor() {
        Destroy(actorPrefab);
        actorPrefab = null;
    }

    #if UNITY_EDITOR

    public void Initialize(ActorHandler handler) => this.handler = handler;
    public void SetInitialActor(ActorData actorData) => initialActor = actorData;

    #endif
}

[CustomEditor(typeof(ActorHandler))]
public class ActorHandlerEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        ActorHandler handler = target as ActorHandler;

        if (handler.PrefabMap != null) {
            if (GUILayout.Button("Launch Actor Space Editor")) LaunchActorSpaceEditor(handler);
        } else {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Assign a Prefab Map to Continue;", MessageType.Warning);
            EditorGUILayout.Space();
        }
    }

    private void LaunchActorSpaceEditor(ActorHandler handler) => ActorSpaceEditor.Launch(handler);
}

public class ActorSpaceEditor : EditorWindow {

    private static ActorSpaceEditor currWindow;
    private static ActorHandler actorHandler;

    private Vector2 characterScroll;
    private Vector2 enemyScroll;
    private Vector2 mainScroll;

    private enum Mode {
        ASE,
        Config,
    } private Mode mode;

    public static void Launch(ActorHandler handler) {
        if (currWindow != null) currWindow.Close();
        actorHandler = handler;
        currWindow = GetWindow<ActorSpaceEditor>("Actor Space Editor");
    }

    void OnDisable() {
        actorHandler = null;
    }

    void OnGUI() {
        if (actorHandler == null) {
            EditorUtils.DrawScopeCenteredText("Assembly Reload: Please Relaunch the Tool;\n-Carlos;");
            return;
        }

        DrawTopBar();
        using (new EditorGUILayout.HorizontalScope()) {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(180))) {
                using (new EditorGUILayout.VerticalScope()) {
                    GUI.color = UIColors.DarkGreen;
                    using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                        GUI.color = UIColors.Green;
                        GUILayout.Label("Character Spaces", new GUIStyle(UIStyles.CenteredLabelBold) { contentOffset = new Vector2(0, 2) });
                        if (GUILayout.Button(EditorUtils.FetchIcon("d_Toolbar Plus"), EditorStyles.miniButtonMid, GUILayout.Width(40))) {

                        } GUI.color = Color.white;
                    } using (var characterScope = new EditorGUILayout.ScrollViewScope(characterScroll, UIStyles.WindowBox, 
                                                                                      GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true))) {
                        characterScroll = characterScope.scrollPosition;
                        if (actorHandler.EditorCharacterSpaces.Length == 0) EditorUtils.DrawScopeCenteredText("No Character Spaces;");
                        else DrawActorSpaces(actorHandler.EditorCharacterSpaces, UIColors.Green);
                    }
                } using (new EditorGUILayout.VerticalScope()) {
                    GUI.color = UIColors.DarkRed;
                    using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                        GUI.color = UIColors.Red;
                        GUILayout.Label("Enemy Spaces", new GUIStyle(UIStyles.CenteredLabelBold) { contentOffset = new Vector2(0, 2) });
                        if (GUILayout.Button(EditorUtils.FetchIcon("d_Toolbar Plus"), EditorStyles.miniButtonMid, GUILayout.Width(40))) {

                        } GUI.color = Color.white;
                    } using (var enemyScope = new EditorGUILayout.ScrollViewScope(enemyScroll, UIStyles.WindowBox,
                                                                                  GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true))) {
                        enemyScroll = enemyScope.scrollPosition;
                        if (actorHandler.EditorEnemySpaces.Length == 0) EditorUtils.DrawScopeCenteredText("No Enemy Spaces;");
                        else DrawActorSpaces(actorHandler.EditorEnemySpaces, UIColors.Red);
                    }
                }
            }
        }
    }

    private void DrawTopBar() {
        switch (mode) {
            case Mode.ASE:
                using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                    EditorGUILayout.LabelField("Actor Space Editor", UIStyles.CenteredLabelBold);
                    if (GUILayout.Button(EditorUtils.FetchIcon("_Popup"), EditorStyles.toolbarButton, GUILayout.Width(48))) {
                        mode = Mode.Config;
                    }
                } break;
            case Mode.Config:
                using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                    EditorGUILayout.LabelField("Handler Status & Configuration", UIStyles.CenteredLabelBold);
                    if (GUILayout.Button(EditorUtils.FetchIcon("CustomTool"), EditorStyles.toolbarButton, GUILayout.Width(48))) {
                        mode = Mode.ASE;
                    }
                } break;
        }
    }

    private void DrawActorSpaces(ActorSpace[] spaceArr, Color color) {
        foreach (ActorSpace space in spaceArr) {

        }
    }
}