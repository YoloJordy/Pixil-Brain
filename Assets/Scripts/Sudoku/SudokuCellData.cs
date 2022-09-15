using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SudokuCellData : BaseCell
{
    public bool selected = false;
    public HashSet<int> annotations = new();
}
