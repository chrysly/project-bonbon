//using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;
using ASEUtilities;
using PseudoDataStructures;

/// <summary>
/// Because the structure I devised for the MADGUI has been quite comfortable to work with, I'm building 
/// this tool using a similar structure. This tool has different needs and less complexity than the MAD,
/// thus I'm creating a new tool altogether rather than inheriting from the former's abstract base;
/// </summary>
namespace BonbonAssetManager {
    public abstract class BonBaseTool : ScriptableObject {

        protected BAMGUI MainGUI;
        protected Vector2 mainScroll;

        public string SelectedPath { get; protected set; }

        public static T CreateTool<T>(BAMGUI mainGUI) where T : BonBaseTool {
            T tool = CreateInstance<T>();
            tool.MainGUI = mainGUI;
            return tool;
        }

        public abstract void Initialize();

        public virtual void ShowGUI() { }
    }

    public class BonbonManager : BonBaseTool {

        private BaseHierarchy<BonbonBlueprint> bonbonHierarchy;
        private BonbonBlueprint selectedBonbon;
        private List<BonbonBlueprint> bonbonList;
        private AssetCreator<BonbonBlueprint> assetCreator;
        private Vector2[] scrollGroup;
        private BonbonMap bonbonMapSO;
        private List<BonbonBlueprint>[] globalBonbonMap;

        private System.Type[] actionTypes;
        private ImmediateAction[] foundActions;
        private Editor bonbonInspector;

        private enum Mode {
            Attributes,
            Recipe,
            GlobalMap,
        } private Mode mode;

        public override void Initialize() {
            assetCreator = new AssetCreator<BonbonBlueprint>(MainGUI.assetPaths[(int) BAMGUI.ToolType.BonbonManager]);
            bonbonHierarchy = new BaseHierarchy<BonbonBlueprint>(this);
            bonbonHierarchy.OnPathSelection += BonbonManager_OnPathSelection;
            assetCreator.OnAssetCreation += bonbonHierarchy.ReloadHierarchy;

            UpdateBonbonList();
            LoadBonbonMap();
            actionTypes = ActionUtils.FetchAssemblyChildren(new System.Type[] { typeof(ImmediateAction.SkillOnly) } );
        }

        void OnDisable() {
            DestroyImmediate(bonbonInspector);
        }

        private int buttonSize = 50;

        private Vector2 bonbonScroll;
        private Vector2 recipeScroll;
        private Vector2 previewScroll;

        public void SetSelectedBonbon(BonbonBlueprint bonbon) {
            selectedBonbon = bonbon;
            UpdateBonbonList();
            if (selectedBonbon.augmentData.immediateActions == null) selectedBonbon.augmentData.immediateActions = new List<ImmediateAction.SkillOnly>();
            if (selectedBonbon.augmentData.augmentEffects == null) selectedBonbon.augmentData.augmentEffects = new List<EffectBlueprint>();
            foundActions = ActionUtils.FetchAvailableActions(selectedBonbon.augmentData.immediateActions, actionTypes);
        }

        private void LoadBonbonMap() {
            var guid = AssetDatabase.FindAssets($"t:{nameof(BonbonMap)}")[0];
            bonbonMapSO = AssetDatabase.LoadAssetAtPath<BonbonMap>(AssetDatabase.GUIDToAssetPath(guid));
            bonbonMapSO.bonbonMap = BAMUtils.VerifyMapSize(bonbonMapSO.bonbonMap);
            globalBonbonMap = bonbonMapSO.bonbonMap.ToListArray();
        }

        private void UpdateBonbonList() {
            bonbonList = new List<BonbonBlueprint>(MainGUI.GlobalBonbonList);
            List<BonbonBlueprint> invalidBonbons = new List<BonbonBlueprint>();
            foreach (BonbonBlueprint bonbon in bonbonList) {
                bonbon.UpdateRecipeSize();

                if (bonbon == selectedBonbon) {
                    invalidBonbons.Add(bonbon);
                    continue;
                }

                FindInvalidBonbons(invalidBonbons, bonbon);
            } foreach (BonbonBlueprint bonbon in invalidBonbons) bonbonList.Remove(bonbon);
        }

        private bool FindInvalidBonbons(List<BonbonBlueprint> invalidBonbons, BonbonBlueprint bonbon) {
            if (bonbon == null) return false;
            bool bonbonInList = invalidBonbons.Contains(bonbon);
            foreach (BonbonBlueprint ingredient in bonbon.recipe) {
                if (ingredient == selectedBonbon) {
                    if (!bonbonInList) invalidBonbons.Add(bonbon);
                    return true;
                } else if (FindInvalidBonbons(invalidBonbons, ingredient)) {
                    if (!bonbonInList) invalidBonbons.Add(bonbon);
                    return true;
                }
            } return false;
        }

        public override void ShowGUI() {
            using (new EditorGUILayout.HorizontalScope()) {
                using (new EditorGUILayout.VerticalScope()) {
                    if (mode == Mode.GlobalMap) GUI.enabled = false;
                    bonbonHierarchy.ShowGUI();
                    assetCreator.ShowCreator();
                    GUI.enabled = true;
                }
                    
                using (new EditorGUILayout.VerticalScope()) {
                    MainGUI.DrawToolbar();
                    using (var scope = new EditorGUILayout.ScrollViewScope(mainScroll)) {
                        mainScroll = scope.scrollPosition;

                        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                            GUILayout.Label("Mode:", GUILayout.Width(50));
                            if (GUILayout.Button("Attributes", mode == Mode.Attributes
                                                           ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton,
                                                           GUILayout.ExpandWidth(true))) {
                                mode = Mode.Attributes;
                            } if (GUILayout.Button("Recipe", mode == Mode.Recipe
                                                            ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton,
                                                            GUILayout.ExpandWidth(true))) {
                                mode = Mode.Recipe;
                            } if (GUILayout.Button("Global Map", mode == Mode.GlobalMap
                                                           ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton,
                                                           GUILayout.ExpandWidth(true))) {
                                mode = Mode.GlobalMap;
                            }
                        }

                        if (selectedBonbon != null || mode == Mode.GlobalMap) {
                            switch (mode) {
                                case Mode.Attributes:
                                    if (bonbonInspector is null) bonbonInspector = Editor.CreateEditor(selectedBonbon);
                                    bonbonInspector.OnInspectorGUI();

                                    EditorGUILayout.Separator();
                                    EditorUtils.DrawSeparatorLines(" Use Immediate Actions");
                                    EditorGUILayout.Separator();

                                    if (ActionUtils.DrawAvailableActions(ref selectedBonbon.augmentData.immediateActions,
                                                                         ref foundActions, actionTypes)) EditorUtility.SetDirty(selectedBonbon);

                                    break;
                                case Mode.Recipe:
                                    BAMUtils.DrawAssetGroup(bonbonList, bonbonScroll, BonbonBlueprint.GUIContent,
                                                            MainGUI.position, buttonSize);
                                    DrawRecipeDropSlots();
                                    DrawRecipePreview(selectedBonbon);
                                    break;
                                case Mode.GlobalMap:
                                    BAMUtils.DrawAssetGroup(MainGUI.GlobalBonbonList, bonbonScroll, BonbonBlueprint.GUIContent,
                                                            MainGUI.position, buttonSize);
                                    BAMUtils.DrawMap(globalBonbonMap, ref scrollGroup, BonbonBlueprint.GUIContent, buttonSize, 
                                                     MainGUI.assetRefs.dndFieldAssets, SaveMap);
                                    break;
                            }
                        } else {
                            EditorUtils.DrawScopeCenteredText("Select a Bonbon to edit it here;");
                        }
                    }
                }
            }
        }

        private void BonbonManager_OnPathSelection(string path) {
            SelectedPath = path;
            DestroyImmediate(bonbonInspector);
            bonbonInspector = null;
            SetSelectedBonbon(AssetDatabase.LoadAssetAtPath<BonbonBlueprint>(path));
        }

        private void DrawRecipeDropSlots() {
            EditorUtils.WindowBoxLabel("Recipe");
            GUI.enabled = false;
            using (var scope = new EditorGUILayout.ScrollViewScope(recipeScroll, EditorStyles.textField, 
                                                                   GUILayout.ExpandHeight(false), GUILayout.Height(buttonSize * 2.5f))) {
                GUI.enabled = true;
                recipeScroll = scope.scrollPosition;

                using (new EditorGUILayout.HorizontalScope()) {
                    BonbonBlueprint acceptedBonbon = BAMUtils.DrawDragAcceptButton<BonbonBlueprint>(FieldUtils.DnDFieldType.Add,
                                                                                                    MainGUI.assetRefs.dndFieldAssets,
                                                                                                    GUILayout.Width(buttonSize * 2),
                                                                                                    GUILayout.Height(buttonSize * 2));
                    if (acceptedBonbon != null) UpdateBonbonRecipe(selectedBonbon, acceptedBonbon);

                    using (new EditorGUILayout.VerticalScope()) {
                        using (new EditorGUILayout.HorizontalScope()) {
                            foreach (BonbonBlueprint bonbon in selectedBonbon.recipe) {
                                if (bonbon != null) BAMUtils.DrawAssetDragButton(bonbon, BonbonBlueprint.GUIContent, buttonSize);
                                else DrawEmptyBox();
                            }
                        } BonbonBlueprint removeObject = BAMUtils.DrawDragAcceptButton<BonbonBlueprint>(FieldUtils.DnDFieldType.Remove,
                                                                                                        MainGUI.assetRefs.dndFieldAssets,
                                                                                                        GUILayout.Width(buttonSize * 4.4f),
                                                                                                        GUILayout.Height(buttonSize * 0.8f));
                        if (removeObject != null) RemoveFromRecipe(selectedBonbon, removeObject);
                    }
                } 
            }
        }

        private void UpdateBonbonRecipe(BonbonBlueprint recipeBonbon, BonbonBlueprint ingredientBonbon) {
            recipeBonbon.AddRecipeSlot(ingredientBonbon);
            EditorUtility.SetDirty(recipeBonbon);
        }

        private void RemoveFromRecipe(BonbonBlueprint recipeBonbon, BonbonBlueprint ingredientBonbon) {
            recipeBonbon.RemoveRecipeSlot(ingredientBonbon);
            EditorUtility.SetDirty(recipeBonbon);
        }

