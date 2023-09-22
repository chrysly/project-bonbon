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

    public override void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) {
        foreach (EffectBlueprint effect in effects) effect.ComputeEffectValue(ref actionValue, casterData);
    }

    public override void Use(StatIteration activeData, Actor target = null) {
        List<Effect> appliedEffectList = new List<Effect>();
        foreach (EffectBlueprint effect in effects) appliedEffectList.Add(effect.InstantiateEffect(activeData));
        target.ApplyEffects(appliedEffectList);
    }

#if UNITY_EDITOR

    private List<EffectBlueprint> globalEffects;
    private int buttonSize = 50;
    private Vector2 upperScroll;
    private Vector2 lowerScroll;

    protected override void DrawActionGUI() {
        if (globalEffects == null) globalEffects = BonbonAssetManager.BAMUtils.InitializeList<EffectBlueprint>();
        DrawEffectGroup();
    }

    private void DrawEffectGroup() {
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(80))) {
            EditorUtils.WindowBoxLabel("Available\nEffects", GUILayout.Height(buttonSize * 1.35f));
            EditorUtils.WindowBoxLabel("Applied\nEffects", GUILayout.Height(buttonSize * 1.35f));
        }
       
        using (new EditorGUILayout.VerticalScope()) {
            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                using (var scope = new EditorGUILayout.ScrollViewScope(upperScroll, GUILayout.ExpandWidth(true), GUILayout.Height(buttonSize * 1.35f))) {
                    upperScroll = scope.scrollPosition;
                
                    for (int i = 0; i < globalEffects.Count; i++) {
                        BonbonAssetManager.BAMUtils.DrawBonbonDragButton(globalEffects[i], new GUIContent(globalEffects[i].name), buttonSize);
                    }
                }
            }
            using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                EffectBlueprint acceptedEffect = BonbonAssetManager.BAMUtils.DrawDragAcceptButton<EffectBlueprint>(GUILayout.Width(buttonSize * 2),
                                                                                                                   GUILayout.Height(buttonSize));
                if (acceptedEffect != null) AddEffect(acceptedEffect);
                using (var scope = new EditorGUILayout.ScrollViewScope(lowerScroll, GUILayout.ExpandWidth(true), GUILayout.Height(buttonSize * 1.35f))) {
                    lowerScroll = scope.scrollPosition;
                    for (int i = 0; i < effects.Count; i++) {
                        BonbonAssetManager.BAMUtils.DrawBonbonDragButton(effects[i], new GUIContent(effects[i].name), buttonSize);
                    }
                } EffectBlueprint removedEffect = BonbonAssetManager.BAMUtils.DrawDragAcceptButton<EffectBlueprint>(GUILayout.Width(buttonSize * 2),
                                                                                                                    GUILayout.Height(buttonSize));
                if (removedEffect != null) RemoveEffect(removedEffect);
            } 
        }
    }

    /// <summary>
    /// EDITOR-ONLY: Add an Effect to the effect list in the class;
    /// </summary>
    /// <param name="effect"> Effect to add; </param>
    private void AddEffect(EffectBlueprint effect) {
        effects.Add(effect);
    }

    /// <summary>
    /// EDITOR-ONLY: Remove an Effect from the effect list in the class;
    /// </summary>
    /// <param name="effect"> Effect to remove; </param>
    private void RemoveEffect(EffectBlueprint effect) {
        effects.Remove(effect);
    }

    #endif
}