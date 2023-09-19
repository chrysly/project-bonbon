using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FakeInventory : MonoBehaviour
{
    [SerializeField] private int slotWidth = 100;
    [SerializeField] private GameObject slotPrefab;
    private RandomObject[] inventorySlots;
    private RectTransform rt;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        inventorySlots = GetComponents<RandomObject>();
        //GetComponentsInChildren<RandomObject>()
        
        int counter = 0;
        foreach (RandomObject item in inventorySlots)
        {
            if (item != null) {
                
                //creates slot and sets transform to this current transform location
                GameObject itemSlot = Instantiate(slotPrefab);
                RectTransform transform = itemSlot.GetComponent<RectTransform>();
                transform.sizeDelta = new Vector2(slotWidth, slotWidth);
                transform.position = rt.transform.position;
                transform.SetParent(rt);

                //displays image
                Image spriteImage = itemSlot.AddComponent<Image>();
                spriteImage.sprite = item.sprite;

                // adjust x val of next slot based on slot width
                Vector3 newPosition = transform.position + new Vector3(counter * slotWidth, 0f, 0f);
                itemSlot.transform.position = newPosition;
            }
            counter++;
        }
    }
}