using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class MainWindow : EditorWindow
{
    public Texture2D buttonTextureA;
    public Texture2D buttonTextureB;


    private static int _columns = 10;
    private static int _rows = 10;
    private static bool[,] grid = new bool[10, 10];
    private static float _roomSeparation = 1;
    private List<GameObject> _nodeList = new List<GameObject>();
    private static GameObject _room1;
    private static GameObject _room2A;
    private static GameObject _room2B;
    private static GameObject _room3;
    private static GameObject _room4;

    private static Generator _generator;

    private static readonly GUIStyle _titleStyle = new GUIStyle(EditorStyles.label);
    private static readonly GUIStyle _style      = new GUIStyle(EditorStyles.label);
    private static readonly GUIStyle _errorStyle = new GUIStyle(EditorStyles.label);

    [MenuItem("CustomTools/MapGenerator")]
    public static void OpenWindow()
    {
        MainWindow window = GetWindow<MainWindow>();

        window.wantsMouseMove = true;

        window.minSize = new Vector2(480, 630);
        
        _generator = new Generator();
        
        _style.normal.textColor = Color.white;
        _errorStyle.normal.textColor = Color.red;
    }

    private void OnGUI()
    {
        _style.normal.textColor = Color.black;
        _errorStyle.normal.textColor = Color.red;
        _titleStyle.normal.textColor = Color.black;
        _titleStyle.fontSize = 20;

        EditorGUILayout.BeginHorizontal();
        EditorGUI.LabelField(new Rect(position.width / 2 - 100, 0, 200, 200), "Dungeon Generator ", _titleStyle);
        EditorGUILayout.EndHorizontal();

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
        
        if (GUI.Button(new Rect(position.width / 2 - 205, 570, 200, 25), "Invert All Grid"))
            InvertGrid();

        if (GUI.Button(new Rect(position.width / 2 + 5  , 570, 200, 25), "Clear Grid"))
            ClearGrid();

        if (GUI.Button(new Rect(position.width / 2 - 205, 600, 200, 25),"Generate"))
            Generate();
         
        if (GUI.Button(new Rect(position.width / 2 + 5  , 600, 200, 25),"Delete Map"))
            DeleteMap();
    }
    
    private void DrawGridConfig()
    {
        bool error = false;

        EditorGUILayout.BeginHorizontal();

        EditorGUI.LabelField(new Rect(0, 130, 200, 15),"Matrix Rows", _style);
        _columns = EditorGUI.IntSlider(new Rect(100, 130, position.width -110, 15), _columns, 1, 10);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUI.LabelField(new Rect(0, 150, 200, 15), "Matrix Columns", _style);
        _rows = EditorGUI.IntSlider(new Rect(100, 150, position.width - 110, 15), _rows, 1, 10);
        
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
        
        if (_rows != grid.GetLength(0) || _columns != grid.GetLength(1))
            grid = new bool[_rows, _columns];
        
        if (!error)
            DrawGrid();
    }
    
    private void DrawGrid()
    {
        EditorGUILayout.BeginHorizontal();
        for (int x = 0; x < _rows; x++)
        {
            EditorGUILayout.BeginVertical();
            for (int y = 0; y < _columns; y++)
            {
                grid[x, y] = ButtonCheck(x,y);
                  
                if (GUI.Button(new Rect(60 + (35 * _rows), 192.5f + (35 * y), 20, 20), "<"))
                {
                    SelectRow(y);
                }
            }

            if (GUI.Button(new Rect(62.5f + (35 * x), 190 + (35 * _columns), 20, 20), "^"))
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
        for (int y = 0; y < _columns; y++)
        {
            if (!grid[columnNumber, y])
            {
                grid[columnNumber, y] = true;
                counter++;
            }
        }
        if (counter == 0)
        {
            for (int y = 0; y < _columns; y++)
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
        for (int x = 0; x < _columns; x++)
        {
            if (!grid[x, rowNumber])
            {
                grid[x, rowNumber] = true;
                counter++;
            }
        }
        if (counter == 0)
        {
            for (int x = 0; x < _columns; x++)
            {
                grid[x, rowNumber] = false;
            }
        }
        EditorGUILayout.EndVertical();
    }

    bool ButtonCheck(int x, int y)
    {
        Texture2D buttonTexture;

        if (grid[x, y]) 
            buttonTexture = buttonTextureA;
        else
            buttonTexture = buttonTextureB;

        if (GUI.Button(new Rect(60 + (35 * x), 190 + (35 * y), 25, 25), buttonTexture))
            return !grid[x, y];
        else 
            return grid[x, y];
    }

    void InvertGrid()
    {
        for (int x = 0; x < _rows; x++)
        {
            for (int y = 0; y < _columns; y++)
            {
                grid[x, y] = !grid[x, y];
            }
        }
    }
    void ClearGrid()
    {
        for (int x = 0; x < _rows; x++)
        {
            for (int y = 0; y < _columns; y++)
            {
                grid[x, y] = false;
            }
        }
    }

    private void Generate()
    {
        DeleteMap(false);

        if (_room1 == null || _room2A == null || _room2B == null || _room3 == null || _room4 == null)
        {
            Debug.LogError("There are prefabs that are not assigned!");
            return;
        }

        _generator.SetParameters(grid, _columns, _rows, _roomSeparation, _room1, _room2A, _room2B, _room3, _room4);
        _generator.GenerateDungeon();

        _nodeList = _generator.GetNodes();

        if (_nodeList.Count == 0)
        {
            Debug.LogError("There are no nodes selected to generate the map!");
            return;
        }

        Debug.Log("Map generated successfully.");
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
