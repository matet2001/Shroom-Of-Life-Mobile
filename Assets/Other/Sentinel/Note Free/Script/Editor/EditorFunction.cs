using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;

namespace Sentinel.NotePlus
{
    public static class EditorFunction
    {
        // EXTENTIONS

        /// <summary>
        /// Scales the window according to the delta position of the mouse.
        /// </summary>
        public static void ResizeMouseDelta(this Window window)
        {
            float deltaX = Event.current.delta.x * 0.5f;
            float deltaY = Event.current.delta.y * 0.5f;

            bool widthMaxLimit = false;
            bool heightMaxLimit = false;

            if (window.windowMaxSize == Vector2.zero)
            {
                widthMaxLimit = true;
                heightMaxLimit = true;
            }
            else
            {
                widthMaxLimit = (window.rect.width + deltaX) < window.windowMaxSize.x;
                heightMaxLimit = (window.rect.height + deltaY) < window.windowMaxSize.y;
            }

            if ((window.rect.width + deltaX) > window.windowMinSize.x & widthMaxLimit)
            {
                window.rect.xMax += deltaX;
            }
            if ((window.rect.height + deltaY) > window.windowMinSize.y & heightMaxLimit)
            {
                window.rect.yMax += deltaY;
            }
        }

        /// <summary>
        /// Rotate by angel.
        /// </summary>
        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            float tx = v.x;
            float ty = v.y;

            return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
        }

        /// <summary>
        /// Calculates the rect position relative to the parent.
        /// </summary>
        public static Rect SetDirection(this Rect rect, Rect _parent, RectDirection direction)
        {
            switch (direction)
            {
                case RectDirection.Up:
                    return new Rect(new Vector2((_parent.width / 2) - (rect.width / 2), 0), rect.size);
                case RectDirection.UpRight:
                    return new Rect(new Vector2(_parent.width - rect.width, 0), rect.size);
                case RectDirection.Right:
                    return new Rect(new Vector2(_parent.width - rect.width, (_parent.height / 2) - (rect.height / 2)), rect.size);
                case RectDirection.RightDown:
                    return new Rect(new Vector2(_parent.width - rect.width, _parent.height - rect.height), rect.size);
                case RectDirection.Down:
                    return new Rect(new Vector2((_parent.width / 2) - (rect.width / 2), _parent.height - rect.height), rect.size);
                case RectDirection.DownLeft:
                    return new Rect(new Vector2(0, _parent.height - rect.height), rect.size);
                case RectDirection.Left:
                    return new Rect(new Vector2(0, (_parent.height / 2) - (rect.height / 2)), rect.size);
                case RectDirection.LeftUp:
                    return rect;
                default:
                    return rect;
            }
        }

        /// <summary>
        /// Create note data and write all byte data.
        /// </summary>
        public static void NoteDataImport(this NoteData noteData, string path)
        {
            StreamReader reader = new StreamReader(path);
            string data = reader.ReadToEnd();
            if (path.Contains(".snpd"))
                data = AESDecryption(data);

            JsonUtility.FromJsonOverwrite(data, noteData);
        }

        // FUNCTIONS

        /// <summary>
        /// The area where the mouse is located relative to the colliders.
        /// </summary>
        /// <param name="windows">Windows collider.</param>
        /// <param name="otherArea">Other collider.</param>
        /// <param name="offset">Offset.</param>
        /// <returns>Current Area Type.</returns>
        public static AreaType OnCurrentMouseItem(Window[] windows, Rect[] otherArea, Vector2 offset)
        {
            AreaType result = AreaType.Background;

            // Window control
            foreach (var item in windows)
                if (item.rect.Contains(Event.current.mousePosition - offset))
                {
                    result = AreaType.Window;
                    break;
                }

            // Window arrow button control
            foreach (Window window in windows)
                foreach (Connect connect in window.connects)
                {
                    if (connect.rect.Contains(Event.current.mousePosition - offset))
                    {
                        result = AreaType.Arrow;
                        break;
                    }
                }

            // Other areas control
            foreach (Rect item in otherArea)
                if (item.Contains(Event.current.mousePosition))
                {
                    result = AreaType.Area;
                    break;
                }

            return result;
        }

        /// <summary>
        /// Returns true if the object is deleted.
        /// </summary>
        /// <param name="windowID">Selected window id.</param>
        /// <param name="data"></param>
        public static bool DeleteWindow(int windowID, NoteData data)
        {
            bool result = false;
            // Delete select window.
            if (windowID != 0)
                if (Event.current.type == EventType.KeyDown & Event.current.keyCode == KeyCode.Delete)
                {
                    result = true;
                    data.RemoveWindow(windowID);
                }

            return result;
        }

