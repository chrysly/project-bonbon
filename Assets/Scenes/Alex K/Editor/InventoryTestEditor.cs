using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(InventoryTest))]
public class InventoryTestEditor : Editor
{
    [SerializeField] private VisualTreeAsset inventoryTestAsset;

    public override VisualElement CreateInspectorGUI() {
        VisualElement element = new VisualElement();
        inventoryTestAsset.CloneTree(element);
        Button bonbonPassButton = element.Query<Button>("passBonbon").First();
        bonbonPassButton.RegisterCallback<ClickEvent>(cevnt => {
            (target as InventoryTest).PassBonbonEvent();
        });
        return element;
    }
}
