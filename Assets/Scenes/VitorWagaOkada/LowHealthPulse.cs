using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LowHealthPulse : MonoBehaviour
{
    int currentHP;
    public Slider slider;
    public Image fillImage;

    public void removeHP() {
        currentHP = currentHP - 10;
        slider.value = currentHP;
    }

    public void addHP()
    {
        if (currentHP < 100)
        {
            currentHP = currentHP + 10;
            slider.value = currentHP;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHP = 100;
        fillImage.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHP <= 30) {
            fillImage.color = Color.Lerp(Color.red, Color.black, Mathf.PingPong(Time.time, 0.5F));
        } else { 
            fillImage.color = Color.green;
        }
    }
}
