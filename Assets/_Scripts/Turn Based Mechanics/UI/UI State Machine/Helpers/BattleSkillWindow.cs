using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UI;

public class BattleSkillWindow : MonoBehaviour
{
    public event System.Action<IEnumerator, bool> OnAnimationEndpoint;
    public IEnumerator activeUIAction = null;

    [SerializeField] private float grayoutAlpha;
    [SerializeField] private Transform icon;
    [SerializeField] private Transform ribbon;
    [SerializeField] private Transform ribbon2;
    [SerializeField] private Transform display;
    [SerializeField] private VerticalLayoutGroup skillLayout;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private GameObject buttonPrefab;

    public List<SkillAction> skills { get; private set; }
    private List<SkillSelectButton> skillButtons;
    public int activeIndex { get; private set; }
    
    private bool tooltipActive = false;
    // Start is called before the first frame update
    void Start()
    {
        skills = new List<SkillAction>();
        skillButtons = new List<SkillSelectButton>();
        QuickDisable(); 
    }

    public void QuickDisable() {
        icon.DOScale(0f, 0f);
        ribbon.DOScaleX(0f, 0f);
        ribbon2.DOScaleX(0f, 0f);
        display.DOScaleY(0f, 0f);
        ClearButtons();
    }

    private void ClearButtons() {
        foreach (SkillSelectButton button in skillButtons) {
            Destroy(button.gameObject);
        }
        skillButtons.Clear();
        skills.Clear();
    }

    public void Initialize(float startDelay, CharacterActor actor) {
        foreach (SkillAction skill in actor.SkillList) {
            skills.Add(skill);
        }
        ToggleMainDisplay(true);
    }

    public void ToggleMainDisplay(bool enable) {
        if (activeUIAction == null) {
            activeUIAction = enable ? EnableAnimation() : DisableAnimation();
            OnAnimationEndpoint?.Invoke(activeUIAction, true);
            StartCoroutine(activeUIAction);
        }
    }

    private IEnumerator EnableAnimation() {
        icon.DOScale(1f, animationDuration);
        icon.DORotate(new Vector3(0, 0, 750), 2f, RotateMode.FastBeyond360).SetEase(Ease.InOutBack);
        yield return new WaitForSeconds(animationDuration);
        ribbon.DOScaleX(1f, 1f).SetEase(Ease.OutBounce);
        ribbon2.DOScaleX(1f, 1.5f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(animationDuration);
        display.DOScaleY(1f, animationDuration);

        foreach (SkillAction skillObject in skills) {
            GameObject obj = Instantiate(buttonPrefab, skillLayout.transform);
            obj.transform.DOScaleX(0f, 0f);
            SkillSelectButton button = obj.GetComponent<SkillSelectButton>();
            skillButtons.Add(button);
            button.Initialize(skillObject);

            bool insufficientStamina = BattleStateMachine.Instance.CurrInput.ActiveActor().Stamina <= skills[activeIndex].SkillData.staminaCost;
            UILogicUtils.SetupButton(obj, insufficientStamina, grayoutAlpha);
            
            obj.transform.DOScaleX(1f, animationDuration);
            yield return new WaitForSeconds(animationDuration / 2);
        }
        var action = activeUIAction;
        activeUIAction = null;
        ButtonSelect(true);

        OnAnimationEndpoint?.Invoke(action, false);
    }

    private IEnumerator DisableAnimation() {
        //ADD ACTUAL EXIT ANIMATION
        ClearButtons();
        ribbon.DOScaleX(0f, 1f).SetEase(Ease.OutBounce);
        ribbon2.DOScaleX(0f, 1.5f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(animationDuration / 2);
        icon.DOScale(0f, animationDuration);
        display.DOScaleY(0f, animationDuration);

        var action = activeUIAction;
        activeUIAction = null;

        OnAnimationEndpoint?.Invoke(action, false);
    }

    public void ButtonSelect(bool downwards) {
        if (activeUIAction == null) {
            activeUIAction = ButtonSelectAction(downwards);
            OnAnimationEndpoint?.Invoke(activeUIAction, true);
            StartCoroutine(activeUIAction);
        }
    }

    private IEnumerator ButtonSelectAction(bool downwards) {
        if (activeIndex == -1) {
            activeIndex = 0;
        } else if (downwards) {
            if (activeIndex >= skillButtons.Count - 1) activeIndex = 0;
            else activeIndex++;
        }
        else {
            if (activeIndex <= 0) activeIndex = skillButtons.Count - 1;
            else activeIndex--;
        }

        for (int i = 0; i < skillButtons.Count; i++) {
            if (i == activeIndex) {
                skillButtons[activeIndex].Select(animationDuration / 2);
            }
            else {
                skillButtons[i].Deselect(animationDuration / 2);
            }
        }

        yield return new WaitForSeconds(animationDuration / 2);

        var action = activeUIAction;
        activeUIAction = null;

        OnAnimationEndpoint?.Invoke(action, false);
    }

    public SkillAction ConfirmSkill() {
        return skillButtons[activeIndex].Confirm();
    }
}
