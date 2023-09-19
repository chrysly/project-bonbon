using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObject : MonoBehaviour {
    public Sprite sprite;
    public RectTransform transform;

    private void Start() {
        transform = GetComponent<RectTransform>();
    }
}