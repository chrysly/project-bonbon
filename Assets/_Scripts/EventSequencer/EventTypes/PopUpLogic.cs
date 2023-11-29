using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpLogic : MonoBehaviour {
    public List<Sprite> imageList = new List<Sprite>();
    private int currIndex = 0;
    private Image img;
    private bool isActive = false;
    private BattleStateMachine bsm => BattleStateMachine.Instance;

    void Start() {
        img = GetComponent<Image>();
    }

    void Update() {
        if (isActive) {
            //bsm.ToggleMachine(true);

            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                CycleImages(1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                CycleImages(-1);
            } else if (Input.GetKeyDown(KeyCode.Escape)) {
                img.enabled = false;
                //bsm.ToggleMachine(false);
                isActive = false;
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void startPopUp(List<Sprite> popUps) {
        isActive = true;

        imageList = popUps;

        if (imageList.Count > 0) {
            img.sprite = imageList[currIndex];
        }
    }

    private void CycleImages(int increment) {
        if (imageList.Count == 0) {
            Debug.Log("No images in list");
            return;
        }

        currIndex += increment;

        // ensure looping through images
        if (currIndex < 0) {
            currIndex = imageList.Count - 1;
        }
        else if (currIndex >= imageList.Count) {
            currIndex = 0;
        }

        // display the current index
        img.sprite = imageList[currIndex];
    }
}
