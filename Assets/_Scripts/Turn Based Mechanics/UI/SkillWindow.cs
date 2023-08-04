using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class SkillWindow : MonoBehaviour
{
    [SerializeField] private float expandDuration = 0.5f;
    [SerializeField] private float contractDuration = 0.5f;
    [SerializeField] private float buttonLoadDuration;
    [SerializeField] private CharacterActor actor;
    [SerializeField] private Transform buttonContainer;

    [SerializeField] private BattleStateSystem battleState;

    [SerializeField] private GameObject buttonPrefab;

    //TODO: Remove after refactoring selection script
    [SerializeField] private CanvasGroup panel;

    private List<SkillObject> skills;

    private void Start() {
        
        skills = new List<SkillObject>(actor.Data().SkillList());
        transform.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
        LoadButtons();
        battleState.OnSkillConfirm += DisplayOnConfirm;
    }

    public void Display() {
        gameObject.SetActive(true);
        transform.DOScaleY(1, expandDuration);
    }

    private void DisplayOnConfirm(bool canceled) {
        panel.alpha = 1;
    }

    public void Hide() {
        StartCoroutine(HideAction());
    }

    private IEnumerator HideAction() {
        transform.DOScaleY(0, contractDuration);

        yield return new WaitForSeconds(contractDuration);

        gameObject.SetActive(false);
        panel.alpha = 0;    //POTATO
    }

    private void LoadButtons() {
        foreach (SkillObject skill in skills) {
            GameObject button = (GameObject) Instantiate(buttonPrefab, buttonContainer);
            SkillButton skillButton = button.GetComponent<SkillButton>();
            skillButton.AssignSkill(skill);
            Button btn = button.GetComponent<Button>();
            btn.onClick.AddListener(delegate { battleState.SwitchToSkillSelect(skillButton.RetrieveSkill()); });
            btn.onClick.AddListener(delegate { Hide(); });
        }
    }
}