        /// <summary>
        /// Returns true if the object is deleted.
        /// </summary>
        /// <param name="arrowData">Selected arrow data.</param>
        /// <param name="data"></param>
        public static bool DeleteArrow(ArrowData arrowData, NoteData data)
        {
            bool result = false;
            // Delete select arrow.
            if (arrowData.windowID != 0)
                if (Event.current.type == EventType.KeyDown & Event.current.keyCode == KeyCode.Delete)
                {
                    result = true;
                    data.GetWindowWithId(arrowData.windowID).RemoveConnect(arrowData.arrowID);
                }

            return result;
        }

        /// <summary>
        /// Recalculates the position according to the specific direction.
        /// </summary>
        public static Vector2 MouseDeltaDirection (Vector2 position,Vector2 target)
        {
            Vector2 dir = target - position;
            float angel = Mathf.Atan2(-dir.y, dir.x) * Mathf.Rad2Deg;
            return Event.current.delta.Rotate(angel) * 0.5f;
        }

        /// <summary>
        /// It serializes note data and generates json file.
        /// </summary>
        public static void NoteDataExport (string path, NoteData noteData,bool encryption)
        {
            string extension = ".txt";
            if (encryption)
                extension = ".snpd";
            string pathResult = Path.Combine(path, noteData.name + extension);
            using (StreamWriter writer = new StreamWriter(pathResult,false))
            {
                string data = JsonUtility.ToJson(noteData);
                if (encryption)
                    data = AESEncryption(data);
                writer.Write(data);
            }//File written
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Encrypts with advanced encryption standard.
        /// </summary>
        public static string AESEncryption(string inputData)
        {
            string key = "A60A5770FE5E7AB200BA9CFC94E4E8B0";
            string iv = "1234567887654321";

            AesCryptoServiceProvider AEScryptoProvider = new AesCryptoServiceProvider();
            AEScryptoProvider.BlockSize = 128;
            AEScryptoProvider.KeySize = 256;
            AEScryptoProvider.Key = ASCIIEncoding.ASCII.GetBytes(key);
            AEScryptoProvider.IV = ASCIIEncoding.ASCII.GetBytes(iv);
            AEScryptoProvider.Mode = CipherMode.CBC;
            AEScryptoProvider.Padding = PaddingMode.PKCS7;

            byte[] txtByteData = ASCIIEncoding.ASCII.GetBytes(inputData);
            ICryptoTransform trnsfrm = AEScryptoProvider.CreateEncryptor(AEScryptoProvider.Key, AEScryptoProvider.IV);

            byte[] result = trnsfrm.TransformFinalBlock(txtByteData, 0, txtByteData.Length);
            return Convert.ToBase64String(result);
        }

        /// <summary>
        ///  Decryption with advanced encryption standard.
        /// </summary>
        public static string AESDecryption(string inputData)
        {
            string key = "A60A5770FE5E7AB200BA9CFC94E4E8B0";
            string iv = "1234567887654321";

            AesCryptoServiceProvider AEScryptoProvider = new AesCryptoServiceProvider();
            AEScryptoProvider.BlockSize = 128;
            AEScryptoProvider.KeySize = 256;
            AEScryptoProvider.Key = ASCIIEncoding.ASCII.GetBytes(key);
            AEScryptoProvider.IV = ASCIIEncoding.ASCII.GetBytes(iv);
            AEScryptoProvider.Mode = CipherMode.CBC;
            AEScryptoProvider.Padding = PaddingMode.PKCS7;

            byte[] txtByteData = Convert.FromBase64String(inputData);
            ICryptoTransform trnsfrm = AEScryptoProvider.CreateDecryptor();

            byte[] result = trnsfrm.TransformFinalBlock(txtByteData, 0, txtByteData.Length);
            return ASCIIEncoding.ASCII.GetString(result);
        }

        /// <summary>
        /// Draw window handler
        /// </summary>
        /// <param name="rect">Handler size.</param>
        /// <param name="handleActive">Handle active.</param>
        /// <param name="mouseCursor">Mouse cursor type.</param>
        /// <returns></returns>
        public static bool WindowHandler(Rect rect, bool handleActive, MouseCursor mouseCursor)
        {
            if (GUIUtility.hotControl == 0)
                handleActive = false;
            EditorGUIUtility.AddCursorRect(rect, mouseCursor);
            bool action = (Event.current.type == EventType.MouseDown) || (Event.current.type == EventType.MouseDrag);
            if (!handleActive && action)
            {
                if (rect.Contains(Event.current.mousePosition, true))
                {
                    handleActive = true;
                    GUIUtility.hotControl = EditorGUIUtility.GetControlID(FocusType.Passive);
                }
            }
            return handleActive;
        }

        /// <summary>
        /// Calculates the middle, start and end points between two windows.Return bezier positions.
        /// </summary>
        /// <param name="startRect">Start window rect.</param>
        /// <param name="endRect">End window rect.</param>
        /// <param name="offset">Mid position offset.</param>
        public static BezierPosition GetBezierPosition(Rect startRect, Rect endRect, Vector2 offset)
        {
            Vector2 startPos = new Vector2(startRect.x + startRect.width / 2, startRect.y + startRect.height / 2);
            Vector2 endPos = new Vector2(endRect.x + endRect.width / 2, endRect.y + endRect.height / 2);
            Vector2 r_dotPos = Vector2.zero;
            Vector2 r_arrowPos = Vector2.zero;
            Vector2 r_midPos = Vector2.zero;

            // Point closest to end window
            int lenght = (int)Vector3.Distance(startPos, endPos);
            for (int i = 0; i < lenght; i++)
            {
                r_dotPos = Vector3.Lerp(startPos, endPos, (float)i / lenght);
                if (!startRect.Contains(r_dotPos))
                    break;
            }

            for (int i = lenght; i > 1; i--)
            {
                r_arrowPos = Vector2.Lerp(startPos, endPos, (float)i / lenght);
                if (!endRect.Contains(r_arrowPos))
                {
                    Vector2 horizontalAxis = (r_dotPos - r_arrowPos).normalized;
                    Vector2 verticalAxis = Vector2.Perpendicular(horizontalAxis);
                    Vector2 tangentResult = horizontalAxis * offset.x + verticalAxis * offset.y;
                    r_midPos = Vector2.Lerp(r_dotPos, r_arrowPos, 0.5f) + tangentResult;
                    break;
                }
            }

            return new BezierPosition(r_dotPos, r_midPos, r_arrowPos);
        }

        // CONVERT FUNCTIONS

        public static Vector2 StringToVector2(string sVector)
        {
            sVector = sVector.Replace("Vector2", string.Empty);

            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            if (sArray[0].Contains('.'))
                sArray[0] = sArray[0].Replace('.', ',');
            if (sArray[1].Contains('.'))
                sArray[1] = sArray[1].Replace('.', ',');

            float x = float.Parse(sArray[0]);
            float y = float.Parse(sArray[1]);
            return new Vector2(x, y);
        }

        public static Vector3 StringToVector3(string sVector)
        {
            sVector = sVector.Replace("Vector3", string.Empty);

            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            if (sArray[0].Contains('.'))
                sArray[0] = sArray[0].Replace('.', ',');
            if (sArray[1].Contains('.'))
                sArray[1] = sArray[1].Replace('.', ',');
            if (sArray[2].Contains('.'))
                sArray[2] = sArray[2].Replace('.', ',');

            float x = float.Parse(sArray[0]);
            float y = float.Parse(sArray[1]);
            float z = float.Parse(sArray[2]);
            return new Vector3(x, y, z);
        }

        public static Vector4 StringToVector4(string sVector)
        {
            sVector = sVector.Replace("Vector4", string.Empty);

            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            if (sArray[0].Contains('.'))
                sArray[0] = sArray[0].Replace('.', ',');
            if (sArray[1].Contains('.'))
                sArray[1] = sArray[1].Replace('.', ',');
            if (sArray[2].Contains('.'))
                sArray[2] = sArray[2].Replace('.', ',');
            if (sArray[3].Contains('.'))
                sArray[3] = sArray[3].Replace('.', ',');

            float x = float.Parse(sArray[0]);
            float y = float.Parse(sArray[1]);
            float z = float.Parse(sArray[2]);
            float w = float.Parse(sArray[3]);
            return new Vector4(x, y, z, w);
        }

        public static Vector4 StringToQuaternion(string sVector)
        {
            sVector = sVector.Replace("Quaternion", string.Empty);

            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            if (sArray[0].Contains('.'))
                sArray[0] = sArray[0].Replace('.', ',');
            if (sArray[1].Contains('.'))
                sArray[1] = sArray[1].Replace('.', ',');
            if (sArray[2].Contains('.'))
                sArray[2] = sArray[2].Replace('.', ',');
            if (sArray[3].Contains('.'))
                sArray[3] = sArray[3].Replace('.', ',');

            float x = float.Parse(sArray[0]);
            float y = float.Parse(sArray[1]);
            float z = float.Parse(sArray[2]);
            float w = float.Parse(sArray[3]);
            return new Vector4(x, y, z, w);
        }
    }
}