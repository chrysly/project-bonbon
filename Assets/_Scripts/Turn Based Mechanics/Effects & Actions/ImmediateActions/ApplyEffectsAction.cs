using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using CJUtils;
#endif

/// <summary>
/// Action to Apply Effects to the given targets;
/// </summary>
[Serializable]
public class ApplyEffectsAction : ImmediateAction.SkillOnly {

    /// <summary> List of effects applied by the action; </summary>
    [SerializeField] private List<EffectBlueprint> effects;

    public ApplyEffectsAction() {
        effects = new List<EffectBlueprint>();
    }

    public ApplyEffectsAction(List<EffectBlueprint> effects) {
        this.effects = effects;
    }

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) {
        foreach (EffectBlueprint effect in effects) effect.ComputeEffectValue(ref actionValue, casterData);
    }

    public override void Use(StatIteration activeData, Actor target = null) {
        List<Effect> appliedEffectList = new List<Effect>();
        foreach (EffectBlueprint effect in effects) {
            appliedEffectList.Add(effect.InstantiateEffect(activeData));
        } target.ApplyEffects(appliedEffectList);
    }

    #if UNITY_EDITOR

    private CJToolAssets.DnDFieldAssets fieldAssets;
    private List<EffectBlueprint> globalEffects;
    private int buttonSize = 35;
    private Vector2 upperScroll;
    private Vector2 lowerScroll;

    protected override void DrawActionGUI() {
        if (fieldAssets == null) fieldAssets = FieldUtils.GetDnDFieldAssets();
        if (globalEffects == null) globalEffects = BonbonAssetManager.BAMUtils.InitializeList<EffectBlueprint>();
        DrawEffectGroup();
    }

    private void DrawEffectGroup() {
       
        using (new EditorGUILayout.VerticalScope()) {
            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                EditorUtils.WindowBoxLabel("Available\nEffects", GUILayout.Width(55), GUILayout.Height(45));
                using (var scope = new EditorGUILayout.ScrollViewScope(upperScroll, false, false, GUI.skin.horizontalScrollbar, GUIStyle.none, GUI.skin.scrollView,
                                                                       GUILayout.ExpandWidth(true), GUILayout.Height(buttonSize * 1.6f))) {
                    upperScroll = scope.scrollPosition;
                
                    for (int i = 0; i < globalEffects.Count; i++) {
                        using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                            BonbonAssetManager.BAMUtils.DrawAssetDragButton(globalEffects[i], EffectBlueprint.GUIContent, buttonSize);
                        }
                    }
                }
            }
            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                EditorUtils.WindowBoxLabel("Applied\nEffects", GUILayout.Width(55), GUILayout.Height(45));
                BonbonAssetManager.BAMUtils.DrawLevelDropdown(effects, ref lowerScroll, EffectBlueprint.GUIContent, buttonSize, fieldAssets);
            }
            /*
            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                EffectBlueprint acceptedEffect = BonbonAssetManager.BAMUtils.DrawDragAcceptButton<EffectBlueprint>(FieldUtils.DnDFieldType.Add,
                                                                                                                   fieldAssets, GUILayout.Width(buttonSize),
                                                                                                                   GUILayout.Height(buttonSize));
                if (acceptedEffect != null && !effects.Contains(acceptedEffect)) AddEffect(acceptedEffect);
                using (var scope = new EditorGUILayout.ScrollViewScope(lowerScroll, false, false, GUI.skin.horizontalScrollbar, GUIStyle.none, GUI.skin.scrollView,
                                                                       GUILayout.ExpandWidth(true), GUILayout.Height(buttonSize * 1.35f))) {
                    lowerScroll = scope.scrollPosition;
                    for (int i = 0; i < effects.Count; i++) {
                        using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                            BonbonAssetManager.BAMUtils.DrawAssetDragButton(effects[i], new GUIContent(effects[i].name), buttonSize);
                        }
                    }
                } EffectBlueprint removedEffect = BonbonAssetManager.BAMUtils.DrawDragAcceptButton<EffectBlueprint>(FieldUtils.DnDFieldType.Remove,
                                                                                                                    fieldAssets, GUILayout.Width(buttonSize),
                                                                                                                    GUILayout.Height(buttonSize));
                if (removedEffect != null) RemoveEffect(removedEffect);
            } */
        }
    }

    #endif
}