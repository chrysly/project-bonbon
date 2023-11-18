using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpLogic : MonoBehaviour {
    public List<Sprite> imageList = new List<Sprite>();
    private int currIndex = 0;
    private SpriteRenderer spriteRenderer;
    public bool isActive = false;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();

        startPopUp(imageList);
    }

    void Update() {
        if (isActive) {
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                CycleImages(1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                CycleImages(-1);
            }
        }
    }

    public void startPopUp(List<Sprite> popUps) {
        isActive = true;

        imageList = popUps;

        if (imageList.Count > 0) {
            spriteRenderer.sprite = imageList[currIndex];
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
        spriteRenderer.sprite = imageList[currIndex];
    }
}
