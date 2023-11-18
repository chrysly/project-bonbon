using System;
using UnityEngine;

/// <summary>
/// Generic definition for an action;
/// </summary>
[Serializable]
public abstract class ImmediateAction {

    [Serializable]
    public abstract class SkillOnly : ImmediateAction {
        /// <summary>
        /// Trigger the action encompassed by this object, signature exclusive to skills;
        /// </summary>
        /// <param name="activeData"> Active data of the casting actor; </param>
        /// <param name="target"> Target actor for the skill; </param>
        public abstract void Use(StatIteration activeData, Actor target = null);

        /// <summary>
        /// Override to implement AI value yield, signature exclusive to skills;
        /// </summary>
        /// <param name="actionValue"> Value bundle to operate on; </param>
        public virtual void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) { }
    }
    [Serializable]
    public abstract class EffectOnly : ImmediateAction {

        /// <summary>
        /// Trigger the action encompassed by this object, signature exclusive to effects;
        /// </summary>
        /// <param name="activeData"> Active data of the casting actor; </param>
        /// <param name="target"> Target actor for the skill; </param>
        public abstract void Use(StatIteration activeData, Actor target = null, int duration = 1);

        /// <summary>
        /// Override to implement AI value yield, signature exclusive to skills;
        /// </summary>
        /// <param name="actionValue"> Value bundle to operate on; </param>
        public virtual void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData, int duration) { }
    }

    #if UNITY_EDITOR

    public void OnGUI() {
        using (new UnityEditor.EditorGUILayout.HorizontalScope(CJUtils.UIStyles.WindowBox)) {
            DrawActionGUI();
        }
    }

    protected abstract void DrawActionGUI();

    #endif
}
