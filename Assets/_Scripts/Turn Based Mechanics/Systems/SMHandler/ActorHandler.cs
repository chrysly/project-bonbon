using System.Linq;
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

    [HideInInspector]
    [SerializeField] private CharacterSpace[] characterSpaces;
    [HideInInspector]
    [SerializeField] private EnemySpace[] enemySpaces;

    private Actor[] actorList;

    public void InitializePrefab(GameObject actorPrefab) {
        Actor actor = actorPrefab.GetComponentInChildren<Actor>(true);
        if (actor is CharacterActor) {
            UIAnimationHandler uHandler = actorPrefab.GetComponentInChildren<UIAnimationHandler>(true);
            ScreenCanvasRefs.SetupRefs(ref uHandler._stateMachine, ref uHandler.skillWindow, ref uHandler.ingredientWindow);
        } actor.InjectHandler(this);
    }

    #if UNITY_EDITOR

    [HideInInspector] public Transform anchorTransform;

    public CharacterSpace[] EditorCharacterSpaces {
        get {
            if (characterSpaces == null) characterSpaces = new CharacterSpace[0];
            return characterSpaces;
        } set => characterSpaces = value;
    }
    public EnemySpace[] EditorEnemySpaces {
        get {
            if (enemySpaces == null) enemySpaces = new EnemySpace[0];
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
    private Transform AnchorTransform { get => actorHandler.anchorTransform; 
                                        set => actorHandler.anchorTransform = value; }

    private ActorSpace selectedSpace;

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

    void OnEnable() {
        if (actorHandler == null) return;
        VerifyHandlerStatus();
    }

    private enum SpaceWarning { InvalidSpaces }

    private void VerifyHandlerStatus() {
        List<SpaceWarning> warnings = new List<SpaceWarning>();

        actorHandler.EditorCharacterSpaces = CleanupSpaceStructure(actorHandler.EditorCharacterSpaces, out bool dirtyCSpaces);
        actorHandler.EditorEnemySpaces = CleanupSpaceStructure(actorHandler.EditorEnemySpaces, out bool dirtyESpaces);
        if (dirtyCSpaces || dirtyESpaces) {
            FixSpaceNotation(actorHandler.EditorCharacterSpaces);
            FixSpaceNotation(actorHandler.EditorEnemySpaces);
            warnings.Add(SpaceWarning.InvalidSpaces);
        }
    }

    /// <summary>
    /// Fix the notation of Actor Space game objects; <br></br>
    /// Warning! The space structure must be free of invalid entries;
    /// </summary>
    /// <param name="spaces"> Actor Space array to fix the notation for; </param>
    private void FixSpaceNotation<T>(T[] spaces) where T : ActorSpace {
        for (int i = 0; i < spaces.Length; i++) {
            spaces[i].gameObject.name = $"{SpacePrefix(typeof(T))} {i + 1}";
        }
    }

    private string SpacePrefix(System.Type spaceType) => spaceType == typeof(CharacterSpace) ? "Character"
                                                       : spaceType == typeof(EnemySpace) ? "Enemy" : "Undefined";

    private T[] CleanupSpaceStructure<T>(T[] spaces, out bool dirty) where T : ActorSpace {
        T[] cleanSpaceArr = spaces.Where(space => space != null).ToArray();
        dirty = cleanSpaceArr.Length != spaces.Length;
        return cleanSpaceArr;
    }

    void OnDisable() {
        actorHandler = null;
    }

    void OnGUI() {
        if (actorHandler == null) {
            EditorUtils.DrawScopeCenteredText("Assembly Reload: Please Relaunch the Tool;\n-Carlos;");
            return;
        }

        using (new EditorGUILayout.HorizontalScope()) {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(210))) {
                using (new EditorGUILayout.VerticalScope()) {
                    GUI.color = UIColors.DarkGreen;
                    using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                        GUI.color = UIColors.Green;
                        GUILayout.Label("Character Spaces", new GUIStyle(UIStyles.CenteredLabelBold) { contentOffset = new Vector2(0, 2) });
                        if (GUILayout.Button(EditorUtils.FetchIcon("d_Toolbar Plus"), EditorStyles.miniButtonMid, GUILayout.Width(40))) {
                            actorHandler.EditorCharacterSpaces = ExpandActorSpace(actorHandler.EditorCharacterSpaces, typeof(CharacterActor));
                            SceneUtils.SortChildren(AnchorTransform);
                        } GUI.color = Color.white;
                    } using (var characterScope = new EditorGUILayout.ScrollViewScope(characterScroll, UIStyles.WindowBox, 
                                                                                      GUILayout.ExpandWidth(true), GUILayout.MaxHeight(position.height))) {
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
                            actorHandler.EditorEnemySpaces = ExpandActorSpace(actorHandler.EditorEnemySpaces, typeof(EnemyActor));
                            SceneUtils.SortChildren(AnchorTransform);
                        } GUI.color = Color.white;
                    } using (var enemyScope = new EditorGUILayout.ScrollViewScope(enemyScroll, UIStyles.WindowBox,
                                                                                  GUILayout.ExpandWidth(true), GUILayout.MaxHeight(position.height))) {
                        enemyScroll = enemyScope.scrollPosition;
                        if (actorHandler.EditorEnemySpaces.Length == 0) EditorUtils.DrawScopeCenteredText("No Enemy Spaces;");
                        else DrawActorSpaces(actorHandler.EditorEnemySpaces, UIColors.Red);
                    }
                }
            } GUILayout.Label("", UIStyles.WindowBox, GUILayout.Width(12), GUILayout.ExpandHeight(true));
            using (new EditorGUILayout.VerticalScope()) {
                DrawTopBar();
                using (var mainScope = new EditorGUILayout.ScrollViewScope(mainScroll)) {
                    mainScroll = mainScope.scrollPosition;

                    if (selectedSpace == null) EditorUtils.DrawScopeCenteredText("Select a Space to Edit;");
                    else {
                        bool empty = selectedSpace.InitialActor == null;
                        using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                            if (empty) {
                                GUILayout.FlexibleSpace();
                                using (new EditorGUILayout.VerticalScope()) {
                                    GUILayout.FlexibleSpace();
                                    GUILayout.Label("This space will be empty at the beginning of the Battle Loop;");
                                    if (GUILayout.Button("Assign Initial Actor")) {
                                        string filterType = selectedSpace is CharacterSpace ? nameof(CharacterData)
                                                          : selectedSpace is EnemySpace ? nameof(EnemyData) : nameof(ActorData);
                                        EditorUtils.ShowObjectPicker(selectedSpace.InitialActor, $"t:{filterType}");
                                    } ActorData ope = CatchOPEvent<ActorData>();
                                    if (ope) selectedSpace.InitialActor = ope;
                                    GUILayout.FlexibleSpace();
                                } GUILayout.FlexibleSpace();
                            } else {
                                bool prewarmedPrefab = selectedSpace.ActorPrefab != null;
                                using (new EditorGUILayout.VerticalScope()) {
                                    EditorUtils.WindowBoxLabel("Preview", GUILayout.Width(164));
                                    if (prewarmedPrefab) {
                                        using (new EditorGUILayout.VerticalScope(EditorStyles.numberField, GUILayout.Width(128), GUILayout.Height(128))) {
                                            EditorUtils.DrawScopeCenteredText("Prefab Not \nPrewarmed;");
                                        }
                                    } else GUILayout.Label(AssetPreview.GetAssetPreview(selectedSpace.ActorPrefab));
                                } using (new EditorGUILayout.VerticalScope(GUILayout.MaxWidth(position.width))) {
                                    EditorUtils.WindowBoxLabel("Actor Data");
                                    using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                                        GUIContent content = EditorGUIUtility.ObjectContent(selectedSpace.InitialActor, typeof(ActorData));
                                        if (GUILayout.Button(content, EditorStyles.objectField, GUILayout.Height(48))) {
                                            EditorGUIUtility.PingObject(selectedSpace.InitialActor);
                                        } GUI.color = UIColors.Red;
                                        if (GUILayout.Button(EditorUtils.FetchIcon("winbtn_win_close"), GUILayout.Width(28), GUILayout.Height(48))) {
                                            selectedSpace.InitialActor = null;
                                            if (selectedSpace.ActorPrefab) selectedSpace.EditorDespawnActor();
                                        } GUI.color = Color.white;
                                    } EditorUtils.WindowBoxLabel("Actions");
                                    using (new EditorGUILayout.HorizontalScope()) {
                                        using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                                            GUI.color = UIColors.Green;
                                            GUI.enabled = !prewarmedPrefab;
                                            if (GUILayout.Button("Prewarm Prefab")) {
                                                selectedSpace.SpawnActor(selectedSpace.InitialActor);
                                            } GUI.color = UIColors.Blue;
                                            GUI.enabled = prewarmedPrefab;
                                            if (GUILayout.Button("Remove Prefab")) {
                                                selectedSpace.EditorDespawnActor();
                                            } GUI.color = Color.white;
                                            GUI.enabled = true;
                                        } using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                                            GUI.color = UIColors.Blue;
                                            if (GUILayout.Button("Replace Data")) {
                                                string filterType = selectedSpace is CharacterSpace ? nameof(CharacterData)
                                                          : selectedSpace is EnemySpace ? nameof(EnemyData) : nameof(ActorData);
                                                EditorUtils.ShowObjectPicker(selectedSpace.InitialActor, $"t:{filterType}");
                                            } ActorData ope = CatchOPEvent<ActorData>();
                                            if (ope) selectedSpace.InitialActor = ope;
                                            GUI.color = UIColors.Red;
                                            if (GUILayout.Button("Remove Space")) {
                                                ;
                                            } GUI.color = Color.white;
                                        }
                                    }
                                }
                            }
                        } EditorUtils.WindowBoxLabel("Space Assignment Status");
                        using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                            if (empty) EditorUtils.DrawScopeCenteredText("- Empty -");
                            else {

                            }
                        }
                    }
                }
            }
        }
    }

    private T CatchOPEvent<T>() where T : Object {
        if (Event.current.commandName == "ObjectSelectorUpdated") {
            var actorData = EditorGUIUtility.GetObjectPickerObject();
            if (actorData is T) return actorData as T;
        } return null;
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

    private Transform CreatePrefabAnchor() {
        Transform anchorTransform = new GameObject("Characters").transform;
        anchorTransform.parent = actorHandler.transform.parent;
        return anchorTransform;
    }

    private T[] ExpandActorSpace<T>(T[] spaceArr, System.Type spaceType) where T : ActorSpace {
        if (AnchorTransform == null) AnchorTransform = CreatePrefabAnchor();
        string prefix = SpacePrefix(typeof(T));
        GameObject spaceGO = new GameObject($"{prefix} {spaceArr.Length + 1}");
        spaceGO.transform.parent = AnchorTransform;
        T[] resizedArr = new T[spaceArr.Length + 1];
        System.Array.Copy(spaceArr, resizedArr, spaceArr.Length);
        resizedArr[spaceArr.Length] = spaceGO.AddComponent<T>();
        resizedArr[spaceArr.Length].Initialize(actorHandler);
        return resizedArr;
    }

    private void DrawActorSpaces(ActorSpace[] spaceArr, Color color) {
        for (int i = 0; i < spaceArr.Length; i++) {
            GUI.color = color;
            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                GUI.color = Color.white;
                using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox, GUILayout.Width(28), GUILayout.Height(14))) {
                    GUILayout.Label((i + 1).ToString(), UIStyles.CenteredLabelBold, GUILayout.Height(10));
                } string initialActor = spaceArr[i].InitialActor == null ? "Empty" : spaceArr[i].InitialActor.DisplayName;
                if (GUILayout.Button(initialActor, selectedSpace == spaceArr[i]
                                                   ? new GUIStyle(EditorStyles.numberField) { alignment = TextAnchor.MiddleCenter, normal = { textColor = UIColors.Blue } }
                                                   : GUI.skin.button, GUILayout.Height(20))) {
                    selectedSpace = spaceArr[i];
                } if (GUILayout.Button(EditorUtils.FetchIcon("d_Transform Icon"),
                                       GUILayout.Width(32), GUILayout.Height(20))) EditorGUIUtility.PingObject(spaceArr[i].transform);
            }
        }
    }
}