using System.Linq;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Static Utils for the Turn Order mechanics;
/// </summary>
public static class TurnOrderUtils {

    /// <summary>
    /// Advance the Turn Order by performing due calculations to the Actor AGs;
    /// </summary>
    /// <param name="list"> List containing the turn order values; </param>
    /// <returns> Actor whose turn must be triggered, and whose gauge will be reset by this method; </returns>
    public static Actor Advance(this List<TurnOrderValue> list) {
        /// Update Turn Order list;
        int min = Mathf.CeilToInt(list.Select(tov => tov.actionGauge / (float) tov.Speed).Min());
        list.ForEach(tov => tov.actionGauge = Mathf.Max(0, tov.actionGauge - min * tov.Speed));

        /// Actors that may go next (AG is 0);
        IEnumerable<TurnOrderValue> activeEntries = list.Where(tov => tov.actionGauge == 0);
        if (activeEntries.Count() == 1) return ResetTOV(activeEntries);

        /// Actors with the highest dynamic speed;
        int maxSpeed = activeEntries.Max(tov => tov.Speed);
        activeEntries = activeEntries.Where(tov => tov.Speed == maxSpeed);
        if (activeEntries.Count() == 1) return ResetTOV(activeEntries);

        /// Actors with the highest base speed;
        maxSpeed = activeEntries.Max(tov => tov.actor.Data.BaseSpeed);
        activeEntries = activeEntries.Where(tov => tov.actor.Data.BaseSpeed == maxSpeed);
        if (activeEntries.Count() == 1) return ResetTOV(activeEntries);

        /// Actors that are actors vs. Actors that are enemies;
        IEnumerable<TurnOrderValue> characterEntries = activeEntries.Where(tov => tov.actor is CharacterActor);
        if (characterEntries.Count() == 0) {
            IEnumerable<TurnOrderValue> enemyEntries = activeEntries.Where(tov => tov.actor is EnemyActor);
            return ResetTOV(MinPrioEntry(enemyEntries));
        } else return ResetTOV(MinPrioEntry(characterEntries));
    }

    /// <summary>
    /// Add Actor extension for the Turn Order list;
    /// </summary>
    public static void Add(this List<TurnOrderValue> list, Actor actor, int priority) {
        TurnOrderValue tov = new TurnOrderValue(actor, priority);
        list.Add(tov);
    }

    /// <summary>
    /// Remove Actor extension for the Turn Order list;
    /// </summary>
    public static void Remove(this List<TurnOrderValue> list, Actor actor) {
        int index = list.FindIndex(tov => tov.actor == actor);
        if (index >= 0) list.RemoveAt(index);
    }

    /// <summary>
    /// Reset the AG of a given character from a singleton TOV IEnumerable;
    /// </summary>
    /// <param name="entries"> IEnumerable expected to contain a single element; </param>
    /// <returns> Actor whose gauge will be reset; </returns>
    private static Actor ResetTOV(IEnumerable<TurnOrderValue> entries) {
        TurnOrderValue tov = entries.ElementAt(0);
        tov.ResetActionGauge();
        return tov.actor;
    }

    /// <summary>
    /// Create an enumeration containing only the highest (closer to 0) priority element;
    /// </summary>
    /// <param name="entries"> IEnumerable from which the highest element will be selected; </param>
    /// <returns> Singleton enumeration containing only the highest priority element; </returns>
    private static IEnumerable<TurnOrderValue> MinPrioEntry(IEnumerable<TurnOrderValue> entries) {
        int minPriority = entries.Min(tov => tov.priority);
        return entries.Where(tov => tov.priority == minPriority);
    }
}