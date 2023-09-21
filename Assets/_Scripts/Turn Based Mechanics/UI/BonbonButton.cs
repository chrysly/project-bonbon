using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BonbonButton : MonoBehaviour
{
    [SerializeField] private BonbonObject _bonbonObject;

    public void AssignBonbon(BonbonObject bonbonObject) {
        this._bonbonObject = bonbonObject;
        UpdateText();
    }

    public BonbonObject RetrieveBonbon() {
        return _bonbonObject;
    }

    private void UpdateText() {
        TextMeshProUGUI text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.SetText(_bonbonObject.Name);
    }
}
