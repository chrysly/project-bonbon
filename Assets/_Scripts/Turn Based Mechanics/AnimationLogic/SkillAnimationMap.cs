using UnityEngine;
using PseudoDataStructures;

public class SkillAnimationMap : ScriptableObject {
    public PseudoDictionary<SkillObject, PseudoDictionary<ActorData, SkillAnimation>> animationMap;
}