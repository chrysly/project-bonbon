using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CJUtils;
using ModelAssetDatabase.MADUtils;
using static ModelAssetDatabase.ModelAssetDatabaseGUI;

namespace ModelAssetDatabase {
    public class ReaderTabPrefabs : ReaderTab {

        private string ModelPath { get { return Reader.Model.assetPath; } }
        private string ModelID { get { return Reader.ModelID; } }

        /// <summary> The prefab name currently written in the naming Text Field; </summary>
        private string prefabName;

        /// <summary> Class containing relevant Prefab Variant information; </summary>
        private class PrefabVariantData {
            public string guid;
            public string name;
            public PrefabVariantData(string guid, string name) {
                this.guid = guid;
                this.name = name;
            }
        } /// <summary> A list containing all relevant prefab info, to avoid unnecessary operations every frame; </summary>
        private List<PrefabVariantData> PrefabVariantInfo;

        /// <summary> Current state of the name validation process; </summary>
        private GeneralUtils.InvalidNameCondition NameCondition;

        /// <summary> Static log of recent prefab registry changes; </summary>
        private Stack<string> PrefabActionLog;

        private static Vector2 prefabLogScroll;
        private static Vector2 prefabListScroll;

        public override void LoadData(string path) {
            PrefabActionLog = new Stack<string>();
            int prefabCount = UpdatePrefabVariantInfo();
            RegisterPrefabLog("Found " + prefabCount + " Prefab Variant(s) in the Asset Library;");
        }

        /// <summary>
        /// Load and process the Prefab Variant Data from the Model Asset Library for future display;
        /// </summary>
        private int UpdatePrefabVariantInfo() {
            PrefabVariantInfo = new List<PrefabVariantData>();
            List<string> prefabIDs = ModelAssetDatabase.ModelDataDict[ModelID].prefabIDList;
            foreach (string prefabID in prefabIDs) {
                string name = ModelAssetDatabase.PrefabDataDict[prefabID].name + ".prefab";
                PrefabVariantInfo.Add(new PrefabVariantData(prefabID, name));
            } DetermineDefaultPrefabName(ModelPath.ToPrefabPath());
            return prefabIDs.Count;
        }

        /// <summary>
        /// Determine the next default prefab name;
        /// </summary>
        /// <param name="basePath"> Path of the prefab asset; </param>
        /// <param name="name"> Updated inside the recursive stack, no input is required; </param>
        /// <param name="annex"> Updated inside the recursive stack, no input is required; </param>
        private void DetermineDefaultPrefabName(string basePath, string name = null, int annex = 0) {
            if (name == null) {
                name = ModelAssetDatabase.ModelDataDict[ModelID].name.Replace(' ', '_');
                if (char.IsLetter(name[0]) && char.IsLower(name[0])) name = name.Substring(0, 1).ToUpper() + name[1..];
            } string annexedName = name + (annex > 0 ? "_" + annex : "");
            if (ModelAssetDatabase.NoAssetAtPath(basePath + "/" + annexedName + ".prefab")) {
                SetDefaultPrefabName(annexedName);
            } else if (annex < 100) { /// Cheap stack overflow error prevention;
                annex++;
                DetermineDefaultPrefabName(basePath, name, annex);
            }
        }

        /// <summary>
        /// Sets the default prefab name and removes hotcontrol (to update text field);
        /// </summary>
        /// <param name="name"> New default name; </param>
        private void SetDefaultPrefabName(string name) {
            this.prefabName = name;
            GUIUtility.keyboardControl = 0;
            GUIUtility.hotControl = 0;
        }

        /// <summary>
        /// Override for convenient internal use;
        /// </summary>
        /// <returns> True if the name is valid, false otherwise; </returns>
        private bool ValidateFilename() {
            NameCondition = GeneralUtils.ValidateFilename(ModelPath.ToPrefabPathWithName(prefabName), prefabName);
            return NameCondition == 0;
        }

