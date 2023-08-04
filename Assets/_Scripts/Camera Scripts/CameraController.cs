using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour {
    [SerializeField] private SelectManager selector;

    [SerializeField] private CinemachineVirtualCamera orbitalCam;
    [SerializeField] private CinemachineVirtualCamera aerialCam;
    [SerializeField] private CinemachineVirtualCamera charCam;

    [SerializeField] private GameObject AerialUI;

    private Transform focus = null;

    public delegate void SwitchView(bool isAerial);
    public event SwitchView OnSwitchView;

    private bool isAerial = false;

    private void Start() {
        charCam.gameObject.SetActive(true);
        //aerialCam.gameObject.SetActive(false);
        AerialUI.SetActive(false);
        RegisterEvents();
    }

    private void RegisterEvents() {
        selector.OnSelect += FocusCamera;
        selector.OnDeselect += ClearFocus;
    }

    void Update()
    {
        if (focus != null) {
            charCam.m_Follow = focus.transform.GetChild(0);
        } else {
            charCam.m_Follow = null;    //Does not follow anything
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (isAerial) {
                //charCam.gameObject.SetActive(true);
                //aerialCam.gameObject.SetActive(false);
                isAerial = false;
                AerialUI.SetActive(false);
                OnSwitchView.Invoke(false);
                PrioritizeCamera(orbitalCam);
            } else {
                /*if (focus != null) {
                    selector.setFadeOut(true);
                    FunctionTimer.Create(selector.deselectUI, .02f);
                    selector.StartCoroutine(selector.setSelected(null));
                }*/
                isAerial = true;
                OnSwitchView.Invoke(true);
                AerialUI.SetActive(true);
                //aerialCam.gameObject.SetActive(true);
                //charCam.gameObject.SetActive(false);
                PrioritizeCamera(aerialCam);
            }
        }
    }

    private void FocusCamera(CharacterActor actor) {
        focus = actor.transform;
        PrioritizeCamera(charCam);
    }
    
    private void ClearFocus() {
        focus = null;
        if (!isAerial) {
            PrioritizeCamera(orbitalCam);
        }
        Debug.Log("focus cleared");
    }
    
    public bool getIsCharCam() {
        return charCam.isActiveAndEnabled;
    }

    public void PrioritizeCamera(CinemachineVirtualCamera camera) {
        orbitalCam.Priority = 0;
        aerialCam.Priority = 0;
        charCam.Priority = 0;

        camera.Priority = 1;
    }
}
