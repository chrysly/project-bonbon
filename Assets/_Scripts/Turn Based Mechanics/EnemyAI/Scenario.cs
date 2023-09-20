using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario
{
    private int _goodnessValue = 0;
    private ScenarioSkillData _scenarioSkillData = null;

    public Scenario(ScenarioSkillData skillAction, int value)
    {
        _goodnessValue = value;
        _scenarioSkillData = skillAction;
    }

    public void setGoodnessValue(int value)
    {
        _goodnessValue += value;
    }

    public int getGoodnessValue()
    {
        return _goodnessValue;
    }

    public ScenarioSkillData getSkillAction()
    {
        return _scenarioSkillData;
    }
}

public class ScenarioSkillData {
    public SkillAction skill;
    public Actor currentActor;
    public List<Actor> characterActors;

    public ScenarioSkillData(SkillAction skill, Actor currentActor, List<Actor> characterActors) {
        this.skill = skill;
        this.currentActor = currentActor;
        this.characterActors = characterActors;
    }
}
