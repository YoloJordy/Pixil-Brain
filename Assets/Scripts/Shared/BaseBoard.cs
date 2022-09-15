using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class BaseBoard : MonoBehaviour
{
    [System.NonSerialized] public Tilemap tilemap;

    [SerializeField] protected string resourcesPath;
    [SerializeField] protected string[] fileNames = { };
    protected Dictionary<string, Tile> tiles = new();

    protected void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();

        GetResources();
    }

    protected void GetResources()
    {
        foreach (string filename in fileNames)
        {
            tiles.Add(filename, Resources.Load<Tile>(resourcesPath + "/" + filename));
        }
    }
}
