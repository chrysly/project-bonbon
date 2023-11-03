using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using CJUtils;
using PseudoDataStructures;

public class SkillAnimationEditor : EditorWindow {

    private SkillAnimationMap sam;

    private Dictionary<SkillObject, Dictionary<ActorData, SkillAnimation>> animationMap;
    private SkillObject[] skills;
    private ActorData[] actors;

    private Vector2 hierarchyScroll;
    private Vector2 panelScroll;

    private SkillAnimation selectedAnimation;

    private enum SectionType {
        Animation,
        VFX,
        Camera,
    } private SectionType activeSection;

    [MenuItem("Testing/Skill Animation Editor")]
    public static void ShowWindow() => GetWindow<SkillAnimationEditor>();

    void OnEnable() {
        sam = LoadAssets<SkillAnimationMap>()[0];

        animationMap = SKAEUtils.ProcessInternalDictionary(sam.animationMap);
        skills = LoadAssets<SkillObject>();
        actors = LoadAssets<ActorData>();
    }

    void OnDisable() {
        /// Need to null these variables to prevent stray 
        /// asset references and unload all assets from memory;
        sam = null;
        animationMap = null;
        skills = null;
        actors = null;
        if (selectedAnimation != null) {
            selectedAnimation.CleanDelayEditor();
        } selectedAnimation = null;
        Resources.UnloadUnusedAssets();
    }

