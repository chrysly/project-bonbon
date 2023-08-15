using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour, IComparable<Actor>
{
    #region Data Attributes
    [SerializeField] 
    #endregion Data Attributes
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public int CompareTo(Actor actor) {
        return 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
