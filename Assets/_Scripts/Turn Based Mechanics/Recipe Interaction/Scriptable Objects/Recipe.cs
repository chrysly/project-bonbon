using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Recipe : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private string id;
    [SerializeField] private List<Ingredient> ingredientList;

}
