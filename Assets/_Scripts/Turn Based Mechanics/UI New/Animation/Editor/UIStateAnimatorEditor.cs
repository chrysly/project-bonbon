using UnityEngine;
using UnityEditor;

namespace BattleUI {
    [CustomEditor(typeof(UIStateAnimator))]
    public class UIStateAnimatorEditor : Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            UIStateAnimator stateAnimator = target as UIStateAnimator;
            if (GUILayout.Button("Collect Animators")) {
                stateAnimator.EditorAnimators = stateAnimator.GetComponentsInChildren<UIAnimator>(true);
                EditorUtility.SetDirty(stateAnimator);
            }
        }
    }
}