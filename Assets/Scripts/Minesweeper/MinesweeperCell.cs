using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MinesweeperCell : BaseCell
{
    public enum Type
    {
        NUMBER,
        BOMB,
    }

    public Type type;

    public bool revealed;
    public bool flagged;
}
