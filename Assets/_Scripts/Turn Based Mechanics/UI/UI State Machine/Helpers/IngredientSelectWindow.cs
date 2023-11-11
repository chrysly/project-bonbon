using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class IngredientSelectWindow : MonoBehaviour
{
    public event System.Action<IEnumerator, bool> OnAnimationEndpoint;
    public IEnumerator activeUIAction = null;

    [SerializeField] private Transform backdrop;
    [SerializeField] private Transform horizontalView;
    [SerializeField] private Transform ingredientDisplayPoint;
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private float animationDuration = 0.5f;

    private List<IngredientButton> _ingredientButtons;
    private List<BonbonBlueprint> _blueprints;
    private Vector3 _originalPos;

    public int slot = -1;
    
    private int activeIndex = -1;
    
    void Start() {
        _blueprints = new List<BonbonBlueprint>();
        _ingredientButtons = new List<IngredientButton>();
        _originalPos = backdrop.position;
        QuickDisable();
    }
    
    public void QuickDisable() {
        backdrop.DOMove(_originalPos, 0f);
        ClearButtons();
    }
    
    private void ClearButtons() {
        foreach (IngredientButton button in _ingredientButtons) {
            Destroy(button.gameObject);
        }
        _ingredientButtons.Clear();
        _blueprints.Clear();
        slot = -1;
    }
    
    public void Initialize(float startDelay, CharacterActor actor) {
        foreach (BonbonBlueprint bonbon in actor.BonbonList) {
            _blueprints.Add(bonbon);
        }
        ToggleMainDisplay(true);
    }
    
    public void ToggleMainDisplay(bool enable) {
        if (activeUIAction == null) {
            activeUIAction = enable ? EnableAnimation() : DisableAnimation();
            OnAnimationEndpoint.Invoke(activeUIAction, true);
            StartCoroutine(activeUIAction);
        }
    }
    
    private IEnumerator EnableAnimation() {
        backdrop.DOMove(ingredientDisplayPoint.position, .3f);
        yield return new WaitForSeconds(.15f);

        foreach (BonbonBlueprint bonbon in _blueprints) {
            GameObject obj = Instantiate(ingredientPrefab, horizontalView);
            obj.transform.DOScale(0f, 0f);
            IngredientButton button = obj.GetComponent<IngredientButton>();
            _ingredientButtons.Add(button);
            button.Initialize(bonbon);
            obj.transform.DOScale(1f, animationDuration).SetEase(Ease.OutBounce);
            yield return new WaitForSeconds(animationDuration / 2);
        }

        var action = activeUIAction;
        activeUIAction = null;

        OnAnimationEndpoint?.Invoke(action, false);
    }

    private IEnumerator DisableAnimation() {
        //ADD ACTUAL EXIT ANIMATION
        ClearButtons();
        backdrop.DOMove(_originalPos, 0.5f);
        yield return new WaitForSeconds(animationDuration);

        var action = activeUIAction;
        activeUIAction = null;

        OnAnimationEndpoint?.Invoke(action, false);
    }
    
    public void ButtonSelect(bool downwards) {
        if (activeUIAction == null) {
            activeUIAction = ButtonSelectAction(downwards);
            OnAnimationEndpoint.Invoke(activeUIAction, true);
            StartCoroutine(activeUIAction);
        }
    }

    private IEnumerator ButtonSelectAction(bool downwards) {
        if (activeIndex == -1) {
            activeIndex = 0;
        } else if (downwards) {
            if (activeIndex >= _ingredientButtons.Count - 1) activeIndex = 0;
            else activeIndex++;
        }
        else {
            if (activeIndex <= 0) activeIndex = _ingredientButtons.Count - 1;
            else activeIndex--;
        }

        for (int i = 0; i < _ingredientButtons.Count; i++) {
            if (i == activeIndex) {
                _ingredientButtons[activeIndex].Select(animationDuration / 2);
            }
            else {
                _ingredientButtons[i].Deselect(animationDuration / 2);
            }
        }

        yield return new WaitForSeconds(animationDuration / 2);

        var action = activeUIAction;
        activeUIAction = null;

        OnAnimationEndpoint?.Invoke(action, false);
    }
    
    public BonbonBlueprint ConfirmBonbon() {
        return _ingredientButtons[activeIndex].Confirm();
    }
}
