using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;
using ASEUtilities;

public class ActorSpaceEditor : EditorWindow {

    private static ActorSpaceEditor currWindow;
    private static ActorHandler actorHandler;
    private Transform AnchorTransform { get => actorHandler.anchorTransform; 
                                        set => actorHandler.anchorTransform = value; }
    private Transform spatialReference;

    private class SpaceStatus {
        public bool validDataType = true;
        public bool prefabAssigned = true;
        public bool prefabStructureValid = true;
        public bool prewarmed = true;
        public bool ValidPrefab => prefabAssigned && prefabStructureValid;

        public void ProcessWarnings(List<SpaceWarning> warnings) {
            foreach (SpaceWarning warning in warnings) {
                switch (warning) {
                    case SpaceWarning.InvalidDataType:
                        validDataType = false;
                        break;
                    case SpaceWarning.MissingPrefabMapping:
                        prefabAssigned = false;
                        break;
                    case SpaceWarning.InvalidPrefabStructure:
                        prefabStructureValid = false;
                        break;
                    case SpaceWarning.NotPrewarmed:
                        prewarmed = false;
                        break;
                }
            }
        }
    } private SpaceStatus spaceStatus;
    private ActorSpace selectedSpace;

    private class HandlerStatus {
        public bool referencesValid = true;
        public bool spacesNominal = true;
        public GUIContent spaceStatus = new GUIContent(EditorUtils.FetchIcon("d__Help"), "Undefined...");

        public void ProcessWarnings(Dictionary<ActorSpace, List<SpaceWarning>> warningMap) {
            if (actorHandler.ScreenCanvasRefs.stateMachine == null
                || actorHandler.ScreenCanvasRefs.skillWindow == null
                || actorHandler.ScreenCanvasRefs.ingredientWindow == null) referencesValid = false;
            warningMap = warningMap.Where(kvp => kvp.Value.Count > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            if (warningMap.Count > 0) {
                spacesNominal = false;
                spaceStatus = new GUIContent("The following spaces have conflicts that must be resolved:", EditorUtils.FetchIcon("Error"));
                foreach (KeyValuePair<ActorSpace, List<SpaceWarning>> kvp in warningMap) {
                    string addendum = $"\n  • {kvp.Key.gameObject.name}:";
                    foreach (SpaceWarning warning in kvp.Value) {
                        addendum += $"\n    ⁃ {warning.ToString().ToCamelSpace()};";
                    } spaceStatus.text += addendum;
                }
            }
        }
    } private HandlerStatus handlerStatus;

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
        ASEUtils.CleanupHandlerStructure(ref actorHandler);
    }

    void OnFocus() {
        if (mode == Mode.ASE) RefreshSpaceStatus();
        else if (mode == Mode.Config) RefreshHandlerStatus();
    }

    void OnDisable() {
        actorHandler = null;
        selectedSpace = null;
        spatialReference = null;
    }

    private void SetSelectedSpace(ActorSpace space) {
        selectedSpace = space;
        spatialReference = null;
        SetMode(Mode.ASE);
    }

    private void SetMode(Mode mode) {
        this.mode = mode;
        if (mode == Mode.ASE) {
            RefreshSpaceStatus();
        } else {
            selectedSpace = null;
            RefreshHandlerStatus();
        }
    }

    private void RefreshSpaceStatus() {
        spaceStatus = new SpaceStatus();
        List<SpaceWarning> warnings = ASEUtils.VerifySpaceStatus(selectedSpace, actorHandler);
        spaceStatus.ProcessWarnings(warnings);
    }

    private void RefreshHandlerStatus() {
        handlerStatus = new HandlerStatus();
        Dictionary<ActorSpace, List<SpaceWarning>> warningMap = ASEUtils.VerifyHandlerStatus(actorHandler);
        handlerStatus.ProcessWarnings(warningMap);
    }

    private Transform CreatePrefabAnchor() {
        Transform anchorTransform = new GameObject("Characters").transform;
        anchorTransform.parent = actorHandler.transform.parent;
        return anchorTransform;
    }

