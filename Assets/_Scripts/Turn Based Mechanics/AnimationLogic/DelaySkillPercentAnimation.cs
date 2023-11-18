using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using CJUtils;
#endif

[System.Serializable]
public abstract class DelaySkillPercentAnimation : DelaySkillAnimation
{
    [SerializeField] protected List<PercentTrigger> triggers = new List<PercentTrigger> { new PercentTrigger(0, 100) };

    public override IEnumerable<IEnumerator> GetCoroutines(AnimationHandler handler, AIActionValue[] avs, Actor[] targets) {
        return triggers.Select(target => CreateCoroutine(handler, avs, targets, target));
    }

    protected abstract IEnumerator CreateCoroutine(AnimationHandler handler, AIActionValue[] avs, Actor[] targets, PercentTrigger trigger);

#if UNITY_EDITOR
    protected string description = string.Empty;

    protected override void InnerGUI() {
        using (new EditorGUILayout.VerticalScope(UIStyles.WindowBox)) {
            foreach (var trigger in triggers) {
                using (new EditorGUILayout.HorizontalScope()) {
                    using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                        GUI.color = UIColors.Red;
                        GUIContent deleteButton = new GUIContent(EditorUtils.FetchIcon("TreeEditor.Trash"));
                        if (GUILayout.Button(deleteButton, GUILayout.Width(30), GUILayout.Height(EditorGUIUtility.singleLineHeight))) {
                            triggers.Remove(trigger);
                            GUIUtility.ExitGUI();
                        } GUI.color = Color.white;
                    }
                    using (new EditorGUILayout.HorizontalScope(UIStyles.WindowBox)) {
                        trigger.OnGUI(description);
                    }
                }
            }
            using (new EditorGUILayout.HorizontalScope()) {
                GUI.color = UIColors.Cyan;
                Rect deleteRect = EditorGUILayout.GetControlRect();
                GUI.color = UIColors.Blue;
                if (GUI.Button(deleteRect, EditorUtils.FetchIcon("d_P4_AddedRemote"), EditorStyles.miniButtonMid)) {
                    triggers.Add(new PercentTrigger());
                }

                GUI.color = UIColors.Green;
                Rect sortRect = EditorGUILayout.GetControlRect();
                GUI.color = UIColors.DarkGreen;
                if (GUI.Button(sortRect, EditorUtils.FetchIcon("CustomSorting"), EditorStyles.miniButtonMid)) {
                    GUI.FocusControl(string.Empty);
                    triggers.Sort(new PercentTrigger.CompareByTriggerTime());
                } GUI.color = Color.white;
            }
        }
    }

    protected override void InitializeFields() {
        triggers.Clear();
        triggers.Add(new PercentTrigger(0, 100));
    }
#endif
}
