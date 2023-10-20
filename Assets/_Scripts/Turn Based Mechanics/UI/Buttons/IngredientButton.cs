using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientButton : BattleButton
{
    [SerializeField] private TextMeshProUGUI bonbonText;
    [SerializeField] private RawImage image;

    private BonbonBlueprint _blueprint;
    
    public void Initialize(BonbonBlueprint blueprint) {
        _blueprint = blueprint;
        bonbonText.SetText(_blueprint.name);
        image.texture = blueprint.texture;
    }
    
    public void Select(float delay) {
        Scale(new Vector3(1.1f, 1.1f, 1.1f), delay);
    }

    public void Deselect(float delay) {
        Scale(new Vector3(1, 1, 1), delay);
    }

    public BonbonBlueprint Confirm() {
        return _blueprint;
    }
}