    private T[] LoadAssets<T>() where T : ScriptableObject {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).FullName}");
        string[] assetPaths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();
        return assetPaths.Select(assetPath => AssetDatabase.LoadAssetAtPath<T>(assetPath)).ToArray();
    }

    void OnGUI() {
        using (new EditorGUILayout.HorizontalScope()) {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(180))) {
                using (new EditorGUILayout.HorizontalScope(UIStyles.PaddedToolbar)) {
                    EditorGUILayout.TextField("", EditorStyles.toolbarSearchField);
                } using (var scope = new EditorGUILayout.ScrollViewScope(hierarchyScroll)) {
                    hierarchyScroll = scope.scrollPosition;
                    using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true))) {
                        DrawMapEntries();
                    }
                } using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                    Rect rect = EditorGUILayout.GetControlRect();
                    if (GUI.Button(rect, EditorUtils.FetchIcon("d_P4_AddedRemote"))) {
                        AdvancedDropdown<SkillObject> dropdown = new AdvancedDropdown<SkillObject>(skills.Where(so => !animationMap.Keys.Contains(so)).ToArray(), 
                                                                                                   skill => skill.Name, OnSkillSelected);
                        dropdown.Show(rect);
                    }
                }
            } using (new EditorGUILayout.VerticalScope()) {
                DrawToolbar();
                using (var changeScope = new EditorGUI.ChangeCheckScope()) {
                    using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
                        using (var scope = new EditorGUILayout.ScrollViewScope(panelScroll)) {
                            panelScroll = scope.scrollPosition;
                            if (selectedAnimation != null) {
                                switch (activeSection) {
                                    case SectionType.Animation:
                                        selectedAnimation.SetActorAnimator(EditorGUILayout.ObjectField(selectedAnimation.actorAnimator,
                                                                                                       typeof(AnimatorController), false) as AnimatorController);
                                        if (selectedAnimation.Triggers != null) {
                                            Rect rect = EditorGUILayout.GetControlRect();
                                            selectedAnimation.triggerIndex = EditorGUI.Popup(rect, selectedAnimation.triggerIndex, selectedAnimation.Triggers);
                                            selectedAnimation.SetAnimationTrigger(selectedAnimation.Triggers[selectedAnimation.triggerIndex]);
                                        } else {
                                            EditorGUILayout.Separator();
                                            GUI.color = UIColors.DarkRed;
                                            GUILayout.Label("Invalid Animator Assignment;");
                                            GUI.color = Color.white;
                                        } selectedAnimation.SetAnimationDuration(EditorGUILayout.FloatField("Animation Length", selectedAnimation.AnimationDuration));
                                        selectedAnimation.SetHitDelay(EditorGUILayout.FloatField("Hit Delay", selectedAnimation.HitDelay));
                                        break;
                                    case SectionType.VFX:
                                        selectedAnimation.SetVFXPrefab(EditorGUILayout.ObjectField(selectedAnimation.VFXPrefab,
                                                                                                   typeof(GameObject), false) as GameObject);
                                        if (selectedAnimation.DelayScriptEditor is not null) {
                                            selectedAnimation.DelayScriptEditor.OnInspectorGUI();
                                        } break;
                                    case SectionType.Camera:
                                        EditorUtils.DrawScopeCenteredText("This section has not been implemented yet;");
                                        break;
                                }
                            } else EditorUtils.DrawScopeCenteredText("Select an Animation Entry to edit;");
                        }
                    } /*if (changeScope.changed)*/ SaveMap();
                }
            }
        }
    }

    private void DrawToolbar() {
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
            foreach (SectionType section in System.Enum.GetValues(typeof(SectionType))) {
                var name = System.Enum.GetName(typeof(SectionType), section).CamelSpace();
                if (GUILayout.Button(name, activeSection == section
                                           ? UIStyles.SelectedToolbar : EditorStyles.toolbarButton,
                                           GUILayout.MinWidth(150), GUILayout.ExpandWidth(true))) activeSection = section;
            }
        }
    }

    private void OnSkillSelected(SkillObject so) {
        animationMap[so] = new Dictionary<ActorData, SkillAnimation>();
        SaveMap();
    }

    private void DrawMapEntries() {
        foreach (KeyValuePair<SkillObject, Dictionary<ActorData, SkillAnimation>> kvp1 in animationMap) {
            
            using (new EditorGUILayout.VerticalScope(GUILayout.Height(40 * kvp1.Value.Count))) {
                using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                    GUI.color = UIColors.Red;
                    GUIContent deleteButton = new GUIContent(EditorUtils.FetchIcon("d_winbtn_win_close@2x"));
                    if (GUILayout.Button(deleteButton, GUILayout.Width(30), GUILayout.Height(19))) {
                        if (BonbonAssetManager.ModalAssetDeletion.ConfirmAssetDeletion($"{kvp1.Key.Name} Animation Entry")) {
                            animationMap.Remove(kvp1.Key);
                            SaveMap();
                        } GUIUtility.ExitGUI();
                    } GUI.color = UIColors.DarkGreen;
                    GUILayout.Label(kvp1.Key.Name, new GUIStyle(UIStyles.CenteredLabelBold) { contentOffset = new Vector2(0, 2) });
                    GUI.color = UIColors.Cyan;
                    Rect rect = EditorGUILayout.GetControlRect(GUILayout.Width(42));
                    GUI.color = UIColors.Blue;
                    if (GUI.Button(rect, EditorUtils.FetchIcon("d_P4_AddedRemote"), EditorStyles.miniButtonMid)) {
                        AdvancedDropdown<ActorData> dropdown = new AdvancedDropdown<ActorData>(actors.Where(ad => !kvp1.Value.Keys.Contains(ad)).ToArray(),
                                                                                                skill => skill.DisplayName,
                                                                                                (data) => {
                                                                                                    kvp1.Value[data] = new SkillAnimation();
                                                                                                    SaveMap();
                                                                                                });
                        dropdown.Show(rect);
                    } GUI.color = Color.white;
                }

                foreach (KeyValuePair<ActorData, SkillAnimation> kvp2 in kvp1.Value) {
                    using (new EditorGUILayout.HorizontalScope()) {
                                            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                        GUI.color = UIColors.Red;
                        GUIContent deleteButton = new GUIContent(EditorUtils.FetchIcon("TreeEditor.Trash"));
                        if (GUILayout.Button(deleteButton, kvp2.Value == selectedAnimation
                                                            ? new GUIStyle(UIStyles.HButton) { margin = UIStyles.HButtonSelected.margin }
                                                            : UIStyles.HButton, GUILayout.Width(30), GUILayout.Height(19))) {
                                if (BonbonAssetManager.ModalAssetDeletion.ConfirmAssetDeletion($"{kvp2.Key.DisplayName} Animation Entry")) {
                                    kvp1.Value.Remove(kvp2.Key);
                                    SaveMap();
                                } GUIUtility.ExitGUI();
                            } GUI.color = Color.white;
                        } using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                            if (GUILayout.Button(kvp2.Key.DisplayName, kvp2.Value == selectedAnimation
                                                                        ? UIStyles.ArrangedButtonSelected : GUI.skin.button)) {
                                selectedAnimation = kvp2.Value;
                            }
                        }
                    }
                }
            }
        }
    }

    private void SaveMap() {
        sam.animationMap = SKAEUtils.RevertInternalDictionary(animationMap);
        EditorUtility.SetDirty(sam);
    }
}