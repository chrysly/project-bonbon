using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{

    public TMPro.TMP_Text QuoteText;

    private List<string> listQuotes;
    private List<string> originalQuotes;
    private ArrayList Quotes;
    private float fadeTime;
    private bool fadingIn;

    // Start is called before the first frame update
    void Start()
    {
        var rand = new System.Random();
        //Put list of quotes here!
        QuoteText.CrossFadeAlpha(0, 0f, false);
        listQuotes = new List<string>();
        //
        listQuotes.Add("Hello! I am a quote!");
        listQuotes.Add("WHOSE QUOTES ARE THESE?");
        originalQuotes = new List<string>(listQuotes);
        fadingIn = true;
        fadeTime = 0;
        int randIndex = rand.Next(listQuotes.Count);
        string randomText = (string) listQuotes[randIndex];
        QuoteText.text = randomText;
        listQuotes.Remove(randomText);
    }

    // Update is called once per frame
    void Update()
    {
        print(listQuotes.Count);
        if (fadingIn)
        {
            fadeIn();
        }
        else if (fadingIn == false)
        {
            // Change the second argument for the speed of the animation
            QuoteText.CrossFadeAlpha(0, 0.5f, false);
            fadeTime += Time.deltaTime;
            if (fadeTime > 1)
            {
                if (listQuotes.Count == 0) {
                    /* Replacing empty list with a copy of the original listQuotes
                     * I'm not sure if there's a more efficient way to implement this.
                     * O(n)
                    */
                    listQuotes = new List<string>(originalQuotes);
                }
                var rand = new System.Random();
                int randIndex = rand.Next(listQuotes.Count);
                string randomText = (string)listQuotes[randIndex];
                QuoteText.text = randomText;
                listQuotes.Remove(randomText);
                fadingIn = true;
                fadeTime = 0;
            }
        }
    }

    // Method for text fading in
    void fadeIn()
    {
        // Change the second argument for the speed of the animation
        QuoteText.CrossFadeAlpha(1, 0.5f, false);
        fadeTime += Time.deltaTime;
        if (fadeTime > 1)
        {
            fadingIn = false;
            fadeTime = 0;
        }
    }
}
