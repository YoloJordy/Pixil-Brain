using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sudoku : Game
{
    const int width = 9;
    const int height = 9;

    public static Sudoku current;

    public SudokuBoard board;

    Dictionary<Vector3Int, SudokuCell> cells;

    private void Awake()
    {
        if (current == null) current = this;
        board = GetComponentInChildren<SudokuBoard>();
    }

    protected override void Start()
    {
        cells = new();
        base.Start();
    }

    protected override void NewGame()
    {
        throw new System.NotImplementedException();
    }

    protected override void InputTapped(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    protected override void InputHeld(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    protected override void LoadGame()
    {
        throw new System.NotImplementedException();
    }
}
