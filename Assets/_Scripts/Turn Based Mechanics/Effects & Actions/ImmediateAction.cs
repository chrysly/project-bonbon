using System;
/// <summary>
/// Generic definition for an action;
/// </summary>
[Serializable]
public abstract class ImmediateAction {

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

    public void DrawProperty() {
        using (new UnityEditor.EditorGUILayout.HorizontalScope(CJUtils.UIStyles.WindowBox)) {
            UnityEditor.EditorGUILayout.IntField(0);
        }
    }

    #endif
}
