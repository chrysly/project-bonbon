using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestForMesh : MonoBehaviour {
    [SerializeField] private MeshFilter grassObject;

    private TextMeshProUGUI text;
    void Start() {
        text = GetComponentInChildren<TextMeshProUGUI>(true);
    }

    private void Update() {
        IsValidMeshFilter();
    }

    private void IsValidMeshFilter() {
        if (grassObject == null) {
            text.text = "No mesh filter somehow AAAAAAAA";
        } else if (grassObject.mesh == null) {
            text.text = "GrassObject filter mesh is null AAAAAAAA";
        }
        else {
            text.text = "No issues I think?? AAAAAAAA";
        }
    }
}
