using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class MainWindow : EditorWindow
{
    public GameObject propList;
    public Texture2D tex;
    public Texture2D tex2;
    public Texture2D buttonTextureA;
    public Texture2D buttonTextureB;


    private int _height = 10;
    private int _width = 10;
    private bool[,] grid = new bool[10, 10];
    private int _propCount;
    private float _roomSeparation = 1;
    private List<GameObject> _nodeList = new List<GameObject>();
    private int _column;
    private int _row;
    private GameObject _room1;
    private GameObject _room2A;
    private GameObject _room2B;
    private GameObject _room3;
    private GameObject _room4;

    private static Generator _generator;

    private static readonly GUIStyle _style      = new GUIStyle(EditorStyles.label);
    private static readonly GUIStyle _titleStyle = new GUIStyle(EditorStyles.label);
    private static readonly GUIStyle _errorStyle = new GUIStyle(EditorStyles.label);

    [MenuItem("CustomTools/MapGenerator")]
    public static void OpenWindow()
    {
        MainWindow window = GetWindow<MainWindow>();

        window.wantsMouseMove = true;

        window.minSize = new Vector2(500, 710);
        
        _generator = new Generator();
        
        _style.normal.textColor = Color.white;
        _errorStyle.normal.textColor = Color.red;
    }

    private void OnGUI()
    {
        _style.normal.textColor = Color.white;
        _errorStyle.normal.textColor = Color.red;
        _titleStyle.normal.textColor = Color.white;
        _titleStyle.fontSize = 20;

        EditorGUILayout.BeginHorizontal();

        //EditorGUI.LabelField(new Rect(0, 10, 200, 200), "Dungeon Generator ", _titleStyle);
          
        EditorGUILayout.EndHorizontal();
        
        if (tex != null)
            GUI.DrawTexture(new Rect(0, 0, position.width, position.height), tex, ScaleMode.StretchToFill);
        
        if (tex2 != null)
            GUI.DrawTexture(new Rect(0, 0, position.width, 170 + _height * 19), tex2, ScaleMode.StretchToFill);

        EditorGUILayout.BeginHorizontal();
        GUI.Label(new Rect(0, 30, 200,25),"One entrance Room",_style);
        _room1 = (GameObject)EditorGUI.ObjectField(new Rect(position.width / 2 , 30, 200,15),_room1, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUI.Label(new Rect(0, 50, 200,25),"Two entrances 'A' Room",_style);
        _room2A = (GameObject) EditorGUI.ObjectField(new Rect(position.width / 2, 50, 200,15),_room2A, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUI.Label(new Rect(0, 70, 200,25),"Two entrances 'B' Room", _style);
        _room2B = (GameObject) EditorGUI.ObjectField(new Rect(position.width / 2, 70, 200, 15),_room2B, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUI.Label(new Rect(0, 90, 200, 25),"Three entrances Room", _style);
        _room3 = (GameObject) EditorGUI.ObjectField(new Rect(position.width / 2, 90, 200, 15), _room3, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUI.Label(new Rect(0, 110, 200, 25),"Four entrances Room", _style);
        _room4 = (GameObject) EditorGUI.ObjectField(new Rect(position.width / 2, 110, 200, 15), _room4, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        DrawGridConfig();
        //int column = EditorGUILayout.IntField(_column, _style);
        //if (column <= 10)
        //    _column = column;
        //else
        //    _column = 10;
        //if (GUILayout.Button("C", GUILayout.Width(20), GUILayout.Height(20)))
        //    SelectColumn(_column - 1);

        //int row = EditorGUILayout.IntField(_row, _style);
        //if (row <= 10)
        //    _row = row;
        //else
        //    _row = 10;
        //if (GUILayout.Button("R", GUILayout.Width(20), GUILayout.Height(20)))
        //    SelectRow(_row -1);

        DrawPropConfig();
        
        if (GUI.Button(new Rect(position.width / 2 - 100, 610, 200, 25), "Invert All Grid"))
            InvertGrid();
         
        if (GUI.Button(new Rect(position.width / 2 - 100, 640, 200, 25),"Generate"))
            Generate();
        
        if (GUI.Button(new Rect(position.width / 2 - 100, 670, 200, 25),"Delete Map"))
            DeleteMap();
    }

    private void DrawPropConfig()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUI.LabelField(new Rect(0,570, 200, 25),"Prop Count", _style);
        _propCount = EditorGUI.IntField(new Rect(position.width / 2, 570, 200, 15), _propCount);

        EditorGUILayout.EndHorizontal();

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty stringsProperty = so.FindProperty("propList");

        EditorGUILayout.BeginHorizontal();

        EditorGUI.LabelField(new Rect(0, 590, 200, 25),"Prop List", _style);
        EditorGUI.ObjectField(new Rect(position.width /2, 590, 200, 15),stringsProperty, new GUIContent(""));

        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawGridConfig()
    {
        bool error = false;

        EditorGUILayout.BeginHorizontal();

        EditorGUI.LabelField(new Rect(0, 130, 200, 15),"Matrix Height", _style);
        _height = EditorGUI.IntSlider(new Rect(100, 130, position.width -110, 15), _height, 1, 10);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUI.LabelField(new Rect(0, 150, 200, 15), "Matrix Width", _style);
        _width = EditorGUI.IntSlider(new Rect(100, 150, position.width - 110, 15), _width, 1, 10);
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();

        EditorGUI.LabelField(new Rect(0, 170, 200, 15), "Room Separation", _style);
        _roomSeparation = EditorGUI.FloatField(new Rect(100, 170, 200, 15), _roomSeparation, _style);
        
        EditorGUILayout.EndHorizontal();

        if (_roomSeparation < 1)
        {
            GUILayout.Label("The room separation must be greater than 1!", _errorStyle);
            error = true;
        }
        
        if (_width != grid.GetLength(0) || _height != grid.GetLength(1))
            grid = new bool[_width, _height];
        
        if (!error)
            DrawGrid();
    }
    
    private void DrawGrid()
    {
        EditorGUILayout.BeginHorizontal();
        for (int x = 0; x < _width; x++)
        {
            EditorGUILayout.BeginVertical();
            for (int y = 0; y < _height; y++)
            {
                grid[x, y] = ButtonCheck(x,y);
                  
                if (GUI.Button(new Rect(60 + (35 * _width), 192.5f + (35 * y), 20, 20), "<"))
                {
                    SelectRow(y);
                }
            }

            if (GUI.Button(new Rect(62.5f + (35 * x), 190 + (35 * _height), 20, 20), "^"))
            {
                SelectColumn(x); 
            }

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }

    void SelectColumn(int columnNumber)
    {
        int counter = 0;

        EditorGUILayout.BeginVertical();
        for (int y = 0; y < _height; y++)
        {
            if (!grid[columnNumber, y])
            {
                grid[columnNumber, y] = true;
                counter++;
            }
        }
        if (counter == 0)
        {
            for (int y = 0; y < _height; y++)
            {
                grid[columnNumber, y] = false; 
            }
        }
        EditorGUILayout.EndVertical();
    }

    void SelectRow(int rowNumber)
    {
        int counter = 0;

        EditorGUILayout.BeginVertical();
        for (int x = 0; x < _height; x++)
        {
            if (!grid[x, rowNumber])
            {
                grid[x, rowNumber] = true;
                counter++;
            }
        }
        if (counter == 0)
        {
            for (int x = 0; x < _height; x++)
            {
                grid[x, rowNumber] = false;
            }
        }
        EditorGUILayout.EndVertical();
    }

    bool ButtonCheck(int x, int y)
    {
        Texture2D buttonTexture;
        if (grid[x, y]) buttonTexture = buttonTextureA;
        else buttonTexture = buttonTextureB;

        if (GUI.Button(new Rect(60 +(35 * x),190+(35 * y), 25,25),buttonTexture))
            return !grid[x, y];

        else return grid[x, y];
    }

    void InvertGrid()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                grid[x, y] = !grid[x, y];
            }
        }
    }

    private void Generate()
    {
        DeleteMap(false);
        
        _generator.SetParameters(grid, _width,_height, _roomSeparation, _room1, _room2A, _room2B, _room3, _room4);
        _generator.GenerateDungeon();

        _nodeList = _generator.GetNodes();
        
        
        if(_nodeList.Count > 0)
            Debug.Log("Map generated successfully.");
        else
            Debug.LogError("There are no nodes selected to generate the map!");
    }

    private void DeleteMap(bool showMessage = true)
    {
        if (_nodeList.Count == 0 && showMessage)
        {
            _nodeList = _generator.GetNodes();

            if (_nodeList.Count == 0)
            {
                Debug.LogWarning("There is no map to delete!");
                return;   
            }
        }

        foreach (GameObject item in _nodeList)
        {
            DestroyImmediate(item);
        }
        
        _nodeList.Clear();
        
        if (showMessage)
            Debug.LogWarning("Map deleted successfully.");
    }
}
