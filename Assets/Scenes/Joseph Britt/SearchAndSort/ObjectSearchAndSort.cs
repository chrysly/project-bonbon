using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSearchAndSort : MonoBehaviour {

    /// <summary> An array containing all available objects;
    /// <br></br> Note: The algorithms SHOULD NOT modify this array in any way; </summary>
    [SerializeField] private List<BaseObject> objList;
    [SerializeField] private List<BaseObject> sortedList;
    
    // Start is called before the first frame update
    void Start() {
        // test case
        BaseObject.IngredientType ingredientType = BaseObject.IngredientType.Dough;
        System.Type effectType = new IceEffect().GetType();
        objList = PopulateTestList();

        ObjectSort(ingredientType, effectType);
    }

    /// <summary>
    /// Write your sorting method here!
    /// </summary>
    /// <param name="ingredientType"> Only object whose ingredientType is equal to this IngredientType must be included in the results list; </param>
    /// <param name="effectType"> Only objects whose custom effects list contains an object of this type should be included in the results list; </param>
    /// <returns> A list of objects that meet the right criteria; </returns>
    public void ObjectSort(BaseObject.IngredientType ingredientType = 0, System.Type effectType = null) {
        sortedList = new List<BaseObject>(objList);
        sortedList = ingredientType > 0 ? FilterByIngredient(sortedList, ingredientType) : sortedList;
        sortedList = effectType != null ? FilterByEffect(sortedList, effectType) : sortedList;
        sortedList.Sort();
    }

    private static List<BaseObject> FilterByIngredient(List<BaseObject> objList, BaseObject.IngredientType ingredientType) {
        return objList.FindAll(obj => obj.ingredientType == ingredientType);
    }

    private static List<BaseObject> FilterByEffect(List<BaseObject> objList, System.Type effectType) {
        return objList.FindAll(obj => obj.customEffects.Exists(effect => effect.GetType() == effectType));
    }

    private static List<BaseObject> PopulateTestList() {
        List<BaseObject> testList = new List<BaseObject>();

        testList.Add(new BaseObject("Chocolate | FireEffect") { ingredientType = BaseObject.IngredientType.Chocolate, customEffects = new List<TestEffect>(new TestEffect[] { new FireEffect() }) });
        testList.Add(new BaseObject("Dough | IceEffect") { ingredientType = BaseObject.IngredientType.Dough, customEffects = new List<TestEffect>(new TestEffect[] { new IceEffect() }) });
        testList.Add(new BaseObject("Cream | IceEffect") { ingredientType = BaseObject.IngredientType.Cream, customEffects = new List<TestEffect>(new TestEffect[] { new IceEffect() }) });
        testList.Add(new BaseObject("Chocolate | FireEffect + IceEffect") { ingredientType = BaseObject.IngredientType.Chocolate, customEffects = new List<TestEffect>(new TestEffect[] { new FireEffect(), new IceEffect() }) });

        return testList;
    }
}