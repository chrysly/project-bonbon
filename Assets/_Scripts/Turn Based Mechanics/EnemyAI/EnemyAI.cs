using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Logic for enemy AI skill selection </summary>
public class EnemyAI
{
    enum AiWeights
    {
        KillUnit = 100,
        Damage = 4,
        HealthPercent = 10,
        NumOfBonBons = 2
        // later inflict different status conditions
    }

    /// <summary> pass in the current active list of actors and the current actor, returns an ActiveSkillPrep </summary>
    public static ActiveSkillPrep ChooseEnemyAISkill(Actor currentActor, List<Actor> activeactors) {

        // enemy generates stamina (%)
        currentActor.RefundStamina(50);

        // setup things
        List<Scenario> scenarios = new List<Scenario>();
        List<Actor> characterActors = new List<Actor>();
        foreach (Actor actor in activeactors)
        {
            if (actor is CharacterActor)
            {
                characterActors.Add(actor);
            }
        }

        // calculate the goodness value for each skill and each possible target it could have
        foreach (SkillAction skill in currentActor.SkillList)
        {
            //Debug.Log("Stamina: " + currentActor.GetStamina());
            // make sure the enemy has enough stamina to use the move
            if (currentActor.GetStamina() >= skill.SkillData.staminaCost)
            {
                // if this attack hits multiple targets
                if (skill.SkillData.aoe)
                {

                    ScenarioSkillData newSkillData = new ScenarioSkillData(skill, currentActor, characterActors);
                    scenarios.Add(new Scenario(newSkillData, calculateGoodnessValue(newSkillData)));
                }
                else
                {
                    foreach (Actor actor in characterActors)
                    {
                        ScenarioSkillData newSkillData = new ScenarioSkillData(skill, currentActor, new List<Actor> {actor}); // idk if this is right owell
                        scenarios.Add(new Scenario(newSkillData, calculateGoodnessValue(newSkillData)));
                    }
                }
            }
        }

        //foreach(Scenario scen in scenarios)
        //{
        //    Debug.Log(scen.getSkillAction().ToString() + " " + scen.getGoodnessValue());
        //}

        // get scenario(s) with the highest goodness values
        int max = scenarios.Max(scene => scene.getGoodnessValue());
        scenarios = scenarios.Where(scene => scene.getGoodnessValue() == max).ToList();

        // if there's a tie in goodness values, pick a random scenario from the list
        Scenario chosenScenario = scenarios[Random.Range(0, scenarios.Count)];
        ActiveSkillPrep bestActiveSkill = new ActiveSkillPrep()
        {
            skill = chosenScenario.getSkillAction().skill,
            targets = chosenScenario.getSkillAction().characterActors.ToArray(),
        };

        return bestActiveSkill;
    }

    private static int calculateGoodnessValue(ScenarioSkillData skillData)
    {
        int value = 0;
        foreach (Actor actor in skillData.characterActors)
        {
            value += addValueBasedOnDamage(skillData, actor);
            value += addValueBasedOnHealth(skillData, actor);
            value += addValueBasedOnNumBonbons(skillData, actor);
        }
        return value;
    }

    // later add functionality with a target array
    private static int addValueBasedOnDamage(ScenarioSkillData skillData, Actor actor)
    {
        int point = 0;

        if (skillData.skill.ComputeSkillActionValues(actor).immediateDamage > actor.Hitpoints)  // -1 ._. (jank)
        {
            point += (int) AiWeights.KillUnit;
        }
        else
        {
            //Debug.Log(skillData.skill.ComputeSkillActionValues(actor).immediateDamage);
            point += skillData.skill.ComputeSkillActionValues(actor).immediateDamage * (int) AiWeights.Damage;  // -1 ._.
        }
        return point;
    }

    // based on % health
    private static int addValueBasedOnHealth(ScenarioSkillData skill, Actor actor)
    {
        int point = 0;
        point = (1 - (actor.Hitpoints / actor.Data.MaxHitpoints)) * (int)AiWeights.HealthPercent;
        return point;
    }

    private static int addValueBasedOnNumBonbons(ScenarioSkillData skill, Actor actor)
    {
        int point = 0;
        point = actor.BonbonList.Count * (int)AiWeights.NumOfBonBons;
        return point;

    }
}
