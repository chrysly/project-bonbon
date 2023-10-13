using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Yarn.Unity;
using UnityTimer;
using System.Collections.ObjectModel;

public class DialogueManager : MonoBehaviour
{
    //References
    DialogueRunner _dialogueRunner;
    CustomDialogueView _dialogueView;
    YarnProject _yarnProject;

    [Header("References")]
    [SerializeField] EventSequencer _eventSequencer;    // J
    [SerializeField] GameObject _viewport;
    [SerializeField] GameObject _dialogueViewPrefab;
    [SerializeField] GameObject _dialoguePortraitPrefab;
    [SerializeField] RectTransform _dialogueTransform;
    [SerializeField] List<DialogueCharacter> characterList = new();
    
    //Values
    [Space(20)]
    [Header("Values")]
    [SerializeField] List<ActiveDialogueCharacter> activeCharacterList = new();
    [SerializeField] List<ActiveDialogueCharacter> rightCharacterList = new();
    [SerializeField] List<ActiveDialogueCharacter> leftCharacterList = new();
    [SerializeField] List<GameObject> activeDialogueBoxList = new();

    [SerializeField] bool readingDialogue = false;
    [SerializeField] bool tweeningDialogue = false;

    [SerializeField] Color fadeColor;

    LocalizedLine currentLine;
    GameObject currentDialogueBox;
    TextMeshProUGUI currentDialogueBoxText;
    string previousName;

    public static UnityEvent<string> dialogueRequestEvent = new UnityEvent<string>();

    #region Monobehavior
    void Start()
    {
        DOTween.Init();
        _dialogueRunner = GetComponent<DialogueRunner>();
        _yarnProject = _dialogueRunner.yarnProject;
        _dialogueView = (CustomDialogueView) _dialogueRunner.dialogueViews[0];
        _dialogueTransform = GameObject.Find("Dialogue Transform").GetComponent<RectTransform>();
        float transformScale = Mathf.Round(Screen.width * 0.00113122f * 100f)/100f;
        _dialogueTransform.localScale = new Vector3 (transformScale, transformScale, 1);
        LoadCommandList();
        dialogueRequestEvent.AddListener(StartNode);
    }

    void Update()
    {
        //Debug Input
        if (Input.GetKeyDown(KeyCode.Space) && !tweeningDialogue)
        {
            if (readingDialogue)
            {
                _dialogueView.UserRequestedViewAdvancement();
                AdvanceDialogue();
            } 
            //else {
            //    //NOTE: Just invoke this event from another script with the node you want to call to start dialogue.
            //    //dialogueRequestEvent.Invoke("BrookeTestScript1");
            //    dialogueRequestEvent.Invoke("BrookeTestScript2");
            //}
        }
    }
    #endregion

