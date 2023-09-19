using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario
{
    private int _goodnessValue = 0;
    private SkillAction _skillAction = null;

    public Scenario(SkillAction skillAction, int value)
    {
        _goodnessValue = value;
        _skillAction = skillAction;
    }

    public void setGoodnessValue(int value)
    {
        _goodnessValue += value;
    }

    public int getGoodnessValue()
    {
        return _goodnessValue;
    }

    public SkillAction getSkillAction()
    {
        return _skillAction;
    }
}
