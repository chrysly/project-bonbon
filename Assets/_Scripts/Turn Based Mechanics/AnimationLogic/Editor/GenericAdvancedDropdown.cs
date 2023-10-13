using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public class AdvancedDropdown<T> : AdvancedDropdown {

    private System.Action<T> onItemSelected;
    private System.Func<T, string> nameExtractor;
    private readonly T[] itemArr;
    private Dictionary<string, T> itemMap;

    public AdvancedDropdown(T[] itemArr, System.Func<T, string> nameExtractor, System.Action<T> onItemSelected)
            : base(new AdvancedDropdownState()) {
        minimumSize = new Vector2(150, 0);
        this.itemArr = itemArr;
        this.nameExtractor = nameExtractor;
        this.onItemSelected = onItemSelected;
    }

    protected override AdvancedDropdownItem BuildRoot() {
        AdvancedDropdownItem root;
        if (itemArr == null || itemArr.Length == 0) {
            root = new AdvancedDropdownItem("No Items Found");
        } else {
            root = new AdvancedDropdownItem("");
            itemMap = new Dictionary<string, T>();
            for (int i = 0; i < itemArr.Length; i++) {
                string itemName = nameExtractor.Invoke(itemArr[i]);
                root.AddChild(new AdvancedDropdownItem(itemName));
                itemMap[itemName] = itemArr[i];
            }
        } return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item) {
        onItemSelected(itemMap[item.name]);
    }
}
