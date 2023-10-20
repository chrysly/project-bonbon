using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

public class SkillSelectButton : BattleButton {
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private TextMeshProUGUI staminaText;
    [SerializeField] private CanvasGroup selectPanel;
    [SerializeField] private string resourceAcronym = "STA";

    private SkillAction _skillObject;

    public void Initialize(SkillAction skillObject) {
        _skillObject = skillObject;
        skillText.SetText(skillObject.SkillData.GetSkillName());
        staminaText.SetText(skillObject.SkillData.staminaCost + " " + resourceAcronym);
    }

    public void Select(float delay) {
        selectPanel.DOFade(1f, delay);
        transform.DOScale(1.1f, delay);
    }

    public void Deselect(float delay) {
        selectPanel.DOFade(0f, delay);
        transform.DOScale(1f, delay);
    }

    public SkillAction Confirm() {
        return _skillObject;
    }
}
