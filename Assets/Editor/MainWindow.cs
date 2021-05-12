using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MainWindow : EditorWindow
{
    public GameObject propList;
    
    private int _height = 1;
    private int _width = 1;
    private int _propCount;
    
    [MenuItem("CustomTools/MapGenerator")]
    public static void OpenWindow()
    {
        MainWindow window = GetWindow<MainWindow>();

        window.wantsMouseMove = true;
    }

    private void OnGUI()
    {
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
        _propCount = EditorGUILayout.IntField("Prop Count", _propCount);
        
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty stringsProperty = so.FindProperty("propList");
 
        EditorGUILayout.PropertyField(stringsProperty, true);
        so.ApplyModifiedProperties();
    }
    
    private void DrawGridConfig()
    {
        bool error = false; 
        GUIStyle style = new GUIStyle(EditorStyles.label);
        style.normal.textColor = Color.red;

        _height = EditorGUILayout.IntField("Matrix Height", _height);
        if (_height < 1 || _height > 10)
        {
            GUILayout.Label("Height must be between 1 and 10!", style);
            error = true;
        }
        
        EditorGUILayout.Space();
        
        _width = EditorGUILayout.IntField("Matrix Width", _width);
        if (_width < 1 || _width > 10)
        {
            GUILayout.Label("Width must be between 1 and 10!", style);
            error = true;
        }

        if (!error)
        {
            if (GUILayout.Button("Draw Grid", GUILayout.Height(30)))
                DrawGrid();
        }
    }

    private void DrawGrid()
    {
        
    }

    private void Generate()
    {
        
    }
}
