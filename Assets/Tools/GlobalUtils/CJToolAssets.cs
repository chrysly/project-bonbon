using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CJUtils {

    public class CJToolAssets : ScriptableObject {

        [System.Serializable]
        public class DnDFieldAssets {
            public Texture addNormal;
            public Texture addHover;
            public Texture removeNormal;
            public Texture removeHover;
        } public DnDFieldAssets dndFieldAssets;

        [System.Serializable]
        public class StatFieldAssets {
            public Texture hitpoints;
            public Texture stamina;
            public Texture staminaRegen;
            
            public Texture attack;
            public Texture heal;
            public Texture paralysis;
            public Texture defense;
            public Texture speed;

            public Texture time;
        } public StatFieldAssets statFieldAssets;
    }
}