using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Yarn.Unity;
using UnityTimer;
using System;
using System.Linq;
using TMPro;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    //References
    DialogueRunner _dialogueRunner;
    CustomDialogueView _dialogueView;
    YarnProject _yarnProject;

    [SerializeField] GameObject _viewport;
    [SerializeField] GameObject _dialogueViewPrefab;
    [SerializeField] GameObject _dialoguePortraitPrefab;
    [SerializeField] RectTransform _dialogueTransform;
    [SerializeField] List<DialogueCharacter> characterList = new();
    
    //Values
    [SerializeField] List<ActiveDialogueCharacter> activeCharacterList = new();
    [SerializeField] List<GameObject> activeDialogueBoxList = new();

    [SerializeField] bool readingDialogue = false;
    [SerializeField] bool tweeningDialogue = false;

    LocalizedLine currentLine;
    GameObject currentDialogueBox;
    TextMeshProUGUI currentDialogueBoxText;

    public static UnityEvent<string> dialogueRequestEvent;

    #region Monobehavior
    void Start()
    {
        DOTween.Init();
        _dialogueRunner = GetComponent<DialogueRunner>();
        _yarnProject = _dialogueRunner.yarnProject;
        _dialogueView = (CustomDialogueView) _dialogueRunner.dialogueViews[0];
        _dialogueTransform = GameObject.Find("Dialogue Transform").GetComponent<RectTransform>();
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
            } else {
                //StartNode();
                //NOTE: Just invoke this event from another script with the node you want to call to start dialogue.
                dialogueRequestEvent.Invoke("BrookeTestScript1");
            }
        }
    }
    #endregion

    #region Main Dialogue Processing
    public void StartNode(string node = "BrookeTestScript1")
    {
        //endOfNode = false;
        //if (_lineView != null) _lineView.canvasGroupEnabled = true;
        if (_dialogueRunner != null) 
        {
            CreateDialogueBox();
            _dialogueRunner.StartDialogue(node);
            ProcessNode();
            _dialogueTransform.DOAnchorPosY(0f, 0.3f)
                .SetEase(Ease.OutCubic)
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
                characterList.Find(x => x.characterName.Contains(nodeTagList[i].Split(":")[0])),
                nodeTagList[i].Split(":")[1],
                onTheRight)
            );
        }
        SetupDialogueBox();
        currentDialogueBox.GetComponent<RectTransform>().DOAnchorPosX(0f, 0.3f)
            .SetEase(Ease.OutCubic)
            .OnStart(() => tweeningDialogue = true)
            .OnComplete(() => tweeningDialogue = false);

        //AdvanceDialogue();
    }

    public void AdvanceDialogue()
    {
        Debug.Log("DialogueManager.AdvanceDialogue()");
        if (_dialogueRunner.CurrentNodeName == null || _dialogueRunner.CurrentNodeName == "")
        {
            EndDialogue();
        } else {
            CreateDialogueBox();
            //_dialogueView.requestInterrupt();
            //_dialogueView.UserRequestedViewAdvancement();
            ProcessDialogue();
        }
    }
    void ProcessDialogue()
    {
        Debug.Log("DialogueManager.ProcessDialogue()");
        //identify speaker from line
        SetupDialogueBox();

        //activeCharacterList.Add(new ActiveDialogueCharacter);
        float increment = 35 + (Regex.Matches(currentDialogueBoxText.text, "<br>").Count * 20);
        Debug.Log("increment: " + increment);
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
            .OnComplete(() => tweeningDialogue = false);
    }
    void EndDialogue()
    {
        Debug.Log("DialogueManager.EndDialogue()");
        _dialogueTransform.DOAnchorPosY(-300f, 0.3f)
            .SetEase(Ease.OutCubic)
            .OnStart(() => tweeningDialogue = true)
            .OnComplete(() => {
                tweeningDialogue = false;
                foreach (GameObject o in activeDialogueBoxList)
                {
                    Destroy(o);
                }
                activeCharacterList.Clear();
                activeDialogueBoxList.Clear();
                readingDialogue = false;
            });
    }
    #endregion

    void SetCharacterExpression(string characterName, string expression)
    {
        //Used when reading dialogue line to select a portrait from a DialogueCharacterObject
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
        rect.anchoredPosition = new Vector2(onRight ? 300f : -300f, 10f);
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
        Debug.Log("AddLineBreaks() complete");
        return result.ToString();
    }
    TextMeshProUGUI GetDialogueBoxText(GameObject dialogueBox)
    {
        return dialogueBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
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
        ActiveDialogueCharacter currentSpeaker = activeCharacterList.Find(x => x.dialogueCharacter.characterName.Contains(currentLine.CharacterName.Split("_")[0]));
        SetDialogueBoxSide(currentDialogueBox, currentSpeaker.onRightSide);
        GameObject textBG = currentDialogueBox.transform.GetChild(0).transform.GetChild(0).gameObject;
        Timer.Register(0.01f, () => { //this timer is needed so that the text content size filter can apply to the bg before it moves behind the text
            textBG.transform.SetParent(currentDialogueBox.transform);
            textBG.transform.SetAsFirstSibling();
        });
    }

    #region CommandHandlers
    void LoadCommandList()
    {
        _dialogueRunner.AddCommandHandler<string, bool>("add_char", (s, b) => AddCharacter(s, b));
        _dialogueRunner.AddCommandHandler<string>("remove_char", (s) => RemoveCharacter(s));
        _dialogueRunner.AddCommandHandler<string, bool>("set_side", (s, b) => SetCharacterSide(s, b));
    }
    void AddCharacter(string characterName, bool isOnRight = false)
    {

    }
    void RemoveCharacter(string characterName)
    {

    }
    void SetCharacterSide(string characterName, bool isOnRight = false)
    {
        //Locate character in ActiveDialogueCharacterList
    }
    #endregion
}

[Serializable]
class ActiveDialogueCharacter
{
    public ActiveDialogueCharacter(DialogueCharacter dialogueCharacter, string expression, bool onRightSide = false)
    {
        this.dialogueCharacter = dialogueCharacter;
        this.expression = expression;
        this.onRightSide = onRightSide;
    }

    public DialogueCharacter dialogueCharacter;
    public string expression;
    public bool onRightSide;
}