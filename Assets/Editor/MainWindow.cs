using UnityEditor;
using UnityEngine;

public class MainWindow : EditorWindow
{
    public GameObject propList;
    public Texture2D tex;
    public Texture2D tex2;

    private int _height = 10;
    private int _width = 10;
    private bool[,] grid = new bool[10, 10];
    private int _propCount;

    GUIStyle style = new GUIStyle(EditorStyles.label);


    [MenuItem("CustomTools/MapGenerator")]
    public static void OpenWindow()
    {
        MainWindow window = GetWindow<MainWindow>();

        window.wantsMouseMove = true;

        window.minSize = new Vector2(450, 360);
    }

    private void OnGUI()
    {
        style.normal.textColor = Color.white;

        GUI.DrawTexture(new Rect(0, 0,position.width, position.height), tex, ScaleMode.StretchToFill);
        GUI.DrawTexture(new Rect(0, 0, position.width, 170 + _height * 19), tex2, ScaleMode.StretchToFill);
        //EditorGUI.DrawRect(GUILayoutUtility.GetRect(1, 250), new Color(0.2f,0.2f,0.2f,0.5f));

        //360

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        DrawGridConfig();
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        DrawPropConfig();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Generate", GUILayout.Height(40)))
            Generate();
    }

    private void DrawPropConfig()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Prop Count", style);
        _propCount = EditorGUILayout.IntField( _propCount);

        EditorGUILayout.EndHorizontal();

        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty stringsProperty = so.FindProperty("propList");

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Prop List", style);
        EditorGUILayout.ObjectField(stringsProperty, new GUIContent(""));

        EditorGUILayout.EndHorizontal();
        /*
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty stringsProperty = so.FindProperty("propList");
 
        EditorGUILayout.PropertyField(stringsProperty, true);
        so.ApplyModifiedProperties();
        */
    }
    
    private void DrawGridConfig()
    {
        bool error = false; 


        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Matrix Height", style); 
        _height = EditorGUI.IntSlider(new Rect(100, 19, position.width -110, 15), _height, 1, 10);

        EditorGUILayout.EndHorizontal();

        if (_height < 1 || _height > 10)
        {
            GUILayout.Label("Height must be between 1 and 10!", style);
            error = true;
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Matrix Width", style);
        _width = EditorGUI.IntSlider(new Rect(100, 44, position.width - 110, 15), _width, 1, 10);

        EditorGUILayout.EndHorizontal();

        if (_width < 1 || _width > 10)
        {
            GUILayout.Label("Width must be between 1 and 10!", style);
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
        for (int j = 0; j < _width; j++)
        {
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < _height; i++)
            {
                grid[j, i] = EditorGUILayout.Toggle(grid[j, i]);
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void Generate()
    {
        
    }
}
