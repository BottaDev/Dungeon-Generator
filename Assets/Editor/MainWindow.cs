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

    private static readonly GUIStyle _style = new GUIStyle(EditorStyles.label);
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

        EditorGUI.LabelField(new Rect(position.width / 2 -100, 0, 200, 25), "Dungeon Generator", _titleStyle);

        EditorGUILayout.EndHorizontal();
        
        if (tex != null)
            GUI.DrawTexture(new Rect(0, 0, position.width, position.height), tex, ScaleMode.StretchToFill);
        
        if (tex2 != null)
            GUI.DrawTexture(new Rect(0, 0, position.width, 170 + _height * 19), tex2, ScaleMode.StretchToFill);


        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        DrawGridConfig();
        //int column = EditorGUILayout.IntField(_column, _style);
        //if (column <= 10)
        //    _column = column;
        //else
        //    _column = 10;
        //if (GUILayout.Button("C", GUILayout.Width(20), GUILayout.Height(20)))
        //    SelectColumn(_column -1);

        //int row = EditorGUILayout.IntField(_row, _style);
        //if (row <= 10)
        //    _row = row;
        //else
        //    _row = 10;
        //if (GUILayout.Button("R", GUILayout.Width(20), GUILayout.Height(20)))
        //    SelectRow(_row -1);
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //if (GUILayout.Button("Invert All Grid", GUILayout.Height(40)))
        //    InvertGrid();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        //DrawPropConfig();
        
        EditorGUILayout.Space();
        
        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Label("One entrance Room");
        //_room1 = (GameObject) EditorGUILayout.ObjectField(_room1, typeof(GameObject), true);
        //EditorGUILayout.EndHorizontal();
        
        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Label("Two entrances 'A' Room");
        //_room2A = (GameObject) EditorGUILayout.ObjectField(_room2A, typeof(GameObject), true);
        //EditorGUILayout.EndHorizontal();
        
        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Label("Two entrances 'B' Room");
        //_room2B = (GameObject) EditorGUILayout.ObjectField(_room2B, typeof(GameObject), true);
        //EditorGUILayout.EndHorizontal();
        
        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Label("Three entrances Room");
        //_room3 = (GameObject) EditorGUILayout.ObjectField(_room3, typeof(GameObject), true);
        //EditorGUILayout.EndHorizontal();
        
        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Label("Four entrances Room");
        //_room4 = (GameObject) EditorGUILayout.ObjectField(_room4, typeof(GameObject), true);
        //EditorGUILayout.EndHorizontal();
        

        //if (GUILayout.Button("Generate", GUILayout.Height(40)))
        //    Generate();
        EditorGUILayout.Space();
        
        //if (GUILayout.Button("Delete Map", GUILayout.Height(40)))
        //    DeleteMap();
    }

    private void DrawPropConfig()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Prop Count", _style);
        _propCount = EditorGUILayout.IntField( _propCount);

        EditorGUILayout.EndHorizontal();

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty stringsProperty = so.FindProperty("propList");

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Prop List", _style);
        EditorGUILayout.ObjectField(stringsProperty, new GUIContent(""));

        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawGridConfig()
    {
        bool error = false;

        EditorGUILayout.BeginHorizontal();

        EditorGUI.LabelField(new Rect(0, 19, 200, 15),"Matrix Height", _style);
        _height = EditorGUI.IntSlider(new Rect(100, 19, position.width -110, 15), _height, 1, 10);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        EditorGUI.LabelField(new Rect(0, 45, 200, 15), "Matrix Width", _style);
        _width = EditorGUI.IntSlider(new Rect(100, 44, position.width - 110, 15), _width, 1, 10);
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();

        _roomSeparation = EditorGUI.FloatField(new Rect(0, 60, 200, 15),"Room Separation", _roomSeparation, _style);
        
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

                //grid[x, y] = ButtonCheck(x,y);
                grid[x, y] = EditorGUI.Toggle(new Rect(position.width / x, position.height / y, 200, 200), ButtonCheck(x,y));
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }

    void SelectColumn(int columnNumber)
    {
        EditorGUILayout.BeginVertical();
        for (int y = 0; y < _height; y++)
        {
            grid[columnNumber, y] = !grid[columnNumber, y];
        }
        EditorGUILayout.EndVertical();
    }

    void SelectRow(int rowNumber)
    {
        EditorGUILayout.BeginHorizontal();
        for (int x = 0; x < _width; x++)
        {
            grid[x, rowNumber] = !grid[x, rowNumber];
        }
        EditorGUILayout.EndHorizontal();
    }

    bool ButtonCheck(int x, int y)
    {
        Texture2D buttonTexture;
        if (grid[x, y]) buttonTexture = buttonTextureA;
        else buttonTexture = buttonTextureB;

        if (GUILayout.Button(buttonTexture, GUILayout.Height(25), GUILayout.Width(25)))
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
