using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MinesweeperSaveData : SaveData
{
    public Cell[] cells;
    public int currentBombs;
    public int totalBombs;
    public int unRevealedCells;
    public int width;
    public int height; 
    public MinesweeperSaveData(Game game) : base(game)
    {
        var minesweeper = (Minesweeper)game;
        List<Cell> cellList = new();
        foreach(var pair in game.cells)
        {
            cellList.Add(pair.Value);
        }
        cells = cellList.ToArray();

        currentBombs = minesweeper.Bombs;
        totalBombs = minesweeper.totalBombs;
        unRevealedCells = minesweeper.UnRevealedCells;
        width = minesweeper.width;
        height = minesweeper.height;
    }
}
