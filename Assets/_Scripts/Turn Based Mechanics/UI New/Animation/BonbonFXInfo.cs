using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleUI {
    public abstract class BonbonFXInfo { public int animationSlot; }

    public class BonbonCraftInfo : BonbonFXInfo {

        public BonbonObject bonbon;

        public BonbonCraftInfo(int animationSlot, BonbonObject bonbon) {
            this.animationSlot = animationSlot;
            this.bonbon = bonbon;
        }
    }

    public class BonbonBakeInfo : BonbonFXInfo {

        public BonbonObject[] ingredients;
        public BonbonBlueprint result;

        public BonbonBakeInfo(int animationSlot, 
                              BonbonObject[] ingredients, BonbonBlueprint result) {
            this.animationSlot = animationSlot;
            this.ingredients = ingredients;
            this.result = result;
        }
    }
}