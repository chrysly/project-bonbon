using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
# endif

/// <summary>
/// A few utility classes wrote by yours truly ;D
/// </summary>
namespace CJUtils {

    public static class UIColors {

        public static Color Blue {
            get {
                return new Vector4(0.861f, 0.925f, 0.994f, 1);
            }
        }

        public static Color Green {
            get {
                return new Vector4(0.825f, 0.99f, 0.99f, 1);
            }
        }

        public static Color Red {
            get {
                return new Vector4(0.99f, 0.825f, 0.825f, 1);
            }
        }

        public static Color DarkRed {
            get {
                return new Vector4(1.0f, 0.5f, 0.5f, 1);
            }
        }

        public static Color DarkBlue {
            get {
                return new Vector4(0.3f, 0.8f, 1.0f, 1);
            }
        }

        public static Color Cyan {
            get {
                return new Vector4(0.25f, 0.95f, 1.0f, 1);
            }
        }
    }

    #if UNITY_EDITOR

    /// <summary>
    /// A collection of functions to draw custom bundles of UI Elements;
    /// </summary>
    public static class EditorUtils {

        /// <summary>
        /// Opens and/or focuses the Project Window;
        /// </summary>
        public static void OpenProjectWindow() {
            System.Type projectBrowserType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
            EditorWindow.GetWindow(projectBrowserType);
        }

        /// <summary>
        /// Opens/focuses the Project Window and Pings Object;
        /// </summary>
        /// <param name="obj"> Object to ping in the Project Window; </param>
        public static void PingObject(Object obj) {
            OpenProjectWindow();
            EditorGUIUtility.PingObject(obj);
        }

        /// <summary>
        /// Draws a bold text label at the center of the current scope;
        /// </summary>
        /// <param name="text"> Text to display; </param>
        public static void DrawScopeCenteredText(string text) {
            GUILayout.FlexibleSpace();
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.FlexibleSpace();
                GUILayout.Label(text, UIStyles.CenteredLabelBold);
                GUILayout.FlexibleSpace();
            } GUILayout.FlexibleSpace();
        }

        /// <summary>
        /// Creates a pop up button with a prefixed label (doesn't do anything yet but we are getting there);
        /// </summary>
        /// <param name="textLabel"> Text for the label; </param>
        /// <param name="textPopup"> Text for the popup; </param>
        public static void CreateLabeledPopUp(string textLabel, string textPopup) {
            EditorGUILayout.LabelField(textLabel, UIStyles.ToolbarText, GUILayout.Width(110));
            GUILayout.Button(textPopup, EditorStyles.toolbarPopup, GUILayout.MinWidth(100), GUILayout.MaxWidth(140));
        }

        /// <summary>
        /// Returns the width of a text string in pixels;
        /// </summary>
        /// <param name="text"> Text string to measure; </param>
        /// <param name="font"> Font to get the width from; </param>
        /// <returns> Width of the text in question; </returns>
        public static float MeasureTextWidth(string text, Font font) {
            float width = 0;
            foreach (char letter in text) {
                CharacterInfo info;
                font.GetCharacterInfo(letter, out info);
                width += info.advance;
            } return width;
        }

        /// <summary>
        /// Draw a single separator line;
        /// </summary>
        public static void DrawSeparatorLine() {
            using (var hscope = new EditorGUILayout.HorizontalScope()) {
                Handles.color = Color.gray;
                Handles.DrawLine(new Vector2(hscope.rect.x, hscope.rect.y), new Vector2(hscope.rect.xMax, hscope.rect.y));
                Handles.color = Color.white;
            }
        }

        /// <summary>
        /// Non-handle version of a line;
        /// </summary>
        /// <param name="height"> Height of the line in pixels; </param>
        public static void DrawSeparatorLine(int height) {
            GUILayout.Space(4);
            Rect rect = GUILayoutUtility.GetRect(1, height, GUILayout.ExpandWidth(true));
            rect.height = height;
            rect.xMin = 0;
            rect.xMax = EditorGUIUtility.currentViewWidth;
            EditorGUI.DrawRect(rect, Color.gray);
            GUILayout.Space(4);
        }

        /// <summary>
        /// Draws two horizontal lines with a title in the middle;
        /// </summary>
        /// <param name="title"> Text drawn between the lines; </param>
        public static void DrawSeparatorLines(string title, bool centered = false, bool spaceOut = true) {
            using (new EditorGUILayout.VerticalScope()) {
                if (spaceOut) EditorGUILayout.Space();
                DrawSeparatorLine();
                if (centered) GUILayout.Label(title, UIStyles.CenteredLabelBold);
                else EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                DrawSeparatorLine();
                if (spaceOut) EditorGUILayout.Space();
            }
        }

        /// <summary>
        /// Fetch an icon texture from the database.
        /// <br></br> A list of Unity's Built-In Icon Names can be found <a href="https://github.com/Zxynine/UnityEditorIcons/tree/main">here</a>;
        /// </summary>
        /// <param name="iconName"> A list of Unity's Built-In Icon Names can be found <a href="https://github.com/Zxynine/UnityEditorIcons/tree/main">here</a>; </param>
        /// <returns> Icon texture; </returns>
        public static Texture2D FetchIcon(string iconName) {
            return (Texture2D) EditorGUIUtility.IconContent(iconName).image;
        }

