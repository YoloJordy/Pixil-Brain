using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SaveData
{
    public Cell[] cells;
    public SaveData(Game game)
    {
        List<Cell> cellList = new();
        foreach(var pair in game.cells)
        {
            cellList.Add(pair.Value);
        }
        cells = cellList.ToArray();
    }
}
