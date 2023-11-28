using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BattleUI {
    public class BonbonCraftButtonAnimator : UIButtonAnimator {
        
        public override void Toggle(bool toggle) {
            base.Toggle(toggle);
            if (!toggle) return;
            SkillSelectButton skillButton = Button as SkillSelectButton;
            GetComponent<SkillNameIdentifier>().GetComponent<TextMeshProUGUI>().text =
                skillButton.Skill.SkillData.GetSkillName();
            GetComponent<StaminaCostIdentifier>().GetComponent<TextMeshProUGUI>().text =
                skillButton.Skill.SkillData.staminaCost.ToString(); 
        }
    }
}
