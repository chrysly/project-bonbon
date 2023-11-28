using UnityEngine;

namespace BattleUI {
    public class UINavigationManager : MonoBehaviour {

        [SerializeField] private GameObject guiCursorGO;
        private GUICursorAnimator guiCursorAnim;
        [SerializeField] private GameObject targetCursorGO;
        private TargetCursorAnimator targetCursorAnim;
        
        public UIButtonAnimator SelectedButton { get; private set; }
        private UIAnimationBrain brain;
        private CursorAnimator currCursor;
        
        void Awake() {
            brain = GetComponent<UIAnimationBrain>();
            brain.OnSelectionChange += UIAnimationBrain_UpdateCursor;

            InitializeCursor(guiCursorGO, ref guiCursorAnim);
            InitializeCursor(targetCursorGO, ref targetCursorAnim);
        }

        private void InitializeCursor<T>(GameObject cursorGO,
                                         ref T cursorAnim) where T : CursorAnimator {
            Instantiate(cursorGO, transform);
            cursorAnim = cursorGO.GetComponentInChildren<T>(true);
            cursorAnim.gameObject.SetActive(false);
        }

        private void UIAnimationBrain_UpdateCursor(UIButtonAnimator buttonAnim) {
            bool isDifferent = SelectedButton != null && SelectedButton.Button == buttonAnim.Button;
            Debug.LogError(SelectedButton != null ? SelectedButton.Button : null);
            if (SelectedButton != null) SelectedButton.OverrideSelect(false);
            SelectedButton = buttonAnim;
            SelectedButton.OverrideSelect(true);
            CursorAnimator cursorAnim = buttonAnim.Button is TargetSelectButton ? targetCursorAnim : guiCursorAnim;
            if (currCursor != null && (cursorAnim.GetType() != currCursor.GetType())) currCursor.Despawn();
            cursorAnim.gameObject.SetActive(true);
            cursorAnim.SpawnAt(buttonAnim.CursorTarget);
            currCursor = cursorAnim;
        }
    }
}