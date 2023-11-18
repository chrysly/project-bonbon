using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BonbonAssetManager;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public abstract class DelaySkillAnimation
{
    public static System.Type[] subTypes = FetchAssemblyChildren(new System.Type[] { typeof(DelaySkillAnimation) });

    public abstract IEnumerable<IEnumerator> GetCoroutines(AnimationHandler handler, AIActionValue[] avs, Actor[] targets);

    public static System.Type[] FetchAssemblyChildren(System.Type[] superTypes) {
        List<System.Type> typeList = new List<System.Type>();
        foreach (System.Type superType in superTypes) {
            System.Type[] assemblies = System.Reflection.Assembly.GetAssembly(superType).GetTypes();
            foreach (System.Type type in assemblies) {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(superType)) typeList.Add(type);
            }
        } return typeList.ToArray();
    }

#if UNITY_EDITOR
    private bool enabled;

    public void OnGUI() {
        using (var scope = new EditorGUILayout.ToggleGroupScope(this.GetType().Name, enabled)) {
            if (enabled && !scope.enabled) InitializeFields();
            enabled = scope.enabled;
            if (!enabled) return;
            InnerGUI();
        }
    }

    protected abstract void InnerGUI();

    protected abstract void InitializeFields();
#endif
}
