using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow2DAnimation : MonoBehaviour
{
    private RectTransform arrowPicture;
    private float originalYPosition;
    private float yPosition;
    private bool goingDown;
    [SerializeField] private float normalSpeed;
    [SerializeField] private float multiplier;


    /*
     * Makes the arrow move up and down to a particular y-coodinate.
     * 
     * @param finalDownPosition: The final y-position that you want to get to and then
     * return back to original position.
    */
    private void arrowMovement(float finalDownPosition) {
        if (goingDown) {
            arrowPicture.anchoredPosition = new Vector2(arrowPicture.anchoredPosition.x, yPosition - (normalSpeed * multiplier));
            yPosition = arrowPicture.anchoredPosition.y;
            if (yPosition <= finalDownPosition) {
                goingDown = false;
            }
        }
        else {
            arrowPicture.anchoredPosition = new Vector2(arrowPicture.anchoredPosition.x, yPosition + normalSpeed);
            yPosition = arrowPicture.anchoredPosition.y;
            if (yPosition >= originalYPosition) {
                goingDown = true;
            }
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        arrowPicture = GetComponent<RectTransform>();
        originalYPosition = arrowPicture.anchoredPosition.y;
        yPosition = arrowPicture.anchoredPosition.y;
        goingDown = true;
    }

    // Update is called once per frame
    void Update()
    {
        arrowMovement(-200);
    }
}
