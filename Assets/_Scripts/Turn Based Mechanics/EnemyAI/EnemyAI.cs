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
        // later inflict different status conditions
        // also how many bonsbons the characters have
    }

    /// <summary> pass in the current active list of actors and the current actor, returns an ActiveSkillPrep </summary>
    public static BattleStateInput.ActiveSkillPrep ChooseEnemyAISkill(Actor currentActor, List<Actor> activeactors)
    {
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

        foreach(Scenario scen in scenarios)
        {
            Debug.Log(scen.getSkillAction().ToString() + " " + scen.getGoodnessValue());
        }

        // get scenario(s) with the highest goodness values
        List<Scenario> bestScenarios = null;   // better practice would prolly be to just assign the frist skill + random target but it's b4 M1 so
        int bestValue = -1;
        foreach(Scenario scene in scenarios)
        {
            if (scene.getGoodnessValue() > bestValue)
            {
                bestScenarios.Add(scene);
                bestValue = scene.getGoodnessValue();
            }
        }

        // if there's a tie in goodness values, pick a random scenario from the list
        Scenario chosenScenario = bestScenarios[Random.Range(0, bestScenarios.Count)];
        BattleStateInput.ActiveSkillPrep bestActiveSkill = new BattleStateInput.ActiveSkillPrep()
        {
            skill = chosenScenario.getSkillAction().skill,
            targets = chosenScenario.getSkillAction().characterActors.ToArray(),
        };

        return bestActiveSkill;
    }
    
    private static int calculateGoodnessValue(ScenarioSkillData skill)
    {
        // PROLLY CHANGE TO INTS???? --> i changed them to ints
        int value = 0;
        value += addValueBasedOnDamage(skill);
        value += addValueBasedOnHealth(skill);
        // add value based on status

        return value;
    }

    // later add functionality with a target array
    private static int addValueBasedOnDamage(ScenarioSkillData skillData)
    {
        int point = 0;

        // if the skill can kill the targets
        foreach (Actor actor in skillData.characterActors)
        {
            if (skillData.skill.ComputeSkillActionValues(actor).immediateDamage > actor.Hitpoints())
            {
                point += (int) AiWeights.KillUnit;
            }
            else
            {
                point += skillData.skill.ComputeSkillActionValues(actor).immediateDamage * (int) AiWeights.Damage;
            }
        }

        return point;
    }

    // based on % health
    private static int addValueBasedOnHealth(ScenarioSkillData skill)
    {
        int point = 0;

        foreach (Actor actor in skill.characterActors)
        {
            point = (1 - (actor.Hitpoints() / actor.data.MaxHitpoints())) * (int)AiWeights.HealthPercent;
        }

        return point;
    }
}