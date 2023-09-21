using UnityEngine;

namespace  ModelAssetDatabase {

    /// <summary>
    /// Collection of assets for the Database GUI;
    /// </summary>
    [CreateAssetMenu(fileName = nameof(MADAssets), menuName = "Model Asset Database/Tool Assets")]
    public class MADAssets : ScriptableObject {
        public Texture2D noMeshPreview;
        public Texture2D meshPreviewBackground;
        public Material defaultMaterial;
        public Material highlight;
        public GameObject spherePrefab;
        public GameObject cubePrefab;
    }
}