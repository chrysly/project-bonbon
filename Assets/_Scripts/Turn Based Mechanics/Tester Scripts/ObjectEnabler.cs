using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectEnabler : MonoBehaviour
{
    void Activate() {
        gameObject.SetActive(true);
    }

    void Deactivate() {
        gameObject.SetActive(false);
    }
}