    private T[] ExpandActorSpace<T>(T[] spaceArr) where T : ActorSpace {
        if (AnchorTransform == null) AnchorTransform = CreatePrefabAnchor();
        string prefix = ASEUtils.SpacePrefix(typeof(T));
        GameObject spaceGO = new GameObject($"{prefix} {spaceArr.Length + 1}");
        spaceGO.transform.parent = AnchorTransform;
        T[] resizedArr = new T[spaceArr.Length + 1];
        System.Array.Copy(spaceArr, resizedArr, spaceArr.Length);
        resizedArr[spaceArr.Length] = spaceGO.AddComponent<T>();
        resizedArr[spaceArr.Length].Initialize(actorHandler);
        return resizedArr;
    }

    private void RemoveActorSpace(ActorSpace actorSpace) {
        DestroyImmediate(actorSpace.gameObject);
        ASEUtils.CleanupHandlerStructure(ref actorHandler);
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
                            actorHandler.EditorCharacterSpaces = ExpandActorSpace(actorHandler.EditorCharacterSpaces);
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
                            actorHandler.EditorEnemySpaces = ExpandActorSpace(actorHandler.EditorEnemySpaces);
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

                    switch (mode) {
                        case Mode.ASE:
                            DrawMainSpace();
                            break;
                        case Mode.Config:
                            DrawConfigurationSpace();
                            break;
                    }
                }
            }
        }
    }

    private void DrawMainSpace() {
        if (selectedSpace == null) EditorUtils.DrawScopeCenteredText("Select a Space to Edit;");
        else {
            bool empty = selectedSpace.InitialActor == null;
            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                if (empty) {
                    GUILayout.FlexibleSpace();
                    using (new EditorGUILayout.VerticalScope()) {
                        GUILayout.FlexibleSpace();
                        using (new EditorGUILayout.HorizontalScope()) {
                            GUILayout.FlexibleSpace();
                            EditorUtils.WindowBoxLabel("This space will be empty at the beginning of the Battle Loop;", UIStyles.CenteredLabelBold);
                            GUILayout.FlexibleSpace();
                        } using (new EditorGUILayout.HorizontalScope()) {
                            GUILayout.FlexibleSpace();
                            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                                string buttonText1 = "Assign Initial Actor";
                                if (GUILayout.Button(buttonText1,
                                                     GUILayout.Width(EditorUtils.MeasureTextWidth(buttonText1,
                                                                     GUI.skin.font) * 2.2f))) {
                                    string filterType = selectedSpace is CharacterSpace ? nameof(CharacterData)
                                                        : selectedSpace is EnemySpace ? nameof(EnemyData) : nameof(ActorData);
                                    EditorUtils.ShowObjectPicker(selectedSpace.InitialActor, $"t:{filterType}");
                                } ActorData ope = EditorUtils.CatchOPEvent<ActorData>();
                                if (ope) {
                                    selectedSpace.InitialActor = ope;
                                    RefreshSpaceStatus();
                                } GUI.color = UIColors.Red;
                                string buttonText2 = "Remove Space";
                                if (GUILayout.Button(buttonText2,
                                                        GUILayout.Width(EditorUtils.MeasureTextWidth(buttonText2,
                                                                        GUI.skin.font) * 1.5f))) {
                                    RemoveActorSpace(selectedSpace);
                                } GUI.color = Color.white;
                            } GUILayout.FlexibleSpace();
                        } GUILayout.FlexibleSpace();
                    } GUILayout.FlexibleSpace();
                } else {
                    bool prewarmedPrefab = selectedSpace.ActorPrefab != null;
                    using (new EditorGUILayout.VerticalScope()) {
                        EditorUtils.WindowBoxLabel("Preview", GUILayout.Width(164));
                        if (!prewarmedPrefab) {
                            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                                GUILayout.FlexibleSpace();
                                using (new EditorGUILayout.VerticalScope(EditorStyles.numberField, GUILayout.Height(123))) {
                                    EditorUtils.DrawScopeCenteredText("Prefab Not \nPrewarmed;");
                                } GUILayout.FlexibleSpace();
                            }
                        } else {
                            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                                GUILayout.FlexibleSpace();
                                using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                                    GUILayout.Label(AssetPreview.GetAssetPreview(selectedSpace.ActorPrefab),
                                                    UIStyles.CenteredLabelBold, GUILayout.Height(113));
                                } GUILayout.FlexibleSpace();
                            }
                        }
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
                                GUI.enabled = !prewarmedPrefab && spaceStatus.ValidPrefab;
                                if (GUILayout.Button("Prewarm Prefab")) {
                                    selectedSpace.SpawnActorEditor(selectedSpace.InitialActor);
                                    RefreshSpaceStatus();
                                } GUI.color = UIColors.Blue;
                                GUI.enabled = prewarmedPrefab;
                                if (GUILayout.Button("Remove Prefab")) {
                                    selectedSpace.EditorDespawnActor();
                                    RefreshSpaceStatus();
                                } GUI.color = Color.white;
                                GUI.enabled = true;
                            } using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                                GUI.color = UIColors.Blue;
                                if (GUILayout.Button("Replace Data")) {
                                    string filterType = selectedSpace is CharacterSpace ? nameof(CharacterData)
                                                : selectedSpace is EnemySpace ? nameof(EnemyData) : nameof(ActorData);
                                    EditorUtils.ShowObjectPicker(selectedSpace.InitialActor, $"t:{filterType}");
                                } ActorData ope = EditorUtils.CatchOPEvent<ActorData>();
                                if (ope) {
                                    selectedSpace.InitialActor = ope;
                                    RefreshSpaceStatus();
                                } GUI.color = UIColors.Red;
                                if (GUILayout.Button("Remove Space")) {
                                    RemoveActorSpace(selectedSpace);
                                } GUI.color = Color.white;
                            }
                        }
                    }
                }
            } EditorUtils.WindowBoxLabel("Spatial Adjustment Subtools");
            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                GUILayout.Label("Reference:");
                spatialReference = EditorGUILayout.ObjectField(spatialReference, typeof(Transform), true) as Transform;
                if (GUILayout.Button("Adjust To Match")) {
                    selectedSpace.transform.position = spatialReference.transform.position;
                    selectedSpace.transform.rotation = spatialReference.transform.rotation;
                    spatialReference = null;
                }
            } EditorUtils.WindowBoxLabel("Space Assignment Status");
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                if (empty) {
                    GUILayout.FlexibleSpace();
                    using (new EditorGUILayout.HorizontalScope()) {
                        GUILayout.FlexibleSpace();
                        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                            EditorGUILayout.Space();
                            GUILayout.Label(" - Empty - ", UIStyles.CenteredLabelBold);
                            EditorGUILayout.Space();
                        } GUILayout.FlexibleSpace();
                    } GUILayout.FlexibleSpace();
                } else {
                    if (spaceStatus.validDataType) {
                        GUI.color = UIColors.Green;
                        EditorUtils.DrawCustomHelpBox("Valid Data Type;", EditorUtils.FetchIcon("Valid"));
                        GUI.color = Color.white;
                    } else {
                        GUI.color = UIColors.Red;
                        EditorUtils.DrawCustomHelpBox(ASEUtils.SpaceWarningText[SpaceWarning.InvalidDataType],
                                                    EditorUtils.FetchIcon("Error"));
                        GUI.color = Color.white;
                    } if (spaceStatus.prefabAssigned) {
                        GUI.color = UIColors.Green;
                        EditorUtils.DrawCustomHelpBox("Valid Prefab Mapping;", EditorUtils.FetchIcon("Valid"));
                        GUI.color = Color.white;
                    } else {
                        GUI.color = UIColors.Red;
                        using (new EditorGUILayout.HorizontalScope()) {
                            EditorUtils.DrawCustomHelpBox(ASEUtils.SpaceWarningText[SpaceWarning.MissingPrefabMapping],
                                                          EditorUtils.FetchIcon("Error"));
                            GUI.color = Color.white;
                            DrawFixPrefabButton();
                        }
                    } GUI.enabled = spaceStatus.prefabAssigned;
                    if (spaceStatus.prefabStructureValid) {
                        GUI.color = UIColors.Green;
                        EditorUtils.DrawCustomHelpBox("Valid Prefab Structure;", EditorUtils.FetchIcon("Valid")); ;
                        GUI.color = Color.white;
                    } else {
                        GUI.color = UIColors.Red;
                        using (new EditorGUILayout.HorizontalScope()) {
                            EditorUtils.DrawCustomHelpBox(ASEUtils.SpaceWarningText[SpaceWarning.InvalidPrefabStructure],
                                                        EditorUtils.FetchIcon("Error"));
                            GUI.color = Color.white;
                            DrawFixPrefabButton();
                        } GUI.color = Color.white;
                    } GUI.enabled = spaceStatus.prefabAssigned && spaceStatus.prefabStructureValid;
                    if (spaceStatus.prewarmed) {
                        GUI.color = UIColors.Green;
                        EditorUtils.DrawCustomHelpBox("Prefab Prewarmed;", EditorUtils.FetchIcon("Valid"));
                        GUI.color = Color.white;
                    } else {
                        GUI.color = UIColors.Yellow;
                        EditorUtils.DrawCustomHelpBox(ASEUtils.SpaceWarningText[SpaceWarning.NotPrewarmed],
                                                        EditorUtils.FetchIcon("Warning"));
                        GUI.color = Color.white;
                    }
                }
            }
        }
    }

    private void DrawConfigurationSpace() {
        EditorUtils.WindowBoxLabel("Handler References");
        using (var changeScope = new EditorGUI.ChangeCheckScope()) {
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                    actorHandler.ScreenCanvasRefs.stateMachine = EditorGUILayout.ObjectField(actorHandler.ScreenCanvasRefs.stateMachine,
                                                                                             typeof(BattleUIStateMachine), true) as BattleUIStateMachine;
                } using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                    actorHandler.ScreenCanvasRefs.skillWindow = EditorGUILayout.ObjectField(actorHandler.ScreenCanvasRefs.skillWindow,
                                                                                            typeof(BattleSkillWindow), true) as BattleSkillWindow;
                } using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                    actorHandler.ScreenCanvasRefs.ingredientWindow = EditorGUILayout.ObjectField(actorHandler.ScreenCanvasRefs.ingredientWindow,
                                                                                                 typeof(IngredientSelectWindow), true) as IngredientSelectWindow;
                } using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                    GUILayout.FlexibleSpace();
                    using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                        if (GUILayout.Button("Auto-Assign")) actorHandler.SetupCanvasReferences();
                    } GUILayout.FlexibleSpace();
                }
            } if (changeScope.changed) EditorUtility.SetDirty(actorHandler);
        }
        EditorUtils.WindowBoxLabel("Handler Status");
        using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
            if (handlerStatus.referencesValid) {
                GUI.color = UIColors.Green;
                EditorUtils.DrawCustomHelpBox("All required references assigned;", EditorUtils.FetchIcon("Valid"));
                GUI.color = Color.white;
            } else {
                GUI.color = UIColors.Red;
                EditorUtils.DrawCustomHelpBox("Missing required references for Actor Initialization;", EditorUtils.FetchIcon("Error"));
                GUI.color = Color.white;
            } if (handlerStatus.spacesNominal) {
                GUI.color = UIColors.Green;
                EditorUtils.DrawCustomHelpBox("All actor spaces nominal;", EditorUtils.FetchIcon("Valid"));
                GUI.color = Color.white;
            } else {
                GUI.color = UIColors.Red;
                EditorUtils.DrawCustomHelpBox(handlerStatus.spaceStatus.text,
                                              handlerStatus.spaceStatus.image);
                GUI.color = Color.white;
            }
        }
    }

    private void DrawTopBar() {
        switch (mode) {
            case Mode.ASE:
                using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                    EditorGUILayout.LabelField("Actor Space Editor", UIStyles.CenteredLabelBold);
                    if (GUILayout.Button(EditorUtils.FetchIcon("_Popup"), EditorStyles.toolbarButton, GUILayout.Width(48))) {
                        SetMode(Mode.Config);
                    }
                } break;
            case Mode.Config:
                using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                    EditorGUILayout.LabelField("Handler Status & Configuration", UIStyles.CenteredLabelBold);
                    if (GUILayout.Button(EditorUtils.FetchIcon("CustomTool"), EditorStyles.toolbarButton, GUILayout.Width(48))) {
                        SetMode(mode = Mode.ASE);
                    }
                } break;
        }
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
                    SetSelectedSpace(spaceArr[i]);
                } if (GUILayout.Button(EditorUtils.FetchIcon("d_Transform Icon"),
                                       GUILayout.Width(32), GUILayout.Height(20))) EditorGUIUtility.PingObject(spaceArr[i].transform);
            }
        }
    }

    private void DrawFixPrefabButton() {
        if (GUILayout.Button("Fix", EditorStyles.miniButton,
                             GUILayout.Width(64), GUILayout.Height(18))) {
            BonbonAssetManager.BAMGUI.ActorPrefabFix(selectedSpace.InitialActor);
        }
    }
}