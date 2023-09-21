using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BonbonDisplay : MonoBehaviour {
    public BonbonObject bonbon;
    private Image imageComponent;   //Sprite including container

    private bool _isSelected = false;
    private float _bumpDist = 0.5f;

    private Vector3 oldPosition;
    
    private void Start() {
        imageComponent = GetComponent<Image>();
        oldPosition = transform.position;
    }

    public void UpdateSprite(Sprite sprite) {
        imageComponent.sprite = sprite;
    }

    public void Select() {
        _isSelected = true;
        transform.DOMove(transform.position + new Vector3(-1 * _bumpDist / 2, _bumpDist / 2, -1 * _bumpDist), 0.3f);
    }

    public void MainSelect(Vector3 location) {
        _isSelected = true;
        transform.DOMove(location, 0.3f);
    }

    public void Hide() {
        _isSelected = false;
        imageComponent.DOFade(0.3f, 0.3f);
    }

    public void Show() {
        imageComponent.DOFade(1f, 0.3f);
    }

    public void Deselect() {
        _isSelected = false;
        transform.DOMove(oldPosition, 0.3f);
    }

    public void QuickReset() {
        _isSelected = false;
        transform.DOMove(oldPosition, 0.1f);
    }
}
