using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell
{
    public enum Type
    {
        NUMBER,
        BOMB,
    }

    public Type type;

    public bool revealed;
    public bool flagged;

    public int number;

    public Vector3Int position;
}
