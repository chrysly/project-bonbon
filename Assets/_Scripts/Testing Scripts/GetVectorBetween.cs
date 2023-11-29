using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetVectorBetween : MonoBehaviour {
    [SerializeField] private Transform lookTarget;

    [SerializeField] private Transform followTarget;

    public Vector3 difference;

    public void CalculateVector() {
        if (lookTarget != null && followTarget != null) {
            difference = lookTarget.position - followTarget.position;
        }
    }
}
