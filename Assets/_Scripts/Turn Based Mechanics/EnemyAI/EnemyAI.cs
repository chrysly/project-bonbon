using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyAI
{
    enum AiWeights
    {
        KillUnit = 100,
        Damage = 4,
        HealthPercent = 10,
        // later inflict different status conditions
    }

    /// <summary> pass in the current active list of actors and the current actor, returns a skillObject </summary>
    public static SkillAction ChooseEnemyAISkill(Actor currentActor, List<Actor> activeactors)
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
        foreach(SkillObject skill in currentActor.data.SkillList())
        {
            // make sure the enemy has enough stamina to use the move
            if (currentActor.GetStamina() >= skill.staminaCost)
            {
                // if this attack hits multiple targets
                if (skill.aoe)
                {
                    SkillAction newSkillAction = new SkillAction(skill, currentActor, characterActors);
                    scenarios.Add(new Scenario(newSkillAction, calculateGoodnessValue(newSkillAction)));
                }
                else
                {
                    foreach (Actor actor in characterActors)
                    {
                        SkillAction newSkillAction = new SkillAction(skill, currentActor, new List<Actor> {actor}); // idk if this is right owell
                        scenarios.Add(new Scenario(newSkillAction, calculateGoodnessValue(newSkillAction)));
                    }
                } 
            }      
        }

        foreach(Scenario scen in scenarios)
        {
            Debug.Log(scen.getSkillAction().ToString() + " " + scen.getGoodnessValue());
        }

        // edit this so if two ppl have == goodness values it randomlly chooses
        // return skill with the highest goodnessvalue
        SkillAction bestSkill = new SkillAction(null, null, null);
        int bestValue = -1;
        foreach(Scenario scene in scenarios)
        {
            if (scene.getGoodnessValue() > bestValue)
            {
                bestSkill = scene.getSkillAction();
                bestValue = scene.getGoodnessValue();
            }
        }

        Debug.Log("target: " + bestSkill.Targets() + " skill: " + bestSkill.ToString());

        return bestSkill;
    }
    
    private static int calculateGoodnessValue(SkillAction skill)
    {
        // PROLLY CHANGE TO INTS???? --> i changed them to ints
        int value = 0;
        value += addValueBasaedOnDamage(skill);
        value += addValueBasedOnHealth(skill);
        // add value based on status

        return value;
    }

    // later add functionality with a target array
    private static int addValueBasaedOnDamage(SkillAction skill)
    {
        int point = 0;

        // if the skill can kill the targets
        foreach (Actor actor in skill.Targets())
        {
            if (skill.Data().damageAmount > actor.Hitpoints())
            {
                point += 100;
            }
            else
            {
                point += skill.Data().damageAmount * (int) AiWeights.Damage;
            }
        }

        return point;
    }

    // based on % health
    private static int addValueBasedOnHealth(SkillAction skill)
    {
        int point = 0;

        foreach (Actor actor in skill.Targets())
        {
            point = (1 - (actor.Hitpoints() / actor.data.MaxHitpoints())) * (int)AiWeights.HealthPercent;
        }

        return point;
    }
}