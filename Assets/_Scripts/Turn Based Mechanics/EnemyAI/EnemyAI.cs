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
        // later inflict different status conditions
        // add some kind of weight for character health
    }

    /// <summary> pass in the current active list of actors and the current actor, returns a skillObject </summary>
    public static SkillAction ChooseEnemyAISkill(Actor currentActor, List<Actor> activeactors)
    {
        // enemy generates stamina


        List<Scenario> scenarios = new List<Scenario>();

        // calculate the goodness value for each skill and each possible target it could have
        foreach(SkillObject skill in currentActor.data.SkillList())
        {
            // make sure the enemy has enough stamina to use the move
            if (currentActor.GetStamina() >= skill.staminaCost)
            {
                foreach(Actor actor in activeactors)
                {
                    if (actor is CharacterActor)
                    {
                        // add to the scenerios list
                        SkillAction newSkillAction = new SkillAction(skill, currentActor, actor);
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

        Debug.Log("target: " + bestSkill.Target() + " skill: " + bestSkill.ToString());

        return bestSkill;
    }
    
    private static int calculateGoodnessValue(SkillAction skill)
    {
        // PROLLY CHANGE TO INTS????
        int value = 0;
        value += addValueBasaedOnDamage(skill);
        // add value based on status

        return value;
    }

    // later add functionality with a target array
    private static int addValueBasaedOnDamage(SkillAction skill)
    {
        int point = 0;

        // if the skill can kill the target
        if (skill.Data().damageAmount > skill.Target().Hitpoints())
        {
            point += 100;
        }
        else
        {
            point += skill.Data().damageAmount * (int) AiWeights.Damage;
        }

        return point;
    }

    //private static float addValueBasedOnHealth(SkillAction)
    //{

    //}
}