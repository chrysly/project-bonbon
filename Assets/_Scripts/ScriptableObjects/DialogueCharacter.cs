using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueCharacter", menuName = "BonBon/DialogueCharacter")]
[Serializable]
public class DialogueCharacter : ScriptableObject
{
    public string characterName;
    public List<PortraitData> portraitList = new();
    public bool defaultToRightSide;
    public Color dialogueBoxColor;
}

[Serializable]
public class PortraitData
{
    public string expression;
    public Sprite portrait;
}