using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class skill_Dropdown : MonoBehaviour
{
    Dropdown skillDropdown;
    // Start is called before the first frame update
    void Start()
    {
        skillDropdown = GetComponent<Dropdown>();
        List<string> skillList = new List<string>();
        //foreach (var skill in collection)
        //{
        //    skillList.Add(skill);
        //}
        skillDropdown.AddOptions(skillList);
        skillDropdown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(skillDropdown);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DropdownValueChanged(Dropdown change)
    {
        Debug.Log(change.value);
    }
}
