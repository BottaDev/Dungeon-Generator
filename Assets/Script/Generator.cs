﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class Generator
{
    private bool[,] _grid;
    private int _width;
    private int _height;
    private float _roomSeparation;
    private List<GameObject> _nodeList = new List<GameObject>();

    private GameObject _room1;
    private GameObject _room2A;
    private GameObject _room2B;
    private GameObject _room3;
    private GameObject _room4;

    public void SetParameters(bool[,] grid, int width, int height, float roomSeparation, GameObject room1, GameObject room2A, GameObject room2B, GameObject room3, GameObject room4)
    {
        _grid = grid;
        _width = width;
        _height = height;
        _roomSeparation = roomSeparation;
        _room1 = room1;
        _room2A = room2A;
        _room2B = room2B;
        _room3 = room3;
        _room4 = room4;
    }

    public void GenerateDungeon()
    {
        CreateNodes();
    }

    private void CreateNodes()
    {
        Vector3 lastPos = Vector3.zero;
        
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_grid[x, y])
                {
                    GameObject node = SetRoom(x, y);
                    node.name = "Node [" + x + " | " + y + "]";

                    GameObject.Instantiate(node);
                    
                    if (x == 0 && y == 0)
                        node.transform.position = Vector3.zero;
                    else
                        node.transform.position = lastPos;
                    
                    _nodeList.Add(node);
                }

                lastPos += new Vector3(_roomSeparation, 0, 0);
            }
            
            lastPos += new Vector3(-lastPos.x, 0, _roomSeparation);
        }
    }

    private GameObject SetRoom(int x, int y)
    {
        bool leftSide;
        bool rightSide;
        bool topSide;
        bool bottonSide;

        GameObject roomPrefab = null;

        // REVISAR CASO CON 1 FILA O 1 COLUMNA

        if (x == 0 || x == _grid.GetLength(0) - 1 || y == 0 || y == _grid.GetLength(1) - 1)
            CheckBoundaryNode(x, y, out leftSide, out rightSide, out topSide, out bottonSide);
        else
            CheckCenterNode(x, y, out leftSide, out rightSide, out topSide, out bottonSide);

        int sidesCount = new bool[4] { leftSide, rightSide, topSide, bottonSide }.Where(c => c).Count();

        if (sidesCount == 0 || sidesCount == 1)
        {
            roomPrefab = _room1;   
        }
        else if (sidesCount == 2)
        {
            if ((bottonSide && topSide) || (leftSide && rightSide))
                roomPrefab = _room2A;
            else
                roomPrefab = _room2B;
        }
        else if (sidesCount == 3)
        {
            roomPrefab = _room3;
        }
        else if (sidesCount == 4)
        {
            roomPrefab = _room4;
        }
        
        return roomPrefab;
    }

    private void CheckBoundaryNode(int x, int y, out bool leftSide, out bool rightSide, out bool topSide, out bool bottonSide)
    {
        leftSide = true;
        rightSide = true;
        topSide = true;
        bottonSide = true;

        if (x == 0)
        {
            topSide = false; 
            
            if (!_grid[x + 1, y]) // Botton Node
                bottonSide = false;
        }
        else if (x == _grid.GetLength(0) - 1)
        {
            bottonSide = false;
            
            if (!_grid[x - 1, y]) // Top Node
                topSide = false;
        }
        else
        {
            if (!_grid[x - 1, y]) // Top Node
                topSide = false;
            
            if (!_grid[x + 1, y]) // Botton Node
                bottonSide = false;
        }

        if (y == 0)
        {
            leftSide = false;
            
            if (!_grid[x, y + 1]) // Right Node
                rightSide = false;
        }
        else if (y == _grid.GetLength(1) - 1)
        {
            rightSide = false;
            
            if (!_grid[x, y - 1]) // Left Node
                leftSide = false;
        }
        else
        {
            if (!_grid[x, y - 1]) // Left Node
                leftSide = false;
            
            if (!_grid[x, y + 1]) // Right Node
                rightSide = false;
        }
    }

    private void CheckCenterNode(int x, int y, out bool leftSide, out bool rightSide, out bool topSide, out bool bottonSide)
    {
        leftSide = true;
        rightSide = true;
        topSide = true;
        bottonSide = true;
        
        if (!_grid[x - 1, y]) // Top Node
            topSide = false;
            
        if (!_grid[x + 1, y]) // Botton Node
            bottonSide = false;
        
        if (!_grid[x, y - 1]) // Left Node
            leftSide = false;
            
        if (!_grid[x, y + 1]) // Right Node
            rightSide = false;
    }
    
    public List<GameObject> GetNodes()
    {
        if (_nodeList.Count == 0)
            _nodeList = GameObject.FindObjectsOfType<GameObject>().Where(x => x.name.Contains("Node")).ToList();
        
        return _nodeList;
    }
}