    #region Main Dialogue Processing
    public void StartNode(string node = "BrookeTestScript1")
    {
        if (_dialogueRunner != null) 
        {
            CreateDialogueBox();
            _dialogueRunner.StartDialogue(node);
            ProcessNode();
            _dialogueTransform.DOAnchorPosY(0f, 0.3f)
                .SetEase(Ease.OutCirc)
                .OnStart(() => tweeningDialogue = true)
                .OnComplete(() => tweeningDialogue = false);
        } else
        {
            Debug.LogError("DialogueManager.StartNode(): _dialogueRunner reference is null");
        }
    }
    void ProcessNode()
    {
        readingDialogue = true;
        Debug.Log("DialogueManager.ProcessNode(): Current Node Name: " + _dialogueRunner.CurrentNodeName);
        //get tags and set up characters + character sides
        List<string> nodeTagList = _dialogueRunner.GetTagsForNode(_dialogueRunner.CurrentNodeName).ToList();
        for (int i = 0; i < nodeTagList.Count; i++)
        {
            bool onTheRight = nodeTagList[i].Split(":")[2] == "right" ? true : false;

            activeCharacterList.Add(new ActiveDialogueCharacter( 
                Instantiate(_dialoguePortraitPrefab, _dialogueTransform),
                characterList.Find(x => x.characterName.Contains(nodeTagList[i].Split(":")[0].ToLower())),
                nodeTagList[i].Split(":")[1].ToLower(),
                onTheRight)
            );
            activeCharacterList[i].portraitObject.transform.SetAsFirstSibling();
            if (activeCharacterList[i].onRightSide) 
            {
                rightCharacterList.Add(activeCharacterList[i]);
                if (rightCharacterList.Count > 1)
                {
                    //activeCharacterList[i].portraitObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(300f + (50f * rightCharacterList.Count), 150f, 0f);
                    activeCharacterList[i].portraitObject.GetComponent<Image>().color = fadeColor;
                }
            }
            else
            {
                leftCharacterList.Add(activeCharacterList[i]);
                if (leftCharacterList.Count > 1)
                {
                    Debug.Log("leftCharacterList.Count: " + leftCharacterList.Count);
                    //activeCharacterList[i].portraitObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-300f - (50f * leftCharacterList.Count), 150f, 0f);
                    activeCharacterList[i].portraitObject.GetComponent<Image>().color = fadeColor;
                }
            }

            SetPortraitExpression(nodeTagList[i]);
            SetPortraitSide(activeCharacterList[i].portraitObject, activeCharacterList[i].onRightSide);
        }
        SetupDialogueBox();
        currentDialogueBox.GetComponent<RectTransform>().DOAnchorPosX(0f, 0.3f)
            .SetEase(Ease.OutCubic)
            .OnStart(() => tweeningDialogue = true)
            .OnComplete(() => tweeningDialogue = false);
    }

    public void AdvanceDialogue()
    {
        //Debug.Log("DialogueManager.AdvanceDialogue()");
        if (_dialogueRunner.CurrentNodeName == null || _dialogueRunner.CurrentNodeName == "")
        {
            EndDialogue();
        } else {
            CreateDialogueBox();
            ProcessDialogue();
        }
    }
    void ProcessDialogue()
    {
        //Debug.Log("DialogueManager.ProcessDialogue()");
        //identify speaker from line
        SetupDialogueBox();

        string currentCharacterName = currentLine.CharacterName.Split('_')[0].ToLower();
        SetPortraitExpression(currentLine.CharacterName);
        if (previousName != currentCharacterName)
        {
            MovePortraitToFront(currentLine.CharacterName);
        }
        float increment = 35 + (Regex.Matches(currentDialogueBoxText.text, "<br>").Count * 20);
        for (int i = 0; i < activeDialogueBoxList.Count-1; i++)
        {
            GameObject dialogueViewObj = activeDialogueBoxList[i];
            RectTransform rect = dialogueViewObj.GetComponent<RectTransform>();
            CanvasGroup dialogueViewCG = dialogueViewObj.GetComponent<CanvasGroup>();
            rect.DOAnchorPosY(rect.anchoredPosition.y + increment, 0.29f).SetEase(Ease.OutCirc);
            if (i == 0 && dialogueViewCG.alpha < 0.5f)
            {
                dialogueViewCG.DOFade(0, 0.29f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => {
                        activeDialogueBoxList.Remove(dialogueViewObj);
                        Destroy(dialogueViewObj);
                    });
            } else {
                dialogueViewCG.DOFade(dialogueViewCG.alpha - 0.3f, 0.29f).SetEase(Ease.OutCirc);
            }
        }
        currentDialogueBox.GetComponent<RectTransform>().DOAnchorPosX(0f, 0.3f)
            .SetEase(Ease.OutCubic)
            .OnStart(() => tweeningDialogue = true)
            .OnComplete(() => {
                tweeningDialogue = false;
                previousName = currentCharacterName;
            });
    }
    void EndDialogue()
    {
        //Debug.Log("DialogueManager.EndDialogue()");
        foreach (GameObject o in activeDialogueBoxList)
        {
            CanvasGroup dialogueViewCG = o.GetComponent<CanvasGroup>();
            dialogueViewCG.DOFade(0, 0.2f)
                .SetEase(Ease.OutCirc);
        }
        _dialogueTransform.DOAnchorPosY(-400f, 0.3f)
            .SetEase(Ease.OutCirc)
            .OnStart(() => tweeningDialogue = true)
            .OnComplete(() => {
                tweeningDialogue = false;
                foreach (GameObject o in activeDialogueBoxList)
                {
                    Destroy(o);
                }
                foreach (ActiveDialogueCharacter a in activeCharacterList)
                {
                    Destroy(a.portraitObject);
                }
                activeCharacterList.Clear();
                activeDialogueBoxList.Clear();
                rightCharacterList.Clear();
                leftCharacterList.Clear();
                readingDialogue = false;
                previousName = null;
            });
        _eventSequencer.CheckForEventEnd(); // J
    }
    #endregion

