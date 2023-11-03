using System;

/// <summary>
/// Generic definition for an action;
/// </summary>
[Serializable]
public abstract class ImmediateAction {

    public abstract class Generic : ImmediateAction { }
    public abstract class SkillOnly : ImmediateAction { }
    public abstract class EffectOnly : ImmediateAction { }

    /// <summary>
    /// Override to implement AI value yield;
    /// </summary>
    /// <param name="actionValue"> Value bundle to operate on; </param>
    public virtual void ComputeActionValue(ref AIActionValue actionValue, StatIteration casterData) { }

    /// <summary>
    /// Trigger the action encompassed by this object;
    /// </summary>
    /// <param name="activeData"> Active data of the casting actor; </param>
    /// <param name="target"> Target actor for the skill; </param>
    public abstract void Use(StatIteration activeData, Actor target = null);

    #if UNITY_EDITOR

    public void OnGUI() {
        using (new UnityEditor.EditorGUILayout.HorizontalScope(CJUtils.UIStyles.WindowBox)) {
            DrawActionGUI();
        }
    }

    protected abstract void DrawActionGUI();

    #endif
}
