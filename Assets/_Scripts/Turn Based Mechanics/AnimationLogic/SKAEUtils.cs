using System.Collections.Generic;
using PseudoDataStructures;

public static class SKAEUtils {

    public static Dictionary<T1, Dictionary<T2, T3>>
        ProcessInternalDictionary<T1, T2, T3>(PseudoDictionary<T1, PseudoDictionary<T2, T3>> complexMap) {
        Dictionary<T1, PseudoDictionary<T2, T3>> pseudoInternalDict = complexMap.ToDictionary();
        Dictionary<T1, Dictionary<T2, T3>> outDict = new Dictionary<T1, Dictionary<T2, T3>>();
        foreach (KeyValuePair<T1, PseudoDictionary<T2, T3>> kvp in pseudoInternalDict) {
            outDict[kvp.Key] = kvp.Value.ToDictionary();
        }
        return outDict;
    }

    public static PseudoDictionary<T1, PseudoDictionary<T2, T3>>
            RevertInternalDictionary<T1, T2, T3>(Dictionary<T1, Dictionary<T2, T3>> simpleMap) {
        Dictionary<T1, PseudoDictionary<T2, T3>> outMap = new Dictionary<T1, PseudoDictionary<T2, T3>>();
        foreach (KeyValuePair<T1, Dictionary<T2, T3>> kvp in simpleMap) {
            outMap[kvp.Key] = new PseudoDictionary<T2, T3>(kvp.Value);
        }
        return new PseudoDictionary<T1, PseudoDictionary<T2, T3>>(outMap);
    }
}
