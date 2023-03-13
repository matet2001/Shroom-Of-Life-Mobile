using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sentinel.NotePlus
{
    public enum AreaType
    {
        Background,
        Area,
        Window,
        Arrow,
    }

    public enum WindowType
    {
        Window,
        Header,
        TextArea,
        TodoList,
        Color,
        Image,
        Vector,
    }

    public enum RectDirection
    {
        Up,
        UpRight,
        Right,
        RightDown,
        Down,
        DownLeft,
        Left,
        LeftUp,
    }


    /// <summary>
    /// Arrow information between 2 interconnected windows.
    /// </summary>
    public class ArrowData
    {
        public int windowID;
        public int arrowID;
        public static ArrowData zero { get { return new ArrowData(0, 0); } }

        public ArrowData(int windowID, int arrowID) { this.windowID = windowID; this.arrowID = arrowID; }
    }

    // Positions needed to draw Bezier
    public class BezierPosition
    {
        public Vector2 start;
        public Vector2 mid;
        public Vector2 end;

        public BezierPosition(Vector2 start, Vector2 mid, Vector2 end) { this.start = start; this.mid = mid; this.end = end; }
    }

    // Arrow data
    [System.Serializable]
    public class Connect
    {
        public int id;
        public Rect rect;
        public bool moveHandleActive;
        public bool label;
        public bool startArrow = false;
        public bool endArrow = true;
        public string labelText = "";
        public Vector2 offset;
        public Color color = Color.white;

        public Connect(int id, Rect rect) { this.id = id; this.rect = rect; }
    }

    [System.Serializable]
    public class Window
    {
        [HideInInspector] public string className;
        public WindowType type;
        public int id;
        public bool moveHandleActive;
        public bool sizeHandleActive;
        public bool arrowHandleActive;
        public Rect rect;
        public Vector2 windowMinSize;
        public Vector2 windowMaxSize;
        public List<Connect> connects = new List<Connect>();

        // Header
        public Header header = new Header();
        [System.Serializable]
        public class Header
        {
            public string header = "Header";
            public FontStyle fontStyle = FontStyle.Normal;
            public int fontSize = 15;
            public Color fontColor = Color.white;
            public Color backgroundColor = new Color(0, 0, 0, 0.3f);
        }


        // TextArea
        public TextArea textArea = new TextArea();
        [System.Serializable]
        public class TextArea
        {
            public string text = "Text";
            public FontStyle fontStyle = FontStyle.Normal;
            public int fontSize = 15;
            public Color fontColor = Color.white;
        }

        // To-do list
        public TodoList todoList = new TodoList();
        [System.Serializable]
        public class TodoList
        {
            [System.Serializable]
            public class Item
            {
                public bool active;
                public string text = "";
            }

            public List<Item> items = new List<Item>(1) { new Item() };
            public bool header;
            public string headerText = "Header";
        }

        // Color
        public ColorVariable colorVariable = new ColorVariable();
        [System.Serializable]
        public class ColorVariable
        {
            public string hexCode = "FFFFFF";
            public Color color = Color.white;
            public bool RGBAtoHex;
        }

        // Color
        public ImageVariable imageVariable = new ImageVariable();
        [System.Serializable]
        public class ImageVariable
        {
            public string imagePath;
            public ScaleMode scaleMode = ScaleMode.StretchToFill;
            public bool alphaBlend = true;
            public Color color = Color.white;
        }

        // Vector
        public VectorVariable vectorVariable = new VectorVariable();
        [System.Serializable]
        public class VectorVariable
        {
            public enum Type { Vector2,Vector3,Vector4,Quaternion };
            public Type type;
            public string header;
            public Vector2 vector2;
            public Vector3 vector3;
            public Vector4 vector4;
        }

        public Connect GetConnect(int id)
        {
            return connects.Find((x) => x.id == id);
        }

        public void AddConnect(int id, Rect rect)
        {
            if (connects.Find((x) => x.id == id) == null)
                connects.Add(new Connect(id, rect));
        }

        public void RemoveConnect(int id)
        {
            connects.Remove(connects.Find((x) => x.id == id));
        }
    }
}