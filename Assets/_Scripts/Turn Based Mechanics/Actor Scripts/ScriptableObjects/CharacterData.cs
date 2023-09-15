using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Actor Data/Character")]
public class CharacterData : ActorData {

    [SerializeField] private SkillObject[][] skillMap;
}

#if UNITY_EDITOR



#endif
