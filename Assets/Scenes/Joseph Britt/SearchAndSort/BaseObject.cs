using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Below are the dummy classes that make up the Base Object! ///

/// <summary>
/// This is the base object;
/// </summary>
[System.SerializableAttribute]
public class BaseObject {

    public enum IngredientType {
        None,
        Chocolate,
        Cream,
        Dough,
    }

    public string name;
    public IngredientType ingredientType;
    public List<TestEffect> customEffects;

    public BaseObject(string name) {
        this.name = name;
    }
}

/// <summary> Base Test Effect; </summary>
public abstract class TestEffect {

    /// We don't care about the code that goes here;
}

/// <summary> Ice go brrr; </summary>
public class IceEffect : TestEffect {

    /// We don't care about the code that goes here either;
}

/// <summary> Fire go brrr; </summary>
public class FireEffect : TestEffect {

    /// We don't care about the code here still;
}