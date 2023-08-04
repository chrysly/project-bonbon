using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeCollection : ScriptableObject
{
    [SerializeField] private List<GradeOneRecipe> gradeOneRecipes;
    [SerializeField] private List<GradeTwoRecipe> gradeTwoRecipes;
    [SerializeField] private List<SpecialGradeRecipe> specialGradeRecipes;
}
