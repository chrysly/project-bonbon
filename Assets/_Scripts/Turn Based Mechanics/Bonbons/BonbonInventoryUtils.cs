using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BonbonInventoryUtils
{
    public static bool PassBonbonBetween(Actor source, Actor target, int sourceInventory, int targetInventory) {
        if (source.BonbonInventory[sourceInventory] == null) {
            Debug.Log($"Source inventory #{sourceInventory} is empty...");
            return false;
        }
        if (target.BonbonInventory[targetInventory] != null) {
            BonbonObject existing = target.BonbonInventory[targetInventory];
            Debug.Log($"Target inventory #{targetInventory} already has a {existing.Name}...");
            return false;
        }

        target.BonbonInventory[targetInventory] = source.BonbonInventory[sourceInventory];
        source.BonbonInventory[sourceInventory] = null;
        return true;
    }
}
