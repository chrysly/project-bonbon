using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActor : Actor
{
    private new CharacterData data;

    protected override void Start() {
        base.Start();
    }

    protected override void InitializeAttributes() {
        base.InitializeAttributes();
    }

    protected override void InitializeLevelObjects() {
        base.InitializeLevelObjects();

        for (int i = 0; i < GameManager.CurrLevel; i++) {
            /// Load Skills
            foreach (SkillObject skill in data.skillMap[i]) {
                CreateSkillAction(skill);
            }

            /// Load Bonbons
            foreach (BonbonObject bonbon in data.bonbonMap[i]) {
                BonbonList.Add(bonbon);
            }
        }
    }
}
