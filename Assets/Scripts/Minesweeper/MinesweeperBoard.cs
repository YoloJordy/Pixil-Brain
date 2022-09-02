using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MinesweeperBoard : BaseBoard
{
    public void DrawBoard(Dictionary<Vector3Int, MinesweeperCell> cells, Vector2Int size)
    {
        tilemap.ClearAllTiles();
        int width = size.x;
        int height = size.y;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                MinesweeperCell cell = cells[new Vector3Int(x, y)];
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }

    public Tile GetTile(MinesweeperCell cell)
    {
        if (cell.revealed) return GetRevealedTile(cell);
        else if (cell.flagged) return tiles["flag"];
        else return tiles["hidden"];
    }

    Tile GetRevealedTile(MinesweeperCell cell)
    {
        return cell.type switch
        {
            MinesweeperCell.Type.BOMB => tiles["bomb"],
            MinesweeperCell.Type.NUMBER => GetNumberTile(cell),
            _ => null,
        };
    }

    Tile GetNumberTile(MinesweeperCell cell)
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
            _ => tiles["empty"],
        };
    }
}
