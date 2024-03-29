using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Sentinel.NotePlus
{
    public static class EditorDraw
    {
        // EXTENTIONS
        public static void Outline(this Rect rect, int size, Color color)
        {
            // Top
            EditorGUI.DrawRect(new Rect(rect.x, rect.y - size, rect.width, size), color);
            // Down
            EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height, rect.width, size), color);
            // Left
            EditorGUI.DrawRect(new Rect(rect.x - size, rect.y - size, size, rect.height + (size * 2)), color);
            // Right
            EditorGUI.DrawRect(new Rect(rect.x + rect.width, rect.y - size, size, rect.height + (size * 2)), color);
        }

        // DRAWS

        // Draws a bezier between the start and end point. Returns the midpoint.
        public static void Bezier (BezierPosition bezierPosition,Texture2D arrowHead,bool startArrow = false,bool endArrow = true, int bezierWidth = 2, int arrowSize = 15, Color color = default(Color))
        {
            if (color == Color.clear)
                color = Color.white;


            Matrix4x4 matrixBackup = GUI.matrix;
            Color colorBackup = GUI.backgroundColor;
            Vector2 direction = Vector2.zero;
            float angle = 0;


            // End Arrow draw
            if (endArrow)
            {
                direction = bezierPosition.mid - bezierPosition.end;
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                GUIUtility.RotateAroundPivot(angle - 90, new Vector2(bezierPosition.end.x, bezierPosition.end.y));
                GUI.color = color;
                GUI.DrawTexture(new Rect(bezierPosition.end.x - (arrowSize / 2), bezierPosition.end.y, arrowSize, arrowSize), arrowHead);
                GUI.color = colorBackup;
                GUI.matrix = matrixBackup;
            }

            // Start Arrow draw
            if (startArrow)
            {
                direction = bezierPosition.mid - bezierPosition.start;
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                GUIUtility.RotateAroundPivot(angle - 90, new Vector2(bezierPosition.start.x, bezierPosition.start.y));
                GUI.color = color;
                GUI.DrawTexture(new Rect(bezierPosition.start.x - (arrowSize / 2), bezierPosition.start.y, arrowSize, arrowSize), arrowHead);
                GUI.color = colorBackup;
                GUI.matrix = matrixBackup;
            }

            // Darw bezier
            Handles.DrawBezier(bezierPosition.start, bezierPosition.end, bezierPosition.mid, bezierPosition.mid, color, null, bezierWidth);
        }

        public static void BezierButton (NoteData noteData, ArrowData selectedArrow, Window window, Connect connect, Texture2D arrowHeadTexture,Texture2D circleTexture)
        {
            bool select = false;
            if (selectedArrow.windowID == window.id & selectedArrow.arrowID == connect.id)
                select = true;

            // Calculate current rect. Adjust the height according to the text length.
            int sizeX = 60;
            if (connect.label)
                sizeX = Mathf.Clamp(connect.labelText.Length * 10, 60, 100);
            connect.rect.size = new Vector2(sizeX, 40);
            int bezierWidth = 2;
            int arrowSize = 15;
            if (select) { bezierWidth = 4; arrowSize = 20;}

            BezierPosition bezierPosition = EditorFunction.GetBezierPosition(window.rect, noteData.GetWindowWithId(connect.id).rect, connect.offset);
            if (bezierPosition.mid != Vector2.zero)
                Bezier(bezierPosition, arrowHeadTexture, connect.startArrow, connect.endArrow, bezierWidth, arrowSize, connect.color);
            else
                return;

            connect.rect.position = new Vector2(bezierPosition.mid.x - (sizeX / 2), bezierPosition.mid.y - 20);

            // Calculate moveHandleActive
            if (connect.rect.Contains(Event.current.mousePosition) & !select & Event.current.type == EventType.MouseDown & Event.current.button == 0)
                connect.moveHandleActive = true;

            // Snap zero offset position.
            if (Event.current.type == EventType.MouseUp & Event.current.button == 0)
            {
                connect.moveHandleActive = false;
                if (connect.offset.magnitude < 20)
                    connect.offset = Vector2.zero;
            }

            if (select)
            {
                if (!connect.label)
                {
                    EditorGUIUtility.AddCursorRect(connect.rect, MouseCursor.MoveArrow);
                    // Move arrow button.
                    connect.offset -= EditorFunction.MouseDeltaDirection(window.rect.position, noteData.GetWindowWithId(connect.id).rect.position);
                    // Draw movemnt circle.
                    GUI.backgroundColor = Color.clear;
                    GUI.Box(new Rect(connect.rect.position.x + connect.rect.width / 2 - 7, connect.rect.position.y + connect.rect.height / 2 - 7, 15, 15), circleTexture);
                    GUI.backgroundColor = Color.white;
                }
            }
            else
            {
                if (connect.label)
                {
                    EditorGUIUtility.AddCursorRect(connect.rect, MouseCursor.MoveArrow);
                    // Move arrow button.
                    if (connect.moveHandleActive)
                        connect.offset -= EditorFunction.MouseDeltaDirection(window.rect.position, noteData.GetWindowWithId(connect.id).rect.position);
                }
            }

            // Draw label
            if (connect.label)
                connect.labelText = ConditionalTextAreaField(connect.rect, connect.labelText, select, connect.color);

            // Draw invisible button
            if (GUI.Button(connect.rect, "", new GUIStyle()))
            {
                selectedArrow.windowID = window.id;
                selectedArrow.arrowID = connect.id;
            }
        }

        /// <summary>
        /// Make a conditional textArea field.
        /// </summary>
        /// <param name="avtive">If active it creates an editable text field, if it is not active it creates a label.</param>
        /// <param name="color">Background Color.</param>
        public static string ConditionalTextAreaField (Rect rect,string label,bool avtive,Color color = default(Color))
        {
            string result = "";
            if (color == Color.clear)
                color = Color.white;

            GUIStyle style = new GUIStyle();
            Color c = GUI.backgroundColor;
            GUI.backgroundColor = color;

            Rect rectResult = new Rect(rect.x,rect.y + 5,rect.width,rect.height - 10);

            if (avtive)
            {
                style = new GUIStyle(GUI.skin.textArea);
                style.alignment = TextAnchor.MiddleCenter;
                result = GUI.TextArea(rectResult, label, style);
            }
            else 
            {
                style = new GUIStyle(GUI.skin.button);
                GUI.Label(rectResult, label, style);
                result = label;
            }
            GUI.backgroundColor = c;
            return result;
        }

        /// <summary>
        /// Finds and sorts all notes in the project.
        /// </summary>
        /// <param name="width">Window lenght.</param>
        /// <returns>Returns the clicked note.</returns>
        public static NoteData AllNoteGridButton (float width)
        {
            NoteData result = null;
            // Find all assets labelled with 'architecture' :
            string[] guids = AssetDatabase.FindAssets("t:" + nameof(NoteData));
            if (guids.Length == 0)
            {
                EditorGUILayout.HelpBox("You have not yet created a note. To create a new note, please click the new note tab.", MessageType.Info);
                return null;
            }

            int columCount = (int)(width / 100);
            if (columCount > guids.Length)
                columCount = guids.Length;
            for (int i = 0; i < guids.Length; i++)
            {
                if (i % columCount == 0)
                    EditorGUILayout.BeginHorizontal();
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                NoteData data = AssetDatabase.LoadAssetAtPath(path, typeof(NoteData)) as NoteData;
                if (GUILayout.Button(new GUIContent(data.name, path), GUILayout.Width(100), GUILayout.Height(100)))
                {
                    result = data;
                }
                if (i % columCount == columCount - 1)
                    EditorGUILayout.EndHorizontal();
            }

            if ((guids.Length / columCount) < 2 & columCount < guids.Length)
            {
                EditorGUILayout.EndHorizontal();
            }

            return result;
        }

        /// <summary>
        ///  Create an area to select folders.
        /// </summary>
        /// <returns>Folder path.</returns>
        public static string FolderField (string directory, string label = "Path")
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(100));
            GUIStyle styleButton = new GUIStyle(GUI.skin.button);
            styleButton.alignment = TextAnchor.MiddleCenter;
            if (GUILayout.Button("o", styleButton, GUILayout.Width(25)))
            {
                directory = Directory.GetCurrentDirectory() + "/Assets";
                directory = EditorUtility.OpenFolderPanel("", directory, "");
            }
            GUILayout.Label(directory);
            GUILayout.EndHorizontal();
            return directory;
        }

        /// <summary>
        /// Create an area to select files.
        /// </summary>
        /// <returns>File path.</returns>
        public static string FileField (string directory,string extension, string label = "Path")
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(100));
            GUIStyle styleButton = new GUIStyle(GUI.skin.button);
            styleButton.alignment = TextAnchor.MiddleCenter;
            if (GUILayout.Button("o", styleButton, GUILayout.Width(25)))
            {
                directory = Directory.GetCurrentDirectory() + "/Assets";
                directory = EditorUtility.OpenFilePanel("", directory, extension);
            }
            GUILayout.Label(directory);
            GUILayout.EndHorizontal();
            return directory;
        }

        // Draws checkered background.
        public static void Background(Rect rect, int snap, Color color)
        {
            GUI.BeginGroup(rect);
            {
                int width = Mathf.RoundToInt(rect.width);
                int height = Mathf.RoundToInt(rect.height);
                for (int y = 0; y < height; y += snap)
                {
                    for (int x = 0; x < width; x += snap)
                    {
                        EditorGUI.DrawRect(new Rect(x, y, 2, 2), color);
                    }
                }
            }
            GUI.EndGroup();
        }

        // Debug error box.
        public static void ErrorWindowLayout(List<string> debugList)
        {
            if (debugList == null)
                return;
            if (debugList.Count == 0)
                return;

            string result = "";
            foreach (var item in debugList)
            {
                result += item + "\n";
            }
            EditorGUILayout.HelpBox(result, MessageType.Error);
        }

        // items to draw if the mouse is in the background.
        public static void MainTool (EditorWindow editorWindow,NoteData noteData,Texture2D[] icons)
        {
            // Test window panel => index = 0
            //if (GUILayout.Button("Window"))
            //{
            //    noteData.AddWindow(WindowType.Window);
            //    editorWindow.Repaint();
            //}

            GUILayout.Space(5);
            string[] array = System.Enum.GetNames(typeof(WindowType));
            for (int i = 1; i < array.Length; i++)
            {
                GUILayout.Space(5);

                string name = array[i];
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(icons[i - 1],GUILayout.Width(45.5f),GUILayout.Height(27.27f)))
                {
                    noteData.AddWindow((WindowType)System.Enum.Parse(typeof(WindowType), name));
                    editorWindow.Repaint();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUIStyle style = new GUIStyle(GUI.skin.label);
                style.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label(array[i], style);
            }
        }

        // Arrow settings tool panel.
        public static void ArrowTool(NoteData noteData,ArrowData arrowData)
        {
            if (arrowData.windowID == 0)
                return;

            Connect connect = noteData.GetWindowWithId(arrowData.windowID).GetConnect(arrowData.arrowID);

            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Arrow", headerStyle);
            HorizontalLine();

            GUILayout.Label("Label", EditorStyles.centeredGreyMiniLabel);
            connect.label = GUILayout.Toggle(connect.label, "");
            GUILayout.Label("Start Arrow", EditorStyles.centeredGreyMiniLabel);
            connect.startArrow = GUILayout.Toggle(connect.startArrow, "");
            GUILayout.Label("End Arrow", EditorStyles.centeredGreyMiniLabel);
            connect.endArrow = GUILayout.Toggle(connect.endArrow, "");
            GUILayout.Label("Color", EditorStyles.centeredGreyMiniLabel);
            connect.color = EditorGUILayout.ColorField(connect.color);
        }

        // Settings to be drawn when the tab with the settings of the windows is opened.
        public static void WindowTool(Window window)
        {
            if (window == null)
                return;

            // Header
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(window.type.ToString(), headerStyle);
            HorizontalLine();

            // Settings
            GUIStyle style = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
            switch (window.type)
            {
                case WindowType.Window:
                    break;
                case WindowType.Header:
                    GUILayout.Label("Font Style", style);
                    window.header.fontStyle = (FontStyle)EditorGUILayout.EnumPopup(window.header.fontStyle);
                    GUILayout.Label("Font Size", style);
                    window.header.fontSize = EditorGUILayout.IntField(window.header.fontSize);
                    GUILayout.Label("Font Color", style);
                    window.header.fontColor = EditorGUILayout.ColorField(window.header.fontColor);
                    GUILayout.Label("Background", style);
                    window.header.backgroundColor = EditorGUILayout.ColorField(window.header.backgroundColor);
                    break;
                case WindowType.TextArea:
                    GUILayout.Label("Font Style", style);
                    window.textArea.fontStyle = (FontStyle)EditorGUILayout.EnumPopup(window.textArea.fontStyle);
                    GUILayout.Label("Font Size", style);
                    window.textArea.fontSize = EditorGUILayout.IntField(window.textArea.fontSize);
                    GUILayout.Label("Font Color", style);
                    window.textArea.fontColor = EditorGUILayout.ColorField(window.textArea.fontColor);
                    break;
                case WindowType.TodoList:
                    GUILayout.Label("Header", style);
                    window.todoList.header = EditorGUILayout.Toggle(window.todoList.header);
                    break;
                case WindowType.Color:
                    GUILayout.Label("RGBA Hex", style);
                    window.colorVariable.RGBAtoHex = EditorGUILayout.Toggle(window.colorVariable.RGBAtoHex);
                    break;
                case WindowType.Image:
                    GUILayout.Label("Scale Mode", style);
                    window.imageVariable.scaleMode = (ScaleMode)EditorGUILayout.EnumPopup(window.imageVariable.scaleMode);
                    GUILayout.Label("Alpha Blend", style);
                    window.imageVariable.alphaBlend = EditorGUILayout.Toggle(window.imageVariable.alphaBlend);
                    GUILayout.Label("Color", style);
                    window.imageVariable.color = EditorGUILayout.ColorField(window.imageVariable.color);
                    break;
                case WindowType.Vector:
                    GUILayout.Label("Type", style);
                    window.vectorVariable.type = (Window.VectorVariable.Type)EditorGUILayout.EnumPopup(window.vectorVariable.type);
                    break;
            }
        }

        // Draw handler icon
        public static void WindowHandler(Rect rect, Texture2D image)
        {
            GUI.backgroundColor = Color.clear;
            GUI.Box(rect, image);
            GUI.backgroundColor = Color.white;
        }

        // Draws a horizontal line on the GUI interface.
        public static void HorizontalLine(Color color = default, int thickness = 1, int padding = 0, int margin = 0)
        {
            color = color != default ? color : Color.grey;
            Rect r = EditorGUILayout.GetControlRect(false, GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding * 0.5f;

            switch (margin)
            {
                // expand to maximum width
                case < 0:
                    r.x = 0;
                    r.width = EditorGUIUtility.currentViewWidth;

                    break;
                case > 0:
                    // shrink line width
                    r.x += margin;
                    r.width -= margin * 2;

                    break;
            }

            EditorGUI.DrawRect(r, color);
        }

        // Draws all windows.
        public static void WindowDraw (Window window,bool selected)
        {
            switch (window.type)
            {
                case WindowType.Window:
                    f_Window();
                    break;
                case WindowType.Header:
                    f_Header();
                    break;
                case WindowType.TextArea:
                    f_TextArea();
                    break;
                case WindowType.TodoList:
                    f_TodoList();
                    break;
                case WindowType.Color:
                    f_Color();
                    break;
                case WindowType.Image:
                    f_Image();
                    break;
                case WindowType.Vector:
                    f_Vector();
                    break;
            }

            void f_Window()
            {
                GUIStyle style = new GUIStyle(GUI.skin.window);
                GUILayout.Box("",style, GUILayout.Width(window.rect.width), GUILayout.Height(window.rect.height));
            }

            void f_Header ()
            {
                Window.Header header = window.header;

                GUIStyle style = new GUIStyle(GUI.skin.box);
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = header.fontSize;
                style.normal.textColor = header.fontColor;
                style.fontStyle = header.fontStyle;

                EditorGUI.DrawRect(new Rect(0, 0, window.rect.width, window.rect.height), header.backgroundColor);
                header.header = GUILayout.TextField(header.header, style,GUILayout.Width(window.rect.width), GUILayout.Height(window.rect.height));
            }

            void f_TextArea ()
            {
                Window.TextArea textArea = window.textArea;

                GUIStyle style = new GUIStyle(GUI.skin.textArea);
                style.alignment = TextAnchor.UpperLeft;
                style.fontSize = textArea.fontSize;
                style.normal.textColor = textArea.fontColor;
                style.fontStyle = textArea.fontStyle;
                style.richText = true;

                textArea.text = GUILayout.TextArea(textArea.text, style, GUILayout.Width(window.rect.width), GUILayout.Height(window.rect.height));
            }

            void f_TodoList()
            {
                Window.TodoList todoList = window.todoList;

                GUIStyle style = new GUIStyle(GUI.skin.window);
                GUI.Box(new Rect(Vector2.zero, window.rect.size), "", style);

                int headerHeight = 30;
                if (todoList.header)
                {
                    GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
                    headerStyle.fontSize = 25;
                    headerStyle.alignment = TextAnchor.MiddleCenter;
                    if (selected)
                        todoList.headerText = GUILayout.TextField(todoList.headerText, headerStyle);
                    else
                    {
                        GUILayout.Label(todoList.headerText, headerStyle, GUILayout.Height(headerHeight));
                    }
                }
                else
                    headerHeight = 0;

                GUI.BeginGroup(new Rect(5, 5, window.rect.width - 10, window.rect.height - 5));
                foreach (var item in todoList.items)
                {
                    GUILayout.BeginHorizontal();
                    item.active = GUILayout.Toggle(item.active, "", GUILayout.Height(20), GUILayout.Width(15));
                    GUIStyle textStyle = new GUIStyle(GUI.skin.textField);
                    if (item.active)
                        textStyle.normal.textColor = Color.gray;
                    item.text = GUILayout.TextField(item.text, textStyle, GUILayout.Height(20));
                    if (item.text == "" & Event.current.keyCode == KeyCode.Backspace & todoList.items.Count > 1 & selected)
                    {
                        EditorGUI.FocusTextInControl(null);
                        todoList.items.Remove(item);
                        GUILayout.EndHorizontal();
                        break;
                    }
                    GUILayout.EndHorizontal();
                }
                window.rect.size = new Vector2(window.rect.width, todoList.items.Count * 22 + 10 + headerHeight);
                if (selected & Event.current.keyCode == KeyCode.Return)
                {
                    EditorGUI.FocusTextInControl(null);
                    if (Event.current.type == EventType.KeyDown)
                        todoList.items.Add(new Window.TodoList.Item());
                }
                GUI.EndGroup();
            }

            void f_Color()
            {
                Window.ColorVariable colorVariable = window.colorVariable;

                GUIStyle style = new GUIStyle(GUI.skin.window);
                GUI.Box(new Rect(Vector2.zero, window.rect.size), "", style);

                GUI.BeginGroup(new Rect(5, 5, window.rect.width - 10, window.rect.height - 10));
                GUI.enabled = selected;
                colorVariable.color = EditorGUILayout.ColorField(colorVariable.color, GUILayout.Width(70),GUILayout.Height(50));
                GUI.enabled = true;
                if (colorVariable.RGBAtoHex)
                    colorVariable.hexCode = ColorUtility.ToHtmlStringRGBA(colorVariable.color);
                else
                    colorVariable.hexCode = ColorUtility.ToHtmlStringRGB(colorVariable.color);
                GUILayout.Space(5);
                GUILayout.TextField(colorVariable.hexCode,GUILayout.Width(70),GUILayout.Height(20));
                GUI.EndGroup();
            }

            void f_Image ()
            {
                Window.ImageVariable imageVariable = window.imageVariable;
                Texture2D image = AssetDatabase.LoadAssetAtPath(imageVariable.imagePath, typeof(Texture2D)) as Texture2D;

                Object obj = null;
                if (selected)
                    DrawImageField(obj);
                else
                {
                    if (image != null)
                        GUI.DrawTexture(new Rect(0, 0, window.rect.width, window.rect.height), image, imageVariable.scaleMode, imageVariable.alphaBlend, 0, imageVariable.color, 0, 0);
                    else
                        DrawImageField(obj);
                }

                void DrawImageField (Object obj)
                {
                    obj = EditorGUI.ObjectField(new Rect(0, 0, window.rect.width - 20, window.rect.height - 20), image, typeof(Texture2D), true);
                    if (obj != null)
                        imageVariable.imagePath = AssetDatabase.GetAssetPath(obj);
                }
            }

            void f_Vector()
            {
                Window.VectorVariable vectorVariable = window.vectorVariable;

                GUIStyle style = new GUIStyle(GUI.skin.window);
                GUI.Box(new Rect(Vector2.zero, window.rect.size), "", style);

                GUI.BeginGroup(new Rect(5, 5, window.rect.width - 10, window.rect.height - 10));
                EditorGUILayout.BeginHorizontal();

                if (selected)
                    vectorVariable.header = GUILayout.TextField(vectorVariable.header, GUILayout.MinWidth(50));
                else
                    GUILayout.Label(vectorVariable.header, GUILayout.MinWidth(50));

                switch (vectorVariable.type)
                {
                    case Window.VectorVariable.Type.Vector2:
                        vectorVariable.vector2 = EditorGUILayout.Vector2Field("", vectorVariable.vector2, GUILayout.MinWidth(50));
                        break;
                    case Window.VectorVariable.Type.Vector3:
                        vectorVariable.vector3 = EditorGUILayout.Vector3Field("", vectorVariable.vector3, GUILayout.MinWidth(60));
                        break;
                    case Window.VectorVariable.Type.Vector4:
                        vectorVariable.vector4 = EditorGUILayout.Vector4Field("", vectorVariable.vector4, GUILayout.MinWidth(100));
                        break;
                    case Window.VectorVariable.Type.Quaternion:
                        vectorVariable.vector4 = EditorGUILayout.Vector4Field("", vectorVariable.vector4, GUILayout.MinWidth(100));
                        break;
                }
                GUILayout.Space(10);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();

                switch (vectorVariable.type)
                {
                    case Window.VectorVariable.Type.Vector2:
                        if (GUILayout.Button("Copy"))
                            GUIUtility.systemCopyBuffer = "Vector2" + vectorVariable.vector2.ToString();
                        if (GUIUtility.systemCopyBuffer.Contains("Vector2"))
                            if (GUILayout.Button("Paste"))
                                vectorVariable.vector2 = EditorFunction.StringToVector2(GUIUtility.systemCopyBuffer);
                        break;
                    case Window.VectorVariable.Type.Vector3:
                        if (GUILayout.Button("Copy"))
                            GUIUtility.systemCopyBuffer = "Vector3" + vectorVariable.vector3.ToString();
                        if (GUIUtility.systemCopyBuffer.Contains("Vector3"))
                            if (GUILayout.Button("Paste"))
                                vectorVariable.vector3 = EditorFunction.StringToVector3(GUIUtility.systemCopyBuffer);
                        break;
                    case Window.VectorVariable.Type.Vector4:
                        if (GUILayout.Button("Copy"))
                            GUIUtility.systemCopyBuffer = "Vector4" + vectorVariable.vector4.ToString();
                        if (GUIUtility.systemCopyBuffer.Contains("Vector4"))
                            if (GUILayout.Button("Paste"))
                                vectorVariable.vector4 = EditorFunction.StringToVector4(GUIUtility.systemCopyBuffer);
                        break;
                    case Window.VectorVariable.Type.Quaternion:
                        if (GUILayout.Button("Copy"))
                            GUIUtility.systemCopyBuffer = "Quaternion" + vectorVariable.vector4.ToString();
                        if (GUIUtility.systemCopyBuffer.Contains("Quaternion"))
                            if (GUILayout.Button("Paste"))
                                vectorVariable.vector4 = EditorFunction.StringToQuaternion(GUIUtility.systemCopyBuffer);
                        break;
                }
                GUILayout.Space(10);
                EditorGUILayout.EndHorizontal();
                GUI.EndGroup();
            }
        }
    }
}