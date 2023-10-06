//using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;
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
        }

        private int buttonSize = 50;

        private Vector2 bonbonScroll;
        private Vector2 recipeScroll;
        private Vector2 previewScroll;

        public void SetSelectedBonbon(BonbonBlueprint bonbon) {
            selectedBonbon = bonbon;
            UpdateBonbonList();
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

                foreach (BonbonBlueprint ingredient in bonbon.recipe) {
                    if (ingredient == selectedBonbon) {
                        invalidBonbons.Add(bonbon);
                        break;
                    }
                }
            } foreach (BonbonBlueprint bonbon in invalidBonbons) bonbonList.Remove(bonbon);
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

                                    break;
                                case Mode.Recipe:
                                    BAMUtils.DrawAssetGroup(bonbonList, bonbonScroll, BonbonBlueprint.GUIContent,
                                                            MainGUI.position, buttonSize);
                                    DrawRecipeDropSlots();
                                    DrawRecipePreview(selectedBonbon);
                                    break;
                                case Mode.GlobalMap:
                                    BAMUtils.DrawAssetGroup(bonbonList, bonbonScroll, BonbonBlueprint.GUIContent,
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
                /*float levelStart = 0;
                float levelWidth = 0;
                int buttonCount = 0;*/
                
                using (new EditorGUILayout.HorizontalScope()) {
                    foreach (BonbonBlueprint ingredient in bonbonObject.recipe) {
                        if (ingredient != null) DrawPreviewLevel(ingredient);
                        //if (buttonCount == 0) levelStart = (rect.xMax - rect.x) / 2;
                        //levelWidth = (rect.xMax - rect.x) / 2 - levelStart;
                    }
                } /*var lineHeight = levelRect.y - 2;
                Handles.DrawLine(new Vector2(levelStart, lineHeight), new Vector2(levelStart + levelWidth, lineHeight));*/
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
            actionTypes = ActionUtils.FetchAssemblyChildren(new System.Type[] { typeof(ImmediateAction.Generic),
                                                                                typeof(ImmediateAction.SkillOnly)});
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
            if (selectedSkill.immediateActions == null) selectedSkill.immediateActions = new List<ImmediateAction>();
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
                            if (selectedSkill != null) skillInspector.OnInspectorGUI();
                            else Debug.Log("Nope");

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
            actionTypes = ActionUtils.FetchAssemblyChildren(new System.Type[] { typeof(ImmediateAction.Generic),
                                                                                typeof(ImmediateAction.EffectOnly)});
        }

        void OnDisable() {
            DestroyImmediate(effectInspector);
        }

        public void EffectHierarchy_OnPathSelection(string path) {
            SelectedPath = path;
            DestroyImmediate(effectInspector);
            effectInspector = null;
            SetSelectedSkill(AssetDatabase.LoadAssetAtPath<EffectBlueprint>(path));
        }

        private void SetSelectedSkill(EffectBlueprint effect) {
            selectedEffect = effect;
            if (selectedEffect.actions == null) selectedEffect.actions = new List<ImmediateAction>();
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
                            if (selectedEffect != null) effectInspector.OnInspectorGUI();
                            else Debug.Log("Nope");

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

        private List<BonbonBlueprint> bonbonList => MainGUI.GlobalBonbonList;
        private List<SkillObject> skillList => MainGUI.GlobalSkillList;

        private float buttonSize = 35;
        private Vector2 upperScroll;
        private Vector2[] scrollGroup;

        private List<SkillObject>[] skillMap;
        private List<BonbonBlueprint>[] bonbonMap;

        private enum MapEditor {
            Skill,
            Bonbon,
        } private MapEditor mapEditor;

        public override void Initialize() {
            assetCreator = new AssetCreator<ActorData>(MainGUI.assetPaths[(int) BAMGUI.ToolType.ActorManager]);
            actorHierarchy = new BaseHierarchy<ActorData>(this);
            actorHierarchy.OnPathSelection += ActorManager_OnPathSelection;
            assetCreator.OnAssetCreation += actorHierarchy.ReloadHierarchy;
        }

        private void ActorManager_OnPathSelection(string path) {
            SelectedPath = path;
            SetSelectedActor(AssetDatabase.LoadAssetAtPath<ActorData>(path));
        }

        private void SetSelectedActor(ActorData data) {
            selectedActor = data;
            InitializeActorMaps();
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

                    using (var scope = new EditorGUILayout.ScrollViewScope(mainScroll)) {
                        mainScroll = scope.scrollPosition;
                        if (selectedActor != null) {
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
                        }
                            
                        } else {
                            EditorUtils.DrawScopeCenteredText("Select an Actor to edit it here;");
                        }
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