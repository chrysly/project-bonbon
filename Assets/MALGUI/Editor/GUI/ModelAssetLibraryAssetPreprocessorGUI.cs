using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CJUtils;
using static ModelAssetLibraryAssetPreprocessor;

/// <summary>
/// User interface for the Asset Library Preprocessor;
/// </summary>
public class ModelAssetLibraryAssetPreprocessorGUI : EditorWindow {

    /// <summary>
    /// Library Reimport context menu;
    /// </summary>
    [MenuItem("Assets/Library Reimport", false, 50)]
    public static void LibraryReimport() {
        string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
        ModelImporter model = AssetImporter.GetAtPath(path) as ModelImporter;
        LibraryReimport(model);
    }

    /// <summary>
    /// Library Reimport function;
    /// </summary>
    /// <param name="model"> Model to reimport; </param>
    public static void LibraryReimport(ModelImporter model) {
        if (model != null) LoadBasicOptions(model);
        ShowWindow();
    }

    /// <summary>
    /// Validate function for the Library Reimport context menu;
    /// </summary>
    /// <returns> Whether the selected asset is a Model; </returns>
    [MenuItem("Assets/Library Reimport", true)]
    private static bool LibraryReimportValidate() {
        return Selection.assetGUIDs.Length == 1 && ModelAssetLibraryConfigurationCore.Config.rootAssetPath != null
               && AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0])) is ModelImporter;
    }

    /// <summary>
    /// Show the Reimport Window;
    /// </summary>
    public static void ShowWindow() {
        var window = GetWindow<ModelAssetLibraryAssetPreprocessorGUI>("Library Reimport");
        window.ShowAuxWindow();
    }

    /// <summary> Array of new materials pending verification in the GUI; </summary>
    private Material[] tempMaterials;
    /// <summary> Key used by the Shader Selection event; </summary>
    private string shaderKey;

    /// <summary> Root GameObject of the target model;
    /// <br></br> Used for the Object Preview; </summary>
    private GameObject modelGO;

    private Vector2 globalScroll;
    private Vector2 materialsNewScroll;
    private Vector2 materialSlotScroll;

    void OnEnable() {
        ModelAssetLibraryModelReader.CleanObjectPreview();
        if (Options == null) return;
        if (Options.model != null) {
            modelGO = AssetDatabase.LoadAssetAtPath<GameObject>(Options.model.assetPath);
        } ProcessLibraryMaterialData(Options.model);
        tempMaterials = new Material[MaterialOverrideMap.Count];
    }

    void OnDisable() {
        ModelAssetLibraryModelReader.CleanObjectPreview();
        if (Options != null) FlushImportData();

        tempMaterials = null;
        shaderKey = null;
        modelGO = null;
    }

    void OnGUI() {
        if (Options == null || Options.model == null) {
            EditorUtils.DrawScopeCenteredText("Unity revolted against this window.\nPlease reload it!") ;
            return;
        } using (var view = new EditorGUILayout.ScrollViewScope(globalScroll)) {
            globalScroll = view.scrollPosition;
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                EditorUtils.DrawSeparatorLines("Core Import Settings", true);
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                    GUILayout.Label("Import Mode:", UIStyles.ArrangedLabel, GUILayout.MaxWidth(125));
                    GUI.enabled = Options.hasMeshes;
                    GUILayout.Label("Model", 
                                    Options.hasMeshes ? UIStyles.ArrangedButtonSelected 
                                    : UIStyles.ArrangedBoxUnselected, GUILayout.MinWidth(105));
                    GUI.enabled = !Options.hasMeshes;
                    GUILayout.Label("Animation(s)",
                                    !Options.hasMeshes ? UIStyles.ArrangedButtonSelected
                                    : UIStyles.ArrangedBoxUnselected, GUILayout.MinWidth(105));
                    GUI.enabled = true;
                } using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                    GUILayout.Label("Category:", UIStyles.ArrangedLabel, GUILayout.MaxWidth(125));
                    Options.folder = EditorGUILayout.Popup(Options.folder, FolderPaths, GUILayout.MinWidth(215));
                } using (new EditorGUILayout.HorizontalScope()) {
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                        GUILayout.Label("Relocate Prefabs:", UIStyles.ArrangedLabel);
                        Options.relocatePrefabs = EditorGUILayout.Toggle(Options.relocatePrefabs, GUILayout.Width(16));
                    } using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                        GUI.enabled = false;
                        GUILayout.Label("Relocate Materials:", UIStyles.ArrangedLabel);
                        Options.relocateMaterials = EditorGUILayout.Toggle(Options.relocateMaterials, GUILayout.Width(16));
                        GUI.enabled = true;
                    }
                } using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                    GUILayout.Label("Material Settings:", UIStyles.ArrangedLabel, GUILayout.MaxWidth(125));
                    GUI.enabled = Options.hasMeshes;
                    Options.model.materialImportMode = (ModelImporterMaterialImportMode)
                                                       EditorGUILayout.EnumPopup(Options.model.materialImportMode, GUILayout.MinWidth(40));
                    Options.model.materialLocation = (ModelImporterMaterialLocation)
                                                     EditorGUILayout.EnumPopup(Options.model.materialLocation, GUILayout.MinWidth(40));
                }
            } using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                EditorUtils.DrawSeparatorLines("Additional Import Settings", true);
                if (Options.hasMeshes && Options.model.materialImportMode > 0 && Options.model.materialLocation > 0) {
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                        GUILayout.Label("Material Override:", UIStyles.ArrangedLabel);
                        if (GUILayout.Button("None", Options.materialOverrideMode == MaterialOverrideMode.None
                                                     ? UIStyles.ArrangedButtonSelected : GUI.skin.button, GUILayout.MinWidth(70))) {
                            SetMaterialOverrideMode(MaterialOverrideMode.None);
                        } if (GUILayout.Button("Single", Options.materialOverrideMode == MaterialOverrideMode.Single
                                                         ? UIStyles.ArrangedButtonSelected : GUI.skin.button, GUILayout.MinWidth(70))) {
                            SetMaterialOverrideMode(MaterialOverrideMode.Single);
                        } if (GUILayout.Button("Multiple", Options.materialOverrideMode == MaterialOverrideMode.Multiple
                                                           ? UIStyles.ArrangedButtonSelected : GUI.skin.button, GUILayout.MinWidth(70))) {
                            SetMaterialOverrideMode(MaterialOverrideMode.Multiple);
                        }
                    } if (Options.materialOverrideMode == MaterialOverrideMode.Multiple) {
                        using (new EditorGUILayout.HorizontalScope()) {
                            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox, GUILayout.MaxWidth(150))) {
                                GUILayout.Label("Use Single Shader:");
                                Options.useSingleShader = EditorGUILayout.Toggle(Options.useSingleShader);
                            } bool noShaderSelected = Options.shader == null;
                            if (!Options.useSingleShader) GUI.enabled = false;
                            else if (noShaderSelected) GUI.color = UIColors.DarkRed;
                            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                                GUI.color = Color.white;
                                GUIContent shaderContent = new GUIContent(noShaderSelected ? "No Shader Selected" : Options.shader.name);
                                DrawShaderPopup(shaderContent, null);
                                if (!Options.useSingleShader) GUI.enabled = true;
                            }
                        }
                    } DrawMaterialSettings();
                } else {
                    using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                        GUI.color = new Color(0.9f, 0.9f, 0.9f);
                        GUIStyle nopeStyle = new GUIStyle(UIStyles.CenteredLabelBold);
                        nopeStyle.fontSize--;
                        GUILayout.Label("No Additional Settings", nopeStyle);
                        GUI.color = Color.white;
                    }
                } 
            } using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                    GUI.color = UIColors.Blue;
                    if (GUILayout.Button("Reimport")) {
                        ReimportAsset();
                        Close();
                    } GUI.color = UIColors.Red;
                    if (GUILayout.Button("Cancel")) Close();
                    GUI.color = Color.white;
                }
            }
        }
    }

    /// <summary>
    /// Choose a Material Override Mode and Draw the content for that mode accordingly;
    /// </summary>
    private void DrawMaterialSettings() {

        if (modelGO != null) {
            switch (Options.materialOverrideMode) {
                case MaterialOverrideMode.Single:
                    using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                        using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                            GUILayout.Label("Preview", UIStyles.CenteredLabel);
                            ModelAssetLibraryModelReader.DrawObjectPreviewEditor(modelGO, 96, 112);
                            if (GUILayout.Button("Expanded Preview")) {
                                ModelAssetLibraryPreviewExpanded.ShowPreviewWindow(modelGO);
                            }
                        }
                    } DrawMaterialSlot(SingleKey, MaterialOverrideMap[SingleKey], 0);
                    break;
                case MaterialOverrideMode.Multiple:
                    using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
                        using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                            using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                                GUILayout.Label("Preview", UIStyles.CenteredLabel);
                                ModelAssetLibraryModelReader.DrawObjectPreviewEditor(modelGO, 96, 112);
                                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button) { richText = true }; 
                                buttonStyle.fontSize--;
                                if (GUILayout.Button("<b>Expand Preview</b>", buttonStyle)) {
                                    ModelAssetLibraryPreviewExpanded.ShowPreviewWindow(modelGO);
                                }
                            } using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.MaxWidth(96))) {
                                EditorUtils.DrawSeparatorLines("New Materials", true);
                                using (var view = new EditorGUILayout.ScrollViewScope(materialsNewScroll)) {
                                    materialsNewScroll = view.scrollPosition;
                                    DrawNewMaterials();
                                }
                            }
                        } using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                            EditorUtils.DrawSeparatorLines("Available Slots", true);
                            using (var view = new EditorGUILayout.ScrollViewScope(materialSlotScroll)) {
                                materialSlotScroll = view.scrollPosition;
                                DrawAvailableSlots();
                            }
                        }
                    } break;
            }
        } else GUILayout.Label("Something went wrong here, ask Carlos or something;", UIStyles.CenteredLabelBold);
    }

    /// <summary>
    /// Draws a 'list' of all materials that will be 'created' after a successful reimport;
    /// </summary>
    private void DrawNewMaterials() {
        if (PreservedMaterialMap.Count > 0) {
            foreach (KeyValuePair<string, Material> kvp in PreservedMaterialMap) {
                using (new EditorGUILayout.VerticalScope(GUI.skin.box)) {
                    GUILayout.Label(kvp.Key, UIStyles.CenteredLabelBold);
                    EditorGUILayout.ObjectField(kvp.Value, typeof(Material), false);
                }
            }
        } else {
            GUILayout.Label("- Empty -", UIStyles.CenteredLabelBold);
            EditorGUILayout.Separator();
        }
    }

    /// <summary>
    /// Draws all available material slots for the given model;
    /// </summary>
    private void DrawAvailableSlots() {
        int i = 1;
        foreach (KeyValuePair<string, MaterialData> kvp in MaterialOverrideMap) {
            if (string.IsNullOrWhiteSpace(kvp.Key)) continue;
            DrawMaterialSlot(kvp.Key, kvp.Value, i);
            EditorGUILayout.Separator();
            EditorUtils.DrawSeparatorLine(1);
            EditorGUILayout.Separator();
            i++;
        }
    }

    /// <summary>
    /// Draw a material slot given a slot key;
    /// </summary>
    /// <param name="key"> Key to the slot to draw; </param>
    /// <param name="data"> Material Data pertaining to the slot to draw; </param>
    /// <param name="i"> How far down the GUI material list are we; </param>
    private void DrawMaterialSlot(string key, MaterialData data, int i) {
        using (new EditorGUILayout.HorizontalScope(GUI.skin.box)) {
            if (string.IsNullOrEmpty(key)) {
                GUILayout.Label("Global Material", UIStyles.CenteredLabelBold);
            } else GUILayout.Label(key, UIStyles.CenteredLabelBold);
        } bool containsTempKey = TempMaterialMap.ContainsKey(key);
        if ((data.isNew && containsTempKey)
            || (!data.isNew && data.materialRef != null)) GUI.color = UIColors.Green;
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
            GUI.color = Color.white;
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                GUILayout.Label("Origin:", UIStyles.ArrangedLabel, GUILayout.MinWidth(52), GUILayout.MaxWidth(52));
                if (GUILayout.Button("New", data.isNew
                                                ? UIStyles.ArrangedButtonSelected : GUI.skin.button, GUILayout.MinWidth(70))) {
                    if (!data.isNew) ToggleMaterialMap(key);
                } if (GUILayout.Button("Remap", data.isNew
                                                    ? GUI.skin.button : UIStyles.ArrangedButtonSelected, GUILayout.MinWidth(70))) {
                    if (data.isNew) ToggleMaterialMap(key);
                }
            } using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                if (data.isNew) {
                    bool validName = ValidateMaterialName(data.name);
                    bool validMaterial = ValidateTemporaryMaterial(key);
                    if (!validName) GUI.color = UIColors.DarkRed;
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                        GUI.color = Color.white;
                        GUILayout.Label("Name:", GUILayout.MinWidth(48), GUILayout.MaxWidth(48));
                        data.name = EditorGUILayout.TextField(data.name);
                        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button) { margin = new RectOffset(), padding = new RectOffset() };
                        if (GUILayout.Button(new GUIContent(EditorUtils.FetchIcon("d__Help")),
                                         buttonStyle, GUILayout.Width(18), GUILayout.Height(18))) {
                            /// Insert Documentation URL Opener here;
                        }
                    } using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                        GUILayout.Label("Albedo:", GUILayout.MinWidth(48), GUILayout.MaxWidth(48));
                        data.albedoMap = (Texture2D) EditorGUILayout.ObjectField(data.albedoMap, typeof(Texture2D), false);
                    } using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                        GUILayout.Label("Normal:", GUILayout.MinWidth(48), GUILayout.MaxWidth(48));
                        data.normalMap = (Texture2D) EditorGUILayout.ObjectField(data.normalMap, typeof(Texture2D), false);
                    } if (!Options.useSingleShader || Options.materialOverrideMode == MaterialOverrideMode.Single) {
                        bool noShader = data.shader == null;
                        if (noShader) GUI.color = UIColors.DarkRed;
                        using (var scope = new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                            GUI.color = Color.white;
                            GUILayout.Label("Shader:", GUILayout.MinWidth(48), GUILayout.MaxWidth(48));
                            GUIContent shaderContent = new GUIContent(noShader ? "No Shader Selected" : data.shader.name);
                            DrawShaderPopup(shaderContent, key);
                        }
                    } /*if (!validMaterial || !validName)*/ GUI.enabled = false;
                    if (containsTempKey) {
                        bool materialsAreEqual = ValidateMaterialEquality(key);
                        if (materialsAreEqual) GUI.enabled = false;
                        using (new EditorGUILayout.HorizontalScope()) {
                            GUI.color = UIColors.Blue;
                            if (GUILayout.Button("Replace")) GenerateTemporaryMaterial(key);
                            GUI.enabled = true;
                            GUI.color = UIColors.Red;
                            if (GUILayout.Button("Remove")) RemoveNewMaterial(key);
                        } GUI.color = Color.white;
                    } else {
                        GUI.color = UIColors.Green;
                        if (GUILayout.Button("Generate Material")) GenerateTemporaryMaterial(key);
                        GUI.color = Color.white;
                    } if (!validMaterial) GUI.enabled = true;
                } else {
                    using (new EditorGUILayout.HorizontalScope()) {
                        GUILayout.Label("Material:", GUILayout.MaxWidth(52));
                        bool wasAssigned = tempMaterials[i] != null;
                        tempMaterials[i] = (Material) EditorGUILayout.ObjectField(tempMaterials[i], typeof(Material), false);
                        if ( (tempMaterials[i] != null && tempMaterials[i] != data.materialRef)
                            || (wasAssigned && tempMaterials[i] == null) ) UpdateMaterialRef(key, tempMaterials[i]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draws a Shader Popup... It literally says it in the header bruh;
    /// </summary>
    /// <param name="shaderContent"> Text to display in the popup header; </param>
    /// <param name="key"> Key where the shader value will be placed; </param>
    private void DrawShaderPopup(GUIContent shaderContent, string key) {
        shaderKey = key;
        Rect position = EditorGUILayout.GetControlRect(GUILayout.MinWidth(135));
        if (EditorGUI.DropdownButton(position, shaderContent, FocusType.Keyboard, EditorStyles.miniPullDown)) {
            OnShaderResult += ApplyShaderResult;
            ShowShaderSelectionMagic(position);
        }
    }

    /// <summary>
    /// Response method to the Shader Selection event;
    /// </summary>
    /// <param name="shader"> Shader returned by the Shader Popup; </param>
    private void ApplyShaderResult(Shader shader) {
        if (shaderKey == null) Options.shader = shader;
        else MaterialOverrideMap[shaderKey].shader = shader;
        shaderKey = null;
        OnShaderResult -= ApplyShaderResult;
    }
}