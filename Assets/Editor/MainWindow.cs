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
    
    private static Generator _generator;

    private readonly GUIStyle _style = new GUIStyle(EditorStyles.label);
    private readonly GUIStyle _errorStyle = new GUIStyle(EditorStyles.label);

    [MenuItem("CustomTools/MapGenerator")]
    public static void OpenWindow()
    {
        MainWindow window = GetWindow<MainWindow>();

        window.wantsMouseMove = true;

        window.minSize = new Vector2(450, 360);
        
        _generator = new Generator();
    }

    private void OnGUI()
    {
        _style.normal.textColor = Color.white;
        _errorStyle.normal.textColor = Color.red;
        
        if (tex != null)
            GUI.DrawTexture(new Rect(0, 0,position.width, position.height), tex, ScaleMode.StretchToFill);
        
        if (tex2 != null)
        GUI.DrawTexture(new Rect(0, 0, position.width, 170 + _height * 19), tex2, ScaleMode.StretchToFill);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("invert"))
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    grid[x, y] = !grid[x, y];
                }
            }

        }


        DrawGridConfig();
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        DrawPropConfig();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Generate", GUILayout.Height(40)))
            Generate();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Delete Map", GUILayout.Height(40)))
            DeleteMap();
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

        EditorGUILayout.LabelField("Matrix Height", _style); 
        _height = EditorGUI.IntSlider(new Rect(100, 19, position.width -110, 15), _height, 1, 10);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Matrix Width", _style);
        _width = EditorGUI.IntSlider(new Rect(100, 44, position.width - 110, 15), _width, 1, 10);
        
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        
        _roomSeparation = EditorGUILayout.FloatField("Room Separation", _roomSeparation, _style);
        
        EditorGUILayout.EndHorizontal();

        if (_roomSeparation < 1)
        {
            GUILayout.Label("The room separation must be greater than 0!", _errorStyle);
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
                //grid[x, y] = EditorGUILayout.Toggle(grid[x, y]);
                grid[x, y] = ButtonCheck(x,y);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }

    bool ButtonCheck(int x, int y)
    {
        Texture2D buttonTexture;
        if (grid[x, y]) buttonTexture = buttonTextureA;
        else buttonTexture = buttonTextureB;

        if (GUILayout.Button(buttonTexture, GUILayout.Height(30), GUILayout.Width(30)))
        {
            return !grid[x, y];
        }

        else return grid[x, y];
    }

    private void Generate()
    {
        DeleteMap(false);
        
        _generator.SetParameters(grid, _width,_height, _roomSeparation);
        _generator.GenerateDungeon();

        _nodeList = _generator.GetNodes();
        
        Debug.Log("Map generated successfully.");
    }

    private void DeleteMap(bool showMessage = true)
    {
        if (_nodeList.Count == 0 && showMessage)
        {
            Debug.LogWarning("There is no map to delete!");
            return;
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
