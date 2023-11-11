using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class AnimationEventTrigger
{
    public enum EventType {
        Empty = 0,
        Damage,
        Heal,
        Effect,
    }

    [Flags]
    enum TargetFlag {
        None = 0,
        Target1 = 1 << 0,
        Target2 = 1 << 1,
        Target3 = 1 << 2,
    }

    [SerializeField] private EventType type;
    public EventType Type => type;
    [SerializeField] private float timestamp;
    [SerializeField] private TargetFlag targetFlag;

    [SerializeField] private int value;
    [SerializeField] private EffectBlueprint effect;

    public IEnumerable QueueTrigger(AnimationHandler handler, Actor[] targets) {
        yield return new WaitForSeconds(timestamp);

        int i = 0;
        foreach (TargetFlag flagType in Enum.GetValues(typeof(TargetFlag))) {
            if (i == targets.Length) {
                break;
            }

            if (targetFlag.HasFlag(flagType)) {
                TriggerEvent(handler, targets[i]);
            }
            ++i;
        }
    }

    private void TriggerEvent(AnimationHandler handler, Actor target) {
        switch (type) {
            case EventType.Damage:
                handler.TriggerDamage(value, target);
                break;
            case EventType.Heal:
                handler.TriggerHeal(value, target);
                break;
            case EventType.Effect:
                handler.TriggerEffect(effect, target);
                break;
        }
    }

#if UNITY_EDITOR
    private bool foldoutState;

    public void SwitchEventType(EventType type) {
        if (type != this.type) {
            value = 0;
            effect = null;
            this.type = type;
        }
    }

    public void DrawProperty() {
        timestamp = EditorGUILayout.FloatField("Trigger Time", timestamp);
        switch (type) {
            case EventType.Damage:
                value = EditorGUILayout.IntField("Damage", value);
                break;
            case EventType.Heal:
                value = EditorGUILayout.IntField("Heal", value);
                break;
            case EventType.Effect:
                effect = EditorGUILayout.ObjectField("Effect", effect, typeof(EffectBlueprint), false) as EffectBlueprint;
                break;
        }
        foldoutState = EditorGUILayout.Foldout(foldoutState, "Select Targets", true);
        if (foldoutState) {
            EditorGUI.indentLevel++;
            using (new EditorGUILayout.HorizontalScope()) {
                foreach (TargetFlag flagType in Enum.GetValues(typeof(TargetFlag))) {
                    if (flagType == TargetFlag.None) {
                        continue;
                    }
                    if (EditorGUILayout.Toggle(flagType.ToString(), targetFlag.HasFlag(flagType))) {
                        targetFlag |= flagType;
                    }
                    else {
                        targetFlag &= ~flagType;
                    }
                }
            }
            EditorGUI.indentLevel--;
        }
    }
#endif
}