    #region Dialogue Processing Supporting Functions
    void SetPortraitExpression(string characterNameFromYarn)
    {
        //Used when reading dialogue line to select a portrait from a DialogueCharacterObject
        char [] delimiters = {'_', ':'};
        string[] splitInput = characterNameFromYarn.Split(delimiters);
        string name = splitInput[0].ToLower();
        string expression = splitInput.Length > 1 ? splitInput[1].ToLower() : "idle";

        //Debug.Log("SetCharacterExpression(): Running with input: " + characterNameFromYarn + ", translated to: " + characterNameFromYarn.Split(":")[0].Split("_")[0].ToLower());
        ActiveDialogueCharacter adc = activeCharacterList.Find(x => x.dialogueCharacter.characterName.Contains(name));
        //Debug.Log("adc is " + adc == null ? "null" : adc.dialogueCharacter.characterName);
        Image img = adc.portraitObject.GetComponent<Image>();
        img.sprite = adc.dialogueCharacter.portraitList.Find(x => x.expression.Contains(expression)).portrait;
    }
    void SetPortraitExpression(string characterName, string expression)
    {
        //Used when reading dialogue line to select a portrait from a DialogueCharacterObject
        ActiveDialogueCharacter adc = activeCharacterList.Find(x => x.dialogueCharacter.characterName.Contains(characterName.ToLower()));
        Image img = adc.portraitObject.GetComponent<Image>();
        if (expression == null)
        {
            img.sprite = adc.dialogueCharacter.portraitList.Find(x => x.expression.Contains("idle")).portrait;
        } else {
            img.sprite = adc.dialogueCharacter.portraitList.Find(x => x.expression.Contains(expression)).portrait;
        }
    }
    void SetPortraitSide(GameObject portrait, bool onRight = false)
    {
        RectTransform rect = portrait.GetComponent<RectTransform>();
        int index;
        if (onRight)
        {
            index = rightCharacterList.IndexOf(rightCharacterList.Find(x => x.portraitObject == portrait));
        } else {
            index = leftCharacterList.IndexOf(leftCharacterList.Find(x => x.portraitObject == portrait));
        }
        rect.anchoredPosition = new Vector2(onRight ? 300f + (50f * index) : -300f - (50f * index), 150f);
        rect.rotation = Quaternion.Euler(new Vector3(0, onRight ? 0 : -180f, 0));
    }
    void MovePortraitToFront(string characterNameFromYarn)
    {
        char [] delimiters = {'_', ':'};
        string[] splitInput = characterNameFromYarn.Split(delimiters);
        string name = splitInput[0].ToLower();
        ActiveDialogueCharacter adc = activeCharacterList.Find(x => x.dialogueCharacter.characterName.Contains(name.ToLower()));
        adc.portraitObject.transform.SetAsLastSibling();

        if (adc.onRightSide && rightCharacterList.Count > 1 && rightCharacterList.IndexOf(adc) != 0)
        {
            rightCharacterList.Remove(adc);
            rightCharacterList.Insert(0, adc);
            adc.portraitObject.GetComponent<RectTransform>().DOAnchorPosX(300f, 0.29f).SetEase(Ease.OutCubic);
            adc.portraitObject.GetComponent<Image>().color = Color.white;
            for (int i = 1; i < rightCharacterList.Count; i++)
            {
                rightCharacterList[i].portraitObject.transform.SetAsFirstSibling();
                rightCharacterList[i].portraitObject.GetComponent<RectTransform>().DOAnchorPosX(300f + (50f * i), 0.29f);
                rightCharacterList[i].portraitObject.GetComponent<Image>().DOColor(fadeColor, 0.29f).SetEase(Ease.OutCubic);
            }
        } else if (!adc.onRightSide && leftCharacterList.Count > 1 && leftCharacterList.IndexOf(adc) != 0) {
            leftCharacterList.Remove(adc);
            leftCharacterList.Insert(0, adc);
            adc.portraitObject.GetComponent<RectTransform>().DOAnchorPosX(-300f, 0.29f).SetEase(Ease.OutCubic);
            adc.portraitObject.GetComponent<Image>().color = Color.white;
            for (int i = 1; i < leftCharacterList.Count; i++)
            {
                leftCharacterList[i].portraitObject.transform.SetAsFirstSibling();
                leftCharacterList[i].portraitObject.GetComponent<RectTransform>().DOAnchorPosX(-300f - (50f * i), 0.29f);
                leftCharacterList[i].portraitObject.GetComponent<Image>().DOColor(fadeColor, 0.29f).SetEase(Ease.OutCubic);
            }
        }
    }

