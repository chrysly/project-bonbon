using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleSkillWindow : MonoBehaviour
{
    public IEnumerator activeUIAction = null;

    [SerializeField] private Transform icon;
    [SerializeField] private Transform ribbon;
    [SerializeField] private Transform display;

    private List<SkillObject> skills;

    private bool mainDisplayActive = false;
    private bool tooltipActive = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize(float startDelay) {
        if (activeUIAction == null) {
            mainDisplayActive = true;
        }
        //scale in
        //bow scales out + background elements
        //skill list
    }

    //private IEnumerator ToggleMainDisplay() {
        
    //}
    
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
