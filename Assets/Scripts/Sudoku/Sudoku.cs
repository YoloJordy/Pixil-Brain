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
    readonly int[] presetGame = {0, 0, 0, 9, 5, 6, 8, 0, 0, 0, 5, 6, 8, 0, 0, 3, 0, 0, 0, 2, 7, 0, 0, 4, 9, 0, 0, 1, 3, 9, 0, 0, 0, 2, 0, 8, 4, 6, 0, 2, 7, 0, 1, 3, 0, 0, 0, 8, 1, 0, 9, 0, 0, 5, 5, 4, 0, 0, 9, 1, 7, 0, 0, 0, 9, 1, 0, 0, 2, 5, 0, 0, 7, 8, 2, 0, 4, 3, 0, 9, 1};
    readonly int[] presetGameSolution = {3, 1, 4, 9, 5, 6, 8, 2, 7, 9, 5, 6, 8, 2, 7, 3, 1, 4, 8, 2, 7, 3, 1, 4, 9, 5, 6, 1, 3, 9, 4, 6, 5, 2, 7, 8, 4, 6, 5, 2, 7, 8, 1, 3, 9, 2, 7, 8, 1, 3, 9, 4, 6, 5, 5, 4, 3, 6, 9, 1, 7, 8, 2, 6, 9, 1, 7, 8, 2, 5, 4, 3, 7, 8, 2, 5, 4, 3, 6, 9, 1};

    private void Awake()
    {
        if (current == null) current = this;
        board = GetComponentInChildren<SudokuBoard>();
    }

    protected override void Start()
    {
        cells = new();
        base.Start();
        NewGame();
    }

    protected override void NewGame()
    {
        cells = board.DrawBoard(new Vector2Int(width, height));
        LoadBoard();
        size = new Vector2(width * board.cellSize.x, height * board.cellSize.y);

        InvokeBeginGame();
        Invoke(nameof(SetStatePlaying), 0.1f);
    }

    void LoadBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                cells[new Vector3Int(i, j)].SetNumber(presetGame[(i * width) + j]);
            }
        }
    }

    protected override void LoadGame()
    {
        throw new System.NotImplementedException();
    }

    protected override void InputTapped(Vector3 position)
    {
        if (state == State.GAMEOVER || state == State.WIN) return;
        var cellPosition = GetClickedCellPosition(position);
        if (!IsValidCell(cellPosition)) return;

        var cell = cells[cellPosition];
        if (!cell.data.selected) SelectCell(cellPosition);
    }

    protected override void InputHeld(Vector3 position)
    {
        throw new System.NotImplementedException();
    }

    Vector3Int GetClickedCellPosition(Vector3 position) => board.tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(position));

    bool IsValidCell(Vector3Int cellPosition) => cellPosition.x >= 0 && cellPosition.x < width && cellPosition.y >= 0 && cellPosition.y < height;

    void SelectCell(Vector3Int cellPosition) 
    {
        var cell = cells[cellPosition];
        cell.Select();

        cells[cellPosition] = cell;

        if (cell.data.number > 0) SelectNumbers(cell.data.number);
    }

    void SelectNumbers(int number)
    {
        
    }
}
