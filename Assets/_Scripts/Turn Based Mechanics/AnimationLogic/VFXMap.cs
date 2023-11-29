using System.Collections.Generic;
using UnityEngine;
using PseudoDataStructures;

public enum GenericVFXType { Damage, Heal, StaminaRegen, StatIncrease, Death }

public class VFXMap : ScriptableObject {

    [SerializeField] private PseudoDictionary<GenericVFXType, VFXAnimationPackage> pseudoDict;
    public Dictionary<GenericVFXType, VFXAnimationPackage> GenericVFXDict => pseudoDict.ToDictionary();
}