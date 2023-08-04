using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraRotate : MonoBehaviour {
    [SerializeField] private float rotateSpeed;
    public void Update() {
        transform.Rotate(new Vector3(0, 1, 0), rotateSpeed * Time.deltaTime);
    }
}
