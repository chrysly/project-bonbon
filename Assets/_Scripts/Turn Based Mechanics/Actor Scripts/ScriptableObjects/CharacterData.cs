using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Actor Data/Character")]
public class CharacterData : ActorData {

    public SkillObject[][] skillMap;
    public BonbonObject[][] bonbonMap;
}

#if UNITY_EDITOR



#endif
