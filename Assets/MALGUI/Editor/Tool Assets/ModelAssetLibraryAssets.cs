using UnityEngine;

[CreateAssetMenu(fileName = nameof(ModelAssetLibraryAssets), menuName = "Model Asset Library/Tool Assets")]
public class ModelAssetLibraryAssets : ScriptableObject {
    public Texture2D noMeshPreview;
    public Texture2D noMaterialPreview;
    public Texture2D meshPreviewBackground;
    public Material highlight;
}