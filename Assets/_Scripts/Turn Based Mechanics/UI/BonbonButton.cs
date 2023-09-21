using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BonbonButton : MonoBehaviour
{
    [SerializeField] private BonbonBlueprint _bonbonObject;

    public void AssignBonbon(BonbonBlueprint bonbonObject) {
        this._bonbonObject = bonbonObject;
        UpdateText();
    }

    public BonbonBlueprint GetBonbon() {
        return _bonbonObject;
    }

    private void UpdateText() {
        TextMeshProUGUI text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.SetText(_bonbonObject.name);
    }
}