        private void DrawEmptyBox() {
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                GUILayout.Box("", GUILayout.Width(buttonSize), GUILayout.Height(buttonSize));
            }
        }

        private void DrawRecipePreview(BonbonBlueprint bonbonObject) {
            EditorUtils.WindowBoxLabel("Recipe Preview");
            using (var scope = new EditorGUILayout.ScrollViewScope(previewScroll)) {
                previewScroll = scope.scrollPosition;
                DrawPreviewLevel(bonbonObject);
            }
        }

        private void DrawPreviewLevel(BonbonBlueprint bonbonObject) {
            if (bonbonObject == null) return;
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                var levelRect = DrawPaddedButton(bonbonObject);
                if (bonbonObject.recipe == null) bonbonObject.UpdateRecipeSize();
                
                using (new EditorGUILayout.HorizontalScope()) {
                    foreach (BonbonBlueprint ingredient in bonbonObject.recipe) {
                        if (ingredient != null) DrawPreviewLevel(ingredient);
                    }
                }
            }
        }

        private Rect DrawPaddedButton(BonbonBlueprint ingredient) {
            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize), GUILayout.ExpandWidth(false))) {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(ingredient.texture, UIStyles.CenteredLabel, GUILayout.Width(buttonSize), GUILayout.Height(buttonSize));
                    GUILayout.FlexibleSpace();
                } GUILayout.FlexibleSpace();
                return new Rect();
            }
        }

        private void SaveMap() {
            bonbonMapSO.bonbonMap = new ArrayArray<BonbonBlueprint>(globalBonbonMap);
            EditorUtility.SetDirty(bonbonMapSO);
        }
    }

    public class SkillManager : BonBaseTool {

        private BaseHierarchy<SkillObject> skillHierarchy;
        private SkillObject selectedSkill;
        private Editor skillInspector;
        private AssetCreator<SkillObject> assetCreator;

        private System.Type[] actionTypes;
        private ImmediateAction[] foundActions;

        public override void Initialize() {
            assetCreator = new AssetCreator<SkillObject>(MainGUI.assetPaths[(int) BAMGUI.ToolType.SkillManager]);
            skillHierarchy = new BaseHierarchy<SkillObject>(this);
            skillHierarchy.OnPathSelection += SkillHierarchy_OnPathSelection;
            assetCreator.OnAssetCreation += skillHierarchy.ReloadHierarchy;
            actionTypes = ActionUtils.FetchAssemblyChildren(new System.Type[] { typeof(ImmediateAction.SkillOnly) });
        }

        void OnDisable() {
            DestroyImmediate(skillInspector);
        }

        public void SkillHierarchy_OnPathSelection(string path) {
            SelectedPath = path;
            DestroyImmediate(skillInspector);
            skillInspector = null;
            SetSelectedSkill(AssetDatabase.LoadAssetAtPath<SkillObject>(path));
        }

        private void SetSelectedSkill(SkillObject skill) {
            selectedSkill = skill;
            if (selectedSkill.immediateActions == null) selectedSkill.immediateActions = new List<ImmediateAction.SkillOnly>();
            foundActions = ActionUtils.FetchAvailableActions(selectedSkill.immediateActions, actionTypes);
        }

        public override void ShowGUI() {
            using (new EditorGUILayout.HorizontalScope()) {
                using (new EditorGUILayout.VerticalScope()) {
                    skillHierarchy.ShowGUI();
                    assetCreator.ShowCreator();
                }
                    
                using (new EditorGUILayout.VerticalScope()) {
                    MainGUI.DrawToolbar();
                    using (var scope = new EditorGUILayout.ScrollViewScope(mainScroll)) {
                        mainScroll = scope.scrollPosition;
                        if (selectedSkill != null) {
                            if (skillInspector is null) skillInspector = Editor.CreateEditor(selectedSkill);
                            if (selectedSkill != null) {
                                if (SerializationUtility.HasManagedReferencesWithMissingTypes(selectedSkill)) {
                                    GUI.color = UIColors.Red;
                                    if (GUILayout.Button("Clear Missing References")) {
                                        SerializationUtility.ClearAllManagedReferencesWithMissingTypes(selectedSkill);
                                        selectedSkill.immediateActions = new List<ImmediateAction.SkillOnly>();
                                        EditorUtility.SetDirty(selectedSkill);
                                    } GUI.color = Color.white;
                                } skillInspector.OnInspectorGUI();
                            }

                            EditorGUILayout.Separator();
                            EditorUtils.DrawSeparatorLines(" Immediate Actions");
                            EditorGUILayout.Separator();

                            if (ActionUtils.DrawAvailableActions(ref selectedSkill.immediateActions,
                                                                 ref foundActions, actionTypes)) EditorUtility.SetDirty(selectedSkill);
                        } else {
                            EditorUtils.DrawScopeCenteredText("Select a Skill to edit it here;");
                        }
                    }
                }
            }
        }
    }

    public class EffectManager : BonBaseTool {

        private BaseHierarchy<EffectBlueprint> effectHierarchy;
        private EffectBlueprint selectedEffect;
        private Editor effectInspector;
        private AssetCreator<EffectBlueprint> assetCreator;

        private System.Type[] actionTypes;
        private ImmediateAction[] foundActions;

        public override void Initialize() {
            assetCreator = new AssetCreator<EffectBlueprint>(MainGUI.assetPaths[(int) BAMGUI.ToolType.EffectManager]);
            effectHierarchy = new BaseHierarchy<EffectBlueprint>(this);
            effectHierarchy.OnPathSelection += EffectHierarchy_OnPathSelection;
            assetCreator.OnAssetCreation += effectHierarchy.ReloadHierarchy;
            actionTypes = ActionUtils.FetchAssemblyChildren(new System.Type[] { typeof(ImmediateAction.EffectOnly) });
        }

        void OnDisable() {
            DestroyImmediate(effectInspector);
        }

        public void EffectHierarchy_OnPathSelection(string path) {
            SelectedPath = path;
            DestroyImmediate(effectInspector);
            effectInspector = null;
            SetSelectedEffect(AssetDatabase.LoadAssetAtPath<EffectBlueprint>(path));
        }

        private void SetSelectedEffect(EffectBlueprint effect) {
            selectedEffect = effect;
            if (selectedEffect.actions == null) selectedEffect.actions = new List<ImmediateAction.EffectOnly>();
            foundActions = ActionUtils.FetchAvailableActions(selectedEffect.actions, actionTypes);
        }

        public override void ShowGUI() {
            using (new EditorGUILayout.HorizontalScope()) {
                using (new EditorGUILayout.VerticalScope()) {
                    effectHierarchy.ShowGUI();
                    assetCreator.ShowCreator();
                }
                    
                using (new EditorGUILayout.VerticalScope()) {
                    MainGUI.DrawToolbar();

                    using (var scope = new EditorGUILayout.ScrollViewScope(mainScroll)) {
                        mainScroll = scope.scrollPosition;
                        if (selectedEffect != null) {
                            if (effectInspector is null) effectInspector = Editor.CreateEditor(selectedEffect);
                            if (selectedEffect != null) {
                                if (SerializationUtility.HasManagedReferencesWithMissingTypes(selectedEffect)) {
                                    GUI.color = UIColors.Red;
                                    if (GUILayout.Button("Clear Missing References")) {
                                        SerializationUtility.ClearAllManagedReferencesWithMissingTypes(selectedEffect);
                                        selectedEffect.actions = new List<ImmediateAction.EffectOnly>();
                                        EditorUtility.SetDirty(selectedEffect);
                                    } GUI.color = Color.white;
                                } effectInspector.OnInspectorGUI();
                            } else Debug.Log("Nope");

                            EditorGUILayout.Separator();
                            EditorUtils.DrawSeparatorLines(" Immediate Actions", true);
                            EditorGUILayout.Separator();

                            if (ActionUtils.DrawAvailableActions(ref selectedEffect.actions,
                                                                 ref foundActions, actionTypes)) EditorUtility.SetDirty(selectedEffect);
                        } else {
                            EditorUtils.DrawScopeCenteredText("Select an Effect to edit it here;");
                        }
                    }
                }
            }
        }
    }

    public class ActorManager : BonBaseTool {

        private BaseHierarchy<ActorData> actorHierarchy;
        private ActorData selectedActor;
        private AssetCreator<ActorData> assetCreator;
        private ActorMap prefabMapSO;

        private List<BonbonBlueprint> bonbonList => MainGUI.GlobalBonbonList;
        private List<SkillObject> skillList => MainGUI.GlobalSkillList;

        private float buttonSize = 35;
        private Vector2 upperScroll;
        private Vector2[] scrollGroup;

        private List<SkillObject>[] skillMap;
        private List<BonbonBlueprint>[] bonbonMap;

        public enum Mode {
            ActorEditor,
            ActorVisuals
        } public Mode mode;

        private enum MapEditor {
            Skill,
            Bonbon,
        } private MapEditor mapEditor;

        public override void Initialize() {
            assetCreator = new AssetCreator<ActorData>(MainGUI.assetPaths[(int) BAMGUI.ToolType.ActorManager]);
            actorHierarchy = new BaseHierarchy<ActorData>(this);
            actorHierarchy.OnPathSelection += ActorManager_OnPathSelection;
            assetCreator.OnAssetCreation += actorHierarchy.ReloadHierarchy;
            LoadPrefabMap();
        }

        public void SetMode(Mode mode) => this.mode = mode;

        private void LoadPrefabMap() {
            var guid = AssetDatabase.FindAssets($"t:{nameof(ActorMap)}")[0];
            prefabMapSO = AssetDatabase.LoadAssetAtPath<ActorMap>(AssetDatabase.GUIDToAssetPath(guid));
            prefabMapSO.PseudoActorMap = UpdatePrefabMap(prefabMapSO.PseudoActorMap);
        }

        private Dictionary<ActorData, GameObject> UpdatePrefabMap(Dictionary<ActorData, GameObject> prefabMap) {
            /// Cleanup of deprecated entries;
            foreach (KeyValuePair<ActorData, GameObject> kvp in prefabMap) {
                if (!MainGUI.GlobalActorList.Contains(kvp.Key)) prefabMap.Remove(kvp.Key);
            } /// Assignment of new entries;
            foreach (ActorData actorData in MainGUI.GlobalActorList) {
                if (!prefabMap.ContainsKey(actorData)) prefabMap[actorData] = null;
            } return prefabMap;
        }

        private void ActorManager_OnPathSelection(string path) {
            SelectedPath = path;
            SetSelectedActor(AssetDatabase.LoadAssetAtPath<ActorData>(path));
        }

        private void SetSelectedActor(ActorData data) {
            selectedActor = data;
            InitializeActorMaps();
        }

        public void ExternalActorSelection(ActorData actorData) {
            string path = AssetDatabase.GetAssetPath(actorData);
            SelectedPath = path;
            SetSelectedActor(actorData);
        }

        private void InitializeActorMaps() {
            if (selectedActor.skillMap != null) {
                selectedActor.skillMap = BAMUtils.VerifyMapSize(selectedActor.skillMap);
                skillMap = selectedActor.skillMap.ToListArray();
            } if (selectedActor.bonbonMap != null) {
                selectedActor.bonbonMap = BAMUtils.VerifyMapSize(selectedActor.bonbonMap);
                bonbonMap = selectedActor.bonbonMap.ToListArray();
            }
        }

        public override void ShowGUI() {
            using (new EditorGUILayout.HorizontalScope()) {
                using (new EditorGUILayout.VerticalScope()) {
                    actorHierarchy.ShowGUI();
                    assetCreator.ShowCreator();
                }
                
                using (new EditorGUILayout.VerticalScope()) {
                    MainGUI.DrawToolbar();

                    using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                        GUILayout.Label("Mode:", GUILayout.Width(50));
                        if (GUILayout.Button("Actor Editor", mode == Mode.ActorEditor
                                                        ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton,
                                                        GUILayout.ExpandWidth(true))) {
                            SetMode(Mode.ActorEditor);
                        } if (GUILayout.Button("Actor Visuals", mode == Mode.ActorVisuals
                                                        ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton,
                                                        GUILayout.ExpandWidth(true))) {
                            SetMode(Mode.ActorVisuals);
                        }
                    }

                    using (var scope = new EditorGUILayout.ScrollViewScope(mainScroll)) {
                        mainScroll = scope.scrollPosition;
                        if (selectedActor != null) {
                            switch (mode) {
                                case Mode.ActorEditor:
                                        DrawActorStats();
                                        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                                            GUILayout.Label("Map:", GUILayout.Width(50));
                                            if (GUILayout.Button("Skills", mapEditor == MapEditor.Skill
                                                                           ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton,
                                                                           GUILayout.ExpandWidth(true))) {
                                                mapEditor = MapEditor.Skill;
                                            } if (GUILayout.Button("Bonbons", mapEditor == MapEditor.Bonbon
                                                                              ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton,
                                                                              GUILayout.ExpandWidth(true))) {
                                                mapEditor = MapEditor.Bonbon;
                                            }
                                        }

                                        switch (mapEditor) {
                                            case MapEditor.Skill:
                                                BAMUtils.DrawAssetGroup(skillList, upperScroll, SkillObject.GUIContent,
                                                                        MainGUI.position, buttonSize);
                                                BAMUtils.DrawMap(skillMap, ref scrollGroup, SkillObject.GUIContent, buttonSize,
                                                                 MainGUI.assetRefs.dndFieldAssets, SaveMap);
                                                break;
                                            case MapEditor.Bonbon:
                                                BAMUtils.DrawAssetGroup(bonbonList, upperScroll, BonbonBlueprint.GUIContent,
                                                                    MainGUI.position, buttonSize);
                                                BAMUtils.DrawMap(bonbonMap, ref scrollGroup, BonbonBlueprint.GUIContent, buttonSize,
                                                                 MainGUI.assetRefs.dndFieldAssets, SaveMap);
                                                break;
                                        } break;
                                case Mode.ActorVisuals:
                                    using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox, GUILayout.Height(200))) {
                                        using (var changeScope1 = new EditorGUI.ChangeCheckScope()) {
                                            using (new EditorGUILayout.VerticalScope(GUILayout.Width(164))) {
                                                EditorUtils.WindowBoxLabel("Icon");
                                                using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                                                    if (selectedActor.Icon != null) {
                                                        if (GUILayout.Button(selectedActor.Icon.texture, 
                                                            GUILayout.Height(164), GUILayout.Width(164))) EditorGUIUtility.PingObject(selectedActor.Icon);
                                                    } else {
                                                        using (new EditorGUILayout.VerticalScope(EditorStyles.numberField, GUILayout.Width(164), GUILayout.Height(164))) {
                                                            EditorUtils.DrawScopeCenteredText("No Sprite Selected;");
                                                        }
                                                    }
                                                } using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                                                    if (GUILayout.Button("Select")) {
                                                        EditorGUIUtility.ShowObjectPicker<Sprite>(selectedActor.Icon, false, "", GUIUtility.GetControlID(FocusType.Passive) + 100);
                                                    } if (Event.current.commandName == "ObjectSelectorUpdated") {
                                                        var icon = EditorGUIUtility.GetObjectPickerObject();
                                                        if (icon is Sprite) selectedActor.SetIcon(icon as Sprite);
                                                    }
                                                }
                                            } EditorUtils.WindowBoxLabel("", GUILayout.Width(20), GUILayout.ExpandHeight(true));
                                            if (changeScope1.changed) EditorUtility.SetDirty(selectedActor);
                                        } using (var changeScope2 = new EditorGUI.ChangeCheckScope()) {
                                            using (new EditorGUILayout.VerticalScope()) {
                                                EditorUtils.WindowBoxLabel("Prefab");
                                                using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                                                    GameObject potentialPrefab = EditorGUILayout.ObjectField("Prefab:", prefabMapSO.PseudoActorMap[selectedActor],
                                                                                                                typeof(GameObject), false) as GameObject;
                                                    if (potentialPrefab != prefabMapSO.PseudoActorMap[selectedActor]) {
                                                        var dict = new Dictionary<ActorData, GameObject>(prefabMapSO.PseudoActorMap);
                                                        dict[selectedActor] = potentialPrefab;
                                                        prefabMapSO.PseudoActorMap = dict;
                                                    }
                                                } using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox, GUILayout.Height(175))) {
                                                    if (prefabMapSO.PseudoActorMap[selectedActor] != null) {
                                                        using (new EditorGUILayout.VerticalScope(GUILayout.Width(128))) {
                                                            EditorUtils.WindowBoxLabel("Preview");
                                                            using (new EditorGUILayout.HorizontalScope()) {
                                                                GUILayout.FlexibleSpace();
                                                                using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                                                                    GUILayout.Label(AssetPreview.GetAssetPreview(prefabMapSO.PseudoActorMap[selectedActor]));
                                                                } GUILayout.FlexibleSpace();
                                                            }
                                                        } using (new EditorGUILayout.VerticalScope()) {
                                                            EditorUtils.WindowBoxLabel("Prefab Status");
                                                            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                                                                List<PrefabWarning> warnings = ASEUtils.VerifyPrefabStatus(prefabMapSO.PseudoActorMap[selectedActor], selectedActor);
                                                                bool invalidActorScript = warnings.Contains(PrefabWarning.InvalidActorScript);
                                                                if (invalidActorScript) {
                                                                    EditorUtils.DrawCustomHelpBox(ASEUtils.PrefabWarningText[PrefabWarning.InvalidActorScript],
                                                                                                  EditorUtils.FetchIcon("Error"));
                                                                } else {
                                                                    EditorUtils.DrawCustomHelpBox("Valid Actor Script;",
                                                                                                  EditorUtils.FetchIcon("Valid"));
                                                                } bool invalidUIHandler = warnings.Contains(PrefabWarning.InvalidUIHandler);
                                                                if (invalidUIHandler) {
                                                                    EditorUtils.DrawCustomHelpBox(ASEUtils.PrefabWarningText[PrefabWarning.InvalidActorScript],
                                                                                                  EditorUtils.FetchIcon("Error"));
                                                                } else {
                                                                    string message = selectedActor is CharacterData ? "Valid UIHandler Script;" : "Doesn't require a UIHandler;";
                                                                    EditorUtils.DrawCustomHelpBox(message, EditorUtils.FetchIcon("Valid"));
                                                                } if (!(invalidUIHandler || invalidActorScript)) {
                                                                    GUILayout.FlexibleSpace();
                                                                    using (new EditorGUILayout.HorizontalScope()) {
                                                                        GUILayout.FlexibleSpace();
                                                                        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                                                                            EditorGUILayout.Space();
                                                                            GUILayout.Label(" All Good! ", UIStyles.CenteredLabelBold);
                                                                            EditorGUILayout.Space();
                                                                        } GUILayout.FlexibleSpace();
                                                                    } GUILayout.FlexibleSpace();
                                                                }
                                                            }
                                                        }
                                                    } else {
                                                        using (new EditorGUILayout.VerticalScope()) {
                                                            EditorUtils.DrawScopeCenteredText("No prefab has been assigned to this Actor;");
                                                        }
                                                    }
                                                }
                                            } if (changeScope2.changed) EditorUtility.SetDirty(prefabMapSO);
                                        } 
                                    } break; 
                            } 
                        } else EditorUtils.DrawScopeCenteredText("Select an Actor to edit it here;");
                    }
                }
            }
        }

        private void DrawActorStats() {
            CJToolAssets.StatFieldAssets statAssets = MainGUI.assetRefs.statFieldAssets;
            using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                using (var scope = new EditorGUI.ChangeCheckScope()) {
                    EditorUtils.WindowBoxLabel("Actor Notation");
                    using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                        selectedActor.SetDisplayName(EditorGUILayout.TextField("Actor Name", selectedActor.DisplayName));
                        selectedActor.SetID(EditorGUILayout.TextField("Actor ID", selectedActor.ID));
                    } EditorUtils.WindowBoxLabel("Actor Stats");
                    using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                        using (new EditorGUILayout.VerticalScope()) {
                            selectedActor.SetMaxHitpoints(EditorGUILayout.IntField(new GUIContent(" Max Hitpoints", statAssets.hitpoints),
                                                                selectedActor.MaxHitpoints));
                            selectedActor.SetMaxStamina(EditorGUILayout.IntField(new GUIContent(" Max Stamina", statAssets.stamina),
                                                                selectedActor.MaxStamina));
                            selectedActor.SetStaminaRegenRate(EditorGUILayout.IntField(new GUIContent(" Stamina Regen", statAssets.staminaRegen),
                                                              selectedActor.StaminaRegenRate));
                        } EditorGUILayout.Separator();
                        using (new EditorGUILayout.VerticalScope()) {
                            selectedActor.SetBasePotency(EditorGUILayout.IntField(new GUIContent(" Base Potency", statAssets.attack),
                                                                selectedActor.BasePotency));
                            selectedActor.SetBaseDefense(EditorGUILayout.IntField(new GUIContent(" Base Defense", statAssets.defense),
                                                                selectedActor.BaseDefense));
                            selectedActor.SetBaseSpeed(EditorGUILayout.IntField(new GUIContent(" Base Speed", statAssets.speed),
                                                                selectedActor.BaseSpeed));
                        }
                    } if (scope.changed) EditorUtility.SetDirty(selectedActor);
                }
            }
        }

        private void SaveMap() {
            switch(mapEditor) {
                case MapEditor.Skill:
                    selectedActor.skillMap = new ArrayArray<SkillObject>(skillMap);
                    break;
                case MapEditor.Bonbon:
                    selectedActor.bonbonMap = new ArrayArray<BonbonBlueprint>(bonbonMap);
                    break;
            } EditorUtility.SetDirty(selectedActor);
        }
    }
}