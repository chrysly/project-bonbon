using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IngredientButton : MonoBehaviour
{
    [SerializeField] private Ingredient ingredient;

    public void AssignIngredient(Ingredient ingredient) {
        this.ingredient = ingredient;
        UpdateText();
    }

    public Ingredient RetrieveIngredient() {
        return ingredient;
    }

    private void UpdateText() {
        TextMeshProUGUI text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        text.SetText(ingredient.name);
    }
}