        /// <summary>
        /// Register a prefab in the Model Asset Library with a given name;
        /// </summary>
        /// <param name="modelID"> ID of the model for which the prefab will be registered; </param>
        /// <param name="newPrefabName"> File name of the new prefab variant; </param>
        private void RegisterPrefab(string modelID, string newPrefabName) {
            ModelAssetDatabase.RegisterNewPrefab(modelID, newPrefabName);
            NameCondition = GeneralUtils.InvalidNameCondition.Success;
            UpdatePrefabVariantInfo();
        }

        /// <summary>
        /// Writes a temporary log string with a timestamp to the stack;
        /// </summary>
        /// <param name="log"> String to push to the stack; </param>
        private void RegisterPrefabLog(string log) {
            string logTime = System.DateTime.Now.ToLongTimeString().RemovePathEnd(" ") + ": ";
            PrefabActionLog.Push(logTime + " " + log);
        }

        /// <summary> GUI Display for the Prefabs Section </summary>
        public override void ShowGUI() {

            using (new EditorGUILayout.HorizontalScope(GUILayout.MaxWidth(660))) {
                using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.MaxWidth(330), GUILayout.MaxHeight(140))) {
                    EditorUtils.DrawSeparatorLines("Prefab Variant Registry", true);
                    using (new EditorGUILayout.HorizontalScope()) {
                        GUILayout.Label("Register New Prefab Variant:");
                        if (GUILayout.Button("Validate & Register")) {
                            if (ValidateFilename()) {
                                RegisterPrefab(ModelID, prefabName);
                                RegisterPrefabLog("Added Prefab Variant: " + prefabName + ".prefab;");
                            }
                        }
                    } string impendingName = EditorGUILayout.TextField("Variant Name:", prefabName);
                    if (impendingName != prefabName) {
                        if (NameCondition != 0) NameCondition = 0;
                        SetDefaultPrefabName(impendingName);
                    } DrawNameConditionBox();
                    GUILayout.FlexibleSpace();
                    GUIContent folderContent = new GUIContent(" Show Prefabs Folder", EditorUtils.FetchIcon("d_Folder Icon"));
                    if (GUILayout.Button(folderContent, EditorStyles.miniButton, GUILayout.MaxHeight(18))) {
                        EditorUtils.PingObject(AssetDatabase.LoadAssetAtPath<Object>(ModelPath.ToPrefabPath()));
                    }
                }

