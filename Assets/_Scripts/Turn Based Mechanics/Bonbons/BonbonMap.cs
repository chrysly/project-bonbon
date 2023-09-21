using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PseudoDataStructures;

[CreateAssetMenu(menuName = "Bonbon/BonbonMap")]
public class BonbonMap : ScriptableObject {

    public ArrayArray<BonbonBlueprint> bonbonMap;
}
