using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButton : MonoBehaviour
{
    [SerializeField] private SkillObject skill;

    public void AssignSkill(SkillObject skill) {
        this.skill = skill;
        UpdateText();
    }

    public SkillObject RetrieveSkill() {
        return skill;
    }

    private void UpdateText() {
        TextMeshProUGUI text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.SetText(skill.GetSkillName());
    }
}
