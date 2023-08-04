using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class IngredientWindow : MonoBehaviour
{
    [SerializeField] private float expandDuration = 0.5f;
    [SerializeField] private float contractDuration = 0.5f;
    [SerializeField] private float buttonLoadDuration;
    [SerializeField] private CharacterActor actor;
    [SerializeField] private Transform buttonContainer;

    [SerializeField] private BattleStateSystem battleState;
    [SerializeField] private GameObject buttonPrefab;

    [SerializeField] private CanvasGroup panel;

    private List<Ingredient> ingredients;

    private void Start() {

        ingredients = new List<Ingredient>(actor.Data().IngredientList());
        transform.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
        LoadButtons();
        //battleState.OnSkillConfirm += DisplayOnConfirm;
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
        foreach (Ingredient ingredient in ingredients) {
            GameObject button = (GameObject)Instantiate(buttonPrefab, buttonContainer);
            IngredientButton ingredientButton = button.GetComponent<IngredientButton>();
            ingredientButton.AssignIngredient(ingredient);
            Button btn = button.GetComponent<Button>();
            //btn.onClick.AddListener(delegate { battleState.SwitchToSkillSelect(.RetrieveSkill()); });
            //btn.onClick.AddListener(delegate { Hide(); });
        }
    }
}
