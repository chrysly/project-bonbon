using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIAnimationHandler : MonoBehaviour {

    public IEnumerator activeUIAction = null;

    public bool QueueIsEmpty() {
        return activeUIAction == null;
    }
    
    #region Main Window
    [Header("Main Panel")] 
    [SerializeField] private CanvasGroup mainPanel;
    [SerializeField] private float mainPanelToggleDuration;

    public void ToggleMainPanel(bool enable) {
        if (QueueIsEmpty()) {
            if (enable) mainPanel.gameObject.SetActive(true);
            activeUIAction = MainPanelAction(enable);
            StartCoroutine(activeUIAction);
        }
    }

    private IEnumerator MainPanelAction(bool enable) {
        mainPanel.DOFade(enable ? 1 : 0, mainPanelToggleDuration);
        yield return new WaitForSeconds(mainPanelToggleDuration);
        if (!enable) mainPanel.gameObject.SetActive(false);
        activeUIAction = null;
        yield return null;
    }
    #endregion Primary Window
    
    
    
}
