using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Generator
{
    private bool[,] _grid;
    private int _width;
    private int _height;
    private float _roomSeparation;
    private List<GameObject> _nodeList = new List<GameObject>();
    
    public void SetParameters(bool[,] grid, int width, int height, float roomSeparation)
    {
        _grid = grid;
        _width = width;
        _height = height;
        _roomSeparation = roomSeparation;
    }

    public void GenerateDungeon()
    {
        CreateNodes();
    }

    private void CreateNodes()
    {
        //Vector3 spawnPos = SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0, 0f, 0));

        Vector3 lastPos = Vector3.zero;
        
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_grid[x, y] && x == 0 && y == 0)
                {
                    GameObject node = new GameObject("Node [" + x + " | " + y + "]");
                    node.transform.position = Vector3.zero;

                    _nodeList.Add(node);
                }
                else if (_grid[x, y])
                {
                    GameObject node = new GameObject("Node [" + x + " | " + y + "]");
                    node.transform.position = lastPos;

                    _nodeList.Add(node);
                }
                
                lastPos += new Vector3(_roomSeparation, 0, 0);
            }
            
            lastPos += new Vector3(-lastPos.x, 0, _roomSeparation);
        }
    }

    public List<GameObject> GetNodes()
    {
        return _nodeList;
    }
}
