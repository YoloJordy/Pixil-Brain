using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SudokuBoard : BaseBoard
{
    public void DrawBoard(Dictionary<Vector3Int, SudokuCell> cells, Vector2Int size)
    {
        tilemap.ClearAllTiles();
        int width = size.x;
        int height = size.y;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SudokuCell cell = cells[new Vector3Int(x, y)];
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }

    public Tile GetTile(SudokuCell cell)
    {
        if (cell.selected) return GetSelectedTile(cell);
        else return GetUnSelectedTile(cell);
    }

    Tile GetUnSelectedTile(SudokuCell cell)
    {
        return cell.number switch
        {
            1 => tiles["1"],
            2 => tiles["2"],
            3 => tiles["3"],
            4 => tiles["4"],
            5 => tiles["5"],
            6 => tiles["6"],
            7 => tiles["7"],
            8 => tiles["8"],
            9 => tiles["9"],
            _ => tiles["empty"],
        };
    }

    Tile GetSelectedTile(SudokuCell cell)
    {
        return cell.number switch
        {
            1 => tiles["1_selected"],
            2 => tiles["2_selected"],
            3 => tiles["3_selected"],
            4 => tiles["4_selected"],
            5 => tiles["5_selected"],
            6 => tiles["6_selected"],
            7 => tiles["7_selected"],
            8 => tiles["8_selected"],
            9 => tiles["9_selected"],
            _ => tiles["empty_selected"],
        };
    }
}