                using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.MaxWidth(330), GUILayout.MaxHeight(140))) {
                    EditorUtils.DrawSeparatorLines("Asset Library Logs", true);
                    using (var view = new EditorGUILayout.ScrollViewScope(prefabLogScroll, GUI.skin.box)) {
                        prefabLogScroll = view.scrollPosition;
                        foreach (string line in PrefabActionLog) {
                            GUILayout.Label(line);
                        }
                    } GUIContent clearContent = new GUIContent(" Clear", EditorUtils.FetchIcon("d_winbtn_win_close@2x"));
                    if (GUILayout.Button(clearContent, EditorStyles.miniButton, GUILayout.MaxHeight(18))) PrefabActionLog.Clear();
                }
            } using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.MaxWidth(660))) {
                EditorUtils.DrawSeparatorLines("Registered Prefab Variants", true);
                using (var view = new EditorGUILayout.ScrollViewScope(prefabListScroll, GUILayout.ExpandHeight(false))) {
                    prefabListScroll = view.scrollPosition;
                    DrawPrefabCards();
                }
            }
        }

        /// <summary>
        /// Draw a box with useful information about the chosen file name and prefab creation;
        /// </summary>
        private void DrawNameConditionBox() {
            switch (NameCondition) {
                case GeneralUtils.InvalidNameCondition.None:
                    EditorGUILayout.HelpBox("Messages concerning the availability of the name written above will be displayed here;", MessageType.Info);
                    break;
                case GeneralUtils.InvalidNameCondition.Empty:
                    EditorGUILayout.HelpBox("The name of the file cannot be empty;", MessageType.Error);
                    break;
                case GeneralUtils.InvalidNameCondition.Overwrite:
                    EditorGUILayout.HelpBox("A file with that name already exists in the target directory. Do you wish to overwrite it?", MessageType.Warning);
                    using (new EditorGUILayout.HorizontalScope()) {
                        if (GUILayout.Button("Overwrite")) {
                            RegisterPrefab(ModelID, prefabName);
                            RegisterPrefabLog("Replaced Prefab Variant: " + prefabName + ".prefab;");
                        } if (GUILayout.Button("Cancel")) {
                            NameCondition = 0;
                        }
                    } break;
                case GeneralUtils.InvalidNameCondition.Symbol:
                    EditorGUILayout.HelpBox("The filename can only contain alphanumerical values and/or whitespace characters;", MessageType.Error);
                    break;
                case GeneralUtils.InvalidNameCondition.Convention:
                    GUIStyle simulateMargins = new GUIStyle(EditorStyles.helpBox) { margin = new RectOffset(18, 0, 0, 0) };
                    using (new EditorGUILayout.HorizontalScope(simulateMargins, GUILayout.MaxHeight(30))) {
                        GUIStyle labelStyle = new GUIStyle();
                        labelStyle.normal.textColor = EditorStyles.helpBox.normal.textColor;
                        labelStyle.fontSize = EditorStyles.helpBox.fontSize;
                        GUILayout.Label(new GUIContent(EditorUtils.FetchIcon("console.erroricon.sml@2x")), labelStyle);
                        using (new EditorGUILayout.VerticalScope()) {
                            GUILayout.FlexibleSpace(); GUILayout.FlexibleSpace(); /// Do not judge me. IT LOOKED OFF OK?!
                            GUILayout.Label("This name violates the project's naming convention;", labelStyle);
                            using (new EditorGUILayout.HorizontalScope()) {
                                GUILayout.Label("More information can be found ", labelStyle, GUILayout.ExpandWidth(false));
                                GUIStyle linkStyle = new GUIStyle(labelStyle);
                                linkStyle.normal.textColor = EditorStyles.linkLabel.normal.textColor;
                                if (GUILayout.Button("here", linkStyle, GUILayout.ExpandWidth(false))) {
                                    Application.OpenURL("");
                                } GUILayout.Label(";", labelStyle);
                            } GUILayout.FlexibleSpace(); GUILayout.FlexibleSpace();
                        }
                    } break;
                case GeneralUtils.InvalidNameCondition.Success:
                    GUIContent messageContent = new GUIContent(" Prefab Variant created successfully!", EditorUtils.FetchIcon("d_PreMatCube@2x"));
                    EditorGUILayout.HelpBox(messageContent);
                    break;
            }
        }

        /// <summary>
        /// Iterate over the prefab variants of the model and display a set of actions for each of them;
        /// </summary>
        private void DrawPrefabCards() {
            if (PrefabVariantInfo != null && PrefabVariantInfo.Count > 0) {
                foreach (PrefabVariantData prefabData in PrefabVariantInfo) {
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox)) {
                        GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel) { contentOffset = new Vector2(2, 2) };
                        GUILayout.Label(prefabData.name, labelStyle, GUILayout.MaxWidth(260));
                        if (GUILayout.Button("Open Prefab", GUILayout.MaxWidth(150), GUILayout.MaxHeight(19))) {
                            EditorUtils.OpenAssetProperties(AssetDatabase.GUIDToAssetPath(prefabData.guid));
                        } if (GUILayout.Button("Open Organizer", GUILayout.MaxWidth(150), GUILayout.MaxHeight(19))) {
                            MainGUI.SwitchToOrganizer(prefabData.guid);
                        } GUI.color = UIColors.Red;
                        GUIContent deleteButton = new GUIContent(EditorUtils.FetchIcon("TreeEditor.Trash"));
                        if (GUILayout.Button(deleteButton, GUILayout.MaxWidth(75), GUILayout.MaxHeight(19))) {
                            if (ModalPrefabDeletion.ConfirmPrefabDeletion(prefabData.name)) {
                                ModelAssetDatabase.DeletePrefab(prefabData.guid);
                                RegisterPrefabLog("Deleted Prefab Variant: " + prefabData.name + ";");
                                UpdatePrefabVariantInfo();
                            } GUIUtility.ExitGUI();
                        } GUI.color = Color.white;
                    }
                }
            } else {
                GUILayout.Label("No Prefab Variants have been Registered for this Model;", UIStyles.CenteredLabelBold);
                EditorGUILayout.Separator();
            }
        }
    }
}