using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorManager {

    public Actor CheckForSelect() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100)) {
                Transform target = hit.transform;
                if (target.gameObject.GetComponent<Actor>() != null) {
                    return target.gameObject.GetComponent<Actor>();
                }
            }
        }
        return null;
    }
}
