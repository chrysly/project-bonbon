using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using CJUtils;
#endif

[System.Serializable]
public class DelaySkillEffectAnimation : DelaySkillAnimation
{
    [SerializeField] private float triggerTime;

    public override IEnumerable<IEnumerator> GetCoroutines(AnimationHandler handler, AIActionValue[] avs, Actor[] targets) {
        yield return CreateCoroutine();
    }

    private IEnumerator CreateCoroutine() {
        yield return new WaitForSeconds(triggerTime);
        // Do nothing since AIActionValue doesn't have effects yet...
    }

#if UNITY_EDITOR
    protected override void InnerGUI() {
        using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
            triggerTime = Mathf.Max(EditorGUILayout.FloatField("Apply all effects at", triggerTime), 0);
            EditorGUILayout.LabelField("s", GUILayout.Width(24));
        }
    }

    protected override void InitializeFields() {
        triggerTime = 0;
    }
#endif
}