        /// <summary>
        /// Draws two labels on a horizontal scope, left and right aligned respectively;
        /// </summary>
        /// <param name="leftLabel"> Content of the left label; </param>
        /// <param name="rightLabel"> Content of the right label; </param>
        /// <param name="options"> Array of GUILayoutOptions to apply to the labels; </param>
        public static void DrawLabelPair(string leftLabel, string rightLabel, params GUILayoutOption[] options) {
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.Label(leftLabel, options);
                GUILayout.Label(rightLabel, UIStyles.RightAlignedLabel, options);
            }   
        }

        /// <summary>
        /// Draws two labels on a horizontal scope, left and right aligned respectively;
        /// </summary>
        /// <param name="leftLabel"> Content of the left label; </param>
        /// <param name="rightLabel"> Content of the right label; </param>
        /// <param name="style"> GUIStyle to apply to both labels (overriding alignment); </param>
        /// <param name="options"> Array of GUILayoutOptions to apply to the labels; </param>
        public static void DrawLabelPair(string leftLabel, string rightLabel, GUIStyle style, params GUILayoutOption[] options) {
            GUIStyle leftStyle = new GUIStyle(style) { alignment = TextAnchor.MiddleLeft };
            GUIStyle rightStyle = new GUIStyle(style) { alignment = TextAnchor.MiddleRight };
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.Label(leftLabel, leftStyle, options);
                GUILayout.Label(rightLabel, rightStyle, options);
            }
        }

        /// <summary>
        /// Takes an integer (long) number of bytes and returns a file size string with reasonable units;
        /// <br></br> PS: The code for this method is intentionally a dumpster fire. I mean, look at it. It's just pretty >.>;
        /// </summary>
        /// <param name="bytes"> File length to parse; </param>
        /// <returns> File size string with reasonable units in the format "{size} {units}"; </returns>
        public static string ProcessFileSize(long bytes) {
            return bytes / Mathf.Pow(1024, 3) > 1 ? (int) (bytes / Mathf.Pow(1024f, 3) * 100f) / 100f + " GB"
                   : bytes / Mathf.Pow(1024, 2) > 1 ? (int) (bytes / Mathf.Pow(1024f, 2) * 100f) / 100f + " MB"
                   : bytes / Mathf.Pow(1024, 1) > 1 ? (int) (bytes / Mathf.Pow(1024f, 1) * 100f) / 100f + " KB"
                   : bytes + " bytes";
        }

        /// <summary>
        /// Draw a texture on the current layout;
        /// </summary>
        /// <param name="texture"> Texture to draw; </param>
        /// <param name="width"> Width of the Box containing the texture; </param>
        /// <param name="height"> Height of the Box containing the texture; </param>
        public static void DrawTexture(Texture2D texture, float width, float height) {
            GUILayout.Box(texture, GUILayout.Width(width), GUILayout.Height(height));
        }

        /// <summary>
        /// Pulls up the inspector properties associated with an asset on a separate window;
        /// </summary>
        /// <param name="path"> Path to the asset to pull up the inspector for; </param>
        public static void OpenAssetProperties(string path) {
            EditorUtility.OpenPropertyEditor(AssetDatabase.LoadAssetAtPath(path, typeof(Object)));
        }

        /// <summary>
        /// Pulls up the inspector properties associated with an asset on a separate window;
        /// </summary>
        /// <param name="targetObject"> Asset to pull up the inspector for; </param>
        public static void OpenAssetProperties(Object targetObject) {
            EditorUtility.OpenPropertyEditor(targetObject);
        }

        /// <summary>
        /// Draws a Help Box with a custom icon;
        /// </summary>
        /// <param name="text"> Help Box message; </param>
        /// <param name="texture"> Help Box icon; </param>
        /// <param name="width"> Help Box width; </param>
        public static void DrawCustomHelpBox(string text, Texture texture, float width, float height) {
            GUIContent messageContent = new GUIContent(text, texture);
            GUILayout.Label(messageContent, UIStyles.HelpBoxLabel, GUILayout.Width(width), GUILayout.Height(height),
                            GUILayout.ExpandWidth(width == 0), GUILayout.ExpandHeight(height == 0));
        }

        /// <summary>
        /// Draws a Help Box with a custom icon;
        /// </summary>
        /// <param name="text"> Help Box message; </param>
        /// <param name="texture"> Help Box icon; </param>
        public static void DrawCustomHelpBox(string text, Texture texture) {
            GUIContent messageContent = new GUIContent(text, texture);
            GUILayout.Label(messageContent, UIStyles.HelpBoxLabel);
        }
    }

    /// <summary>
    /// A collection of Custom Editor UI Styles to make things pretty;
    /// </summary>
    public class UIStyles {

        public static GUIStyle TemplateStyle {
            get {
                GUIStyle style = new GUIStyle();
                return style;
            }
        }

        #region Views and Bars;

        public static GUIStyle WindowBox {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.window) {
                    padding = new RectOffset(5, 5, 5, 5),
                    stretchWidth = false,
                    stretchHeight = false,
                }; return style;
            }
        }

        public static GUIStyle ToolbarText {
            get {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = GUI.skin.label.normal.textColor;
                style.alignment = TextAnchor.MiddleRight;
                style.padding = new RectOffset(0, 10, 0, 1);
                style.contentOffset = new Vector2(15, 0);
                return style;
            }
        }

        public static GUIStyle ToolbarPaddedPopUp {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.toolbarPopup);
                style.contentOffset = new Vector2(2, 0);
                return style;
            }
        }

        public static GUIStyle PaddedToolbar {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.toolbar);
                style.padding = new RectOffset(10, 10, 0, 0);
                return style;
            }
        }

        public static GUIStyle PaddedScrollView {
            get {
                GUIStyle style = new GUIStyle();
                style.padding = new RectOffset(7, 7, 3, 3);
                return style;
            }
        }
        public static GUIStyle MorePaddingScrollView {
            get {
                GUIStyle style = new GUIStyle();
                style.padding = new RectOffset(15, 15, 15, 15);
                return style;
            }
        }

        #endregion

        #region | Buttons & Toggles |

        public static GUIStyle HButtonSelected {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.miniTextField);
                style.normal.textColor = new Color(0.725f, 0.83f, 0.84f);
                style.hover.textColor = new Color(0.725f, 0.83f, 0.84f);
                style.margin = new RectOffset(EditorGUI.indentLevel * 15 + 5, 0, -4, -4);
                style.padding = new RectOffset(7, 0, 1, 1);
                style.alignment = TextAnchor.MiddleLeft;
                return style;
            }
        }

        public static GUIStyle HButton {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.miniButton);
                style.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
                style.hover.textColor = Color.white;
                style.margin = new RectOffset(EditorGUI.indentLevel * 15 + 5, 0, -3, -3);
                style.alignment = TextAnchor.MiddleLeft;
                return style;
            }
        }

        public static GUIStyle HFButtonSelected {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.miniTextField);
                style.normal.textColor = new Color(0.725f, 0.83f, 0.84f);
                style.hover.textColor = new Color(0.725f, 0.83f, 0.84f);
                style.margin = new RectOffset(EditorGUI.indentLevel * 15 + 5, 0, 0, 0);
                style.padding = new RectOffset(7, 0, 0, 0);
                style.alignment = TextAnchor.MiddleLeft;
                style.fixedHeight = 18;
                return style;
            }
        }

        public static GUIStyle HFButton {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.miniButton);
                style.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
                style.hover.textColor = Color.white;
                style.margin = new RectOffset(EditorGUI.indentLevel * 15 + 5, 0, 0, 0);
                style.alignment = TextAnchor.MiddleLeft;
                return style;
            }
        }

        public static GUIStyle TextureButton {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                style.alignment = TextAnchor.MiddleCenter;
                return style;
            }
        }

        public static GUIStyle SquashedButton {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.miniButton);
                style.richText = true;
                return style;
            }
        }

        public static GUIStyle LowerToggle {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.toggle);
                style.fixedHeight = GUI.skin.label.fixedHeight;
                style.alignment = TextAnchor.LowerCenter;
                return style;
            }
        }

        public static GUIStyle SelectedToolbar {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.box);
                style.margin = new RectOffset(0, 0, -10, 0);
                style.alignment = TextAnchor.UpperCenter;
                style.normal.textColor = GUI.skin.label.normal.textColor;
                return style;
            }
        }

        public static GUIStyle ArrangedBoxUnselected {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.box) {
                    normal = { textColor = EditorStyles.label.normal.textColor },
                    margin = new RectOffset(2, 2, 0, 2),
                    fixedHeight = 20,
                }; style.fontSize--;
                return style;
            }
        }

        public static GUIStyle ArrangedButtonSelected {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.numberField) {
                    alignment = TextAnchor.MiddleCenter,
                    margin = new RectOffset(0, 0, 4, 0),
                    normal = { textColor = UIColors.Blue }
                }; return style;
            }
        }

        public static GUIStyle ArrangedLabel {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.label) { 
                    contentOffset = new Vector2(0, 1) 
                }; return style;
            }
        }
        

        #endregion

        #region | Labels |

        public static GUIStyle CenteredLabel {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;
                return style;
            }
        }

        public static GUIStyle CenteredLabelBold {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
                style.alignment = TextAnchor.MiddleCenter;
                return style;
            }
        }

        public static GUIStyle RightAlignedLabel {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleRight;
                return style;
            }
        }

        public static GUIStyle LeftAlignedLabel {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleLeft;
                return style;
            }
        }

        public static GUIStyle ItalicLabel {
            get {
                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.fontStyle = FontStyle.Italic;
                return style;
            }
        }

        public static GUIStyle HelpBoxLabel {
            get {
                GUIStyle style = new GUIStyle(EditorStyles.helpBox);
                style.richText = true;
                style.padding = new RectOffset(4, 0, 1, 1);
                return style;
            }
        }

        #endregion
    }

    #endif
}