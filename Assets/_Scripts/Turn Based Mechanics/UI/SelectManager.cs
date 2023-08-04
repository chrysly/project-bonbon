using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour {

    [SerializeField] private BattleStateSystem state;
    [SerializeField] private CameraController cameraController;

    public delegate void Deselect();
    public event Deselect OnDeselect;

    public delegate void Select(CharacterActor actor);
    public event Select OnSelect;

    private CharacterActor selectedActor;

    [SerializeField] private bool isActive = false;

    private void Start() {
        //TODO: REGISTER EVENT FOR SWITCHING STATES, DEACTIVATE DURING ANIMATION STATE
        cameraController.OnSwitchView += FireDeselectEvent;
        state.OnSwitchState += EnableSelect;
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            OnDeselect.Invoke();
            selectedActor = null;
        }

        if (isActive) {
            Transform actor = ClickedCharacter();
            if (actor != null) {
                CharacterActor selected = actor.GetComponent<CharacterActor>();
                if (selectedActor == null || !selectedActor.Equals(selected)) {
                    OnSelect.Invoke(selected);
                    selectedActor = selected;
                }
            }
        }
    }

    private void EnableSelect(bool isEnabled) {
        isActive = isEnabled;
        if (!isEnabled) {
            selectedActor = null;
            OnDeselect.Invoke();
        }
    }

    private void FireDeselectEvent(bool isAerial) {
        OnDeselect.Invoke();
        isActive = !isAerial;
    }

    private Transform ClickedCharacter() {
        if (Input.GetMouseButton(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100)) {
                Transform target = hit.transform;
                if (target.GetComponent<CharacterActor>() != null) {
                    return target;
                }
            }
        }
        return null;
    }
}
