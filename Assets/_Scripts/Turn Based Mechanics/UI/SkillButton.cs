using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
{
    [SerializeField] private SkillAction skill;

    public void AssignSkill(SkillAction skill) {
        this.skill = skill;
        UpdateText();
    }

    public SkillAction RetrieveSkill() {
        return skill;
    }

    private void UpdateText() {
        TextMeshProUGUI text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.SetText(skill.SkillData.GetSkillName());
    }
}
