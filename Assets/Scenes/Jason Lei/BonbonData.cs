using UnityEngine;

[CreateAssetMenu(fileName = "Bonbon", menuName = "BonBon/BonbonData")]
[Serializable]
public class BonbonData : ScriptableObject
{
    public string ID;
    public string Name;
    public float attackBuff;
    public float healthBuff;
    public float defenseBuff;
    public int shieldPoints;
    public int targetCount;
}