using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActor : Actor
{
    private CharacterData _characterData;

    protected override void Start() {
        base.Start();
    }

    protected override void InitializeAttributes() {
        base.InitializeAttributes();
        _characterData = data as CharacterData;
    }

    protected override void InitializeLevelObjects() {
        base.InitializeLevelObjects();

        for (int i = 0; i < GameManager.CurrLevel; i++) {
            /// Load Skills
            foreach (SkillObject skill in _characterData.skillMap[i]) {
                CreateSkillAction(skill);
            }

            /// Load Bonbons
            foreach (BonbonBlueprint bonbon in _characterData.bonbonMap[i]) {
                BonbonList.Add(bonbon);
            }
        }
    }
}