    void CreateDialogueBox()
    {
        currentDialogueBox = Instantiate(_dialogueViewPrefab, _viewport.transform);
        activeDialogueBoxList.Add(currentDialogueBox);
        currentDialogueBoxText = GetDialogueBoxText(currentDialogueBox);
    }
    void SetupDialogueBox()
    {
        currentLine = _dialogueView.GetCurrentLine();
        currentDialogueBoxText.text = currentLine.TextWithoutCharacterName.Text;
        currentDialogueBoxText.text = AddLineBreaks(currentDialogueBoxText.text);
        ActiveDialogueCharacter currentSpeaker = activeCharacterList.Find(x => x.dialogueCharacter.characterName.Contains(currentLine.CharacterName.Split("_")[0].ToLower()));
        if (currentSpeaker == null)
        {
            string characterName = currentLine.CharacterName.ToLower();
            AddCharacter(characterName);
            currentSpeaker = activeCharacterList.Find(x => x.dialogueCharacter.characterName.Contains(currentLine.CharacterName.Split("_")[0].ToLower()));
        }
        SetDialogueBoxSide(currentDialogueBox, currentSpeaker.onRightSide);
        GameObject textBG = currentDialogueBox.transform.GetChild(0).transform.GetChild(0).gameObject;
        Timer.Register(0.01f, () => { //this timer is needed so that the text content size filter can apply to the bg before it moves behind the text
            textBG.transform.SetParent(currentDialogueBox.transform);
            textBG.transform.SetAsFirstSibling();
            //Debug.Log("Setting color: " + currentSpeaker.dialogueCharacter.dialogueBoxColor);
            textBG.GetComponent<Image>().color = currentSpeaker.dialogueCharacter.dialogueBoxColor;
        });
    }
    void SetDialogueBoxSide(GameObject dialogueBox, bool onRight = false)
    {
        RectTransform rect = dialogueBox.GetComponent<RectTransform>();
        rect.anchorMax = new Vector2 (onRight ? 0.87f : 0.13f, 0);
        rect.anchorMin = new Vector2 (onRight ? 0.87f : 0.13f, 0);
        RectTransform textRect = dialogueBox.transform.GetChild(0).GetComponent<RectTransform>();
        textRect.anchorMax = new Vector2 (onRight ? 1f : 0f, 0.5f);
        textRect.anchorMin = new Vector2 (onRight ? 1f : 0f, 0.5f);
        textRect.pivot = new Vector2(onRight ? 1f : 0f, 0f);
        rect.anchoredPosition = new Vector2(onRight ? 300f : -300f, 15f);
    }
    string AddLineBreaks(string text)
    {
        StringBuilder result = new StringBuilder(text);
        int count = 0;
        for (int i = 0; i < result.Length - 1; i++)
        {
            if (count >= 40 && result[i] == ' ')
            {
                result.Remove(i, 1);
                result.Insert(i, "<br>");
                count = 0;
            } else {
                count++;
            }
        }
        return result.ToString();
    }
    TextMeshProUGUI GetDialogueBoxText(GameObject dialogueBox)
    {
        return dialogueBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    
    #endregion

    #region Command Handlers
    void LoadCommandList()
    {
        _dialogueRunner.AddCommandHandler<string, bool>("add_char", (s, b) => AddCharacter(s, b));
        _dialogueRunner.AddCommandHandler<string>("remove_char", (s) => RemoveCharacter(s));
        _dialogueRunner.AddCommandHandler<string, bool>("set_side", (s, b) => SetCharacterSide(s, b));
    }
    void AddCharacter(string nameInput, bool? isOnRight = null) //assumes you're adding the
    {
        string expression = nameInput.Split("_").Length > 1 ? nameInput.Split("_")[1].ToLower() : "idle";
        DialogueCharacter dialogueCharacter = characterList.Find(x => x.characterName.Contains(nameInput.Split("_")[0].ToLower()));
        bool sideToSet = isOnRight == null ? dialogueCharacter.defaultToRightSide : (bool) isOnRight;
        if (dialogueCharacter != null)
        {
            activeCharacterList.Add(new ActiveDialogueCharacter( 
                Instantiate(_dialoguePortraitPrefab, _dialogueTransform),
                dialogueCharacter,
                expression,
                sideToSet)
            );
            ActiveDialogueCharacter addedChar = activeCharacterList[^1];
            addedChar.portraitObject.transform.SetAsFirstSibling();
            if (sideToSet == true) 
            {
                rightCharacterList.Add(addedChar);
                if (rightCharacterList.Count > 1) addedChar.portraitObject.GetComponent<Image>().color = fadeColor;
            }
            else
            {
                leftCharacterList.Add(addedChar);
                if (leftCharacterList.Count > 1) addedChar.portraitObject.GetComponent<Image>().color = fadeColor;
            }
            SetPortraitExpression(nameInput);
            SetPortraitSide(addedChar.portraitObject, sideToSet == true);
        } else {
            Debug.LogError("DialogueManager.AddCharacter(): Could not find specified character: " + nameInput.Split("_")[0].ToLower());
        }
    }
    void RemoveCharacter(string nameInput)
    {
        ActiveDialogueCharacter adc = activeCharacterList.Find(x => x.dialogueCharacter.characterName.Contains(nameInput.Split("_")[0].ToLower()));
        if (adc != null)
        {
            adc.portraitObject.GetComponent<RectTransform>().DOAnchorPosY(-150f, 0.28f)
            .SetEase(Ease.OutCirc)
            .OnStart(() => tweeningDialogue = true)
            .OnComplete(() => {
                activeCharacterList.Remove(adc);
                if (adc.onRightSide == true) rightCharacterList.Remove(adc); else leftCharacterList.Remove(adc);
                Destroy(adc.portraitObject);
                tweeningDialogue = false;
            });
        } else {
            Debug.LogError("DialogueManager.RemoveCharacter(): Could not find character to remove! Character name: " + nameInput.Split("_")[0].ToLower());
        }
    }
    void SetCharacterSide(string nameInput, bool isOnRight = false)
    {
        //Locate character in ActiveDialogueCharacterList
    }
    #endregion
}

[Serializable]
class ActiveDialogueCharacter
{
    public ActiveDialogueCharacter(GameObject portrait, DialogueCharacter dialogueCharacter, string expression, bool onRightSide = false)
    {
        this.portraitObject = portrait;
        this.dialogueCharacter = dialogueCharacter;
        this.expression = expression;
        this.onRightSide = onRightSide;
    }

    public GameObject portraitObject;
    public DialogueCharacter dialogueCharacter;
    public string expression;
    public bool onRightSide;
}