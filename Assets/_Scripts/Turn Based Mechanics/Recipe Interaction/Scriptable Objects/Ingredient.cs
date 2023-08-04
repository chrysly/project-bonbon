using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Ingredient : ScriptableObject
{
    [Header("Ingredient Properties")]
    [SerializeField] private string displayName;
    [SerializeField] private string id;
    [SerializeField] private IngredientType ingredientType;
    [SerializeField] private int staminaCost;

}
