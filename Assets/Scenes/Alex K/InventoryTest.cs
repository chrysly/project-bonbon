using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    [SerializeField] private Actor source;
    [SerializeField] private Actor target;
    [SerializeField] private int sourceInventory;
    [SerializeField] private int targetInventory;
    [SerializeField] private BonbonRadialWindow window;

    public void PassBonbonEvent() {
        if (BonbonInventoryUtils.PassBonbonBetween(source, target, sourceInventory, targetInventory)) {
            window.UpdateSlots();
        }
    }
}
