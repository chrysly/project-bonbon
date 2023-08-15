using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindIKManager : MonoBehaviour {
    public float windStrength = 1f;
    public Vector3 windDirection = new Vector3(1f, 0f, 0f);
    public float intervalTimeInSeconds = 3f;
    public float windStrengthRandom = 0.2f;
}
