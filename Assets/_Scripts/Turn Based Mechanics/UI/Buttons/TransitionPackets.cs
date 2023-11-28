using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {
    public abstract class BaseTransitionInfo { }

    public class BonbonTransitionInfo : BaseTransitionInfo { 
        public int slot;

        public BonbonTransitionInfo() { }
        public BonbonTransitionInfo(int slot) => this.slot = slot;
    }

    public class SkillTransitionInfo : BaseTransitionInfo {

        private ActiveSkillPrep skillPrep;
        public ActiveSkillPrep SkillPrep {
            get {
                if (skillPrep == null) skillPrep = new ActiveSkillPrep();
                return skillPrep;
            }
        }

        public SkillTransitionInfo() { }
        public SkillTransitionInfo(BonbonObject bonbon) => SkillPrep.bonbon = bonbon;
        public SkillTransitionInfo(SkillTransitionInfo info) {
            this.skillPrep = info.skillPrep;
        }

        public SkillAction Skill { get => SkillPrep.skill; set => SkillPrep.skill = value; }
        public BonbonObject Bonbon { get => SkillPrep.bonbon; set => SkillPrep.bonbon = value; }

        public SkillTransitionInfo ExpandWith(SkillAction skill) {
            var info = new SkillTransitionInfo(this);
            info.Skill = skill;
            return info;
        }
    }
}
