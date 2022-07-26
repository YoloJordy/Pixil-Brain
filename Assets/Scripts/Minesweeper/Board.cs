using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    [System.NonSerialized] public Tilemap tilemap;

    [SerializeField] Tile tile1;
    [SerializeField] Tile tile2;
    [SerializeField] Tile tile3;
    [SerializeField] Tile tile4;
    [SerializeField] Tile tile5;
    [SerializeField] Tile tile6;
    [SerializeField] Tile tile7;
    [SerializeField] Tile tile8;
    [SerializeField] Tile tileFlagged;
    [SerializeField] Tile tileBomb;
    [SerializeField] Tile tileEmpty;
    [SerializeField] Tile tileHidden;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    public void DrawBoard(Dictionary<Vector3Int, Cell> cells, Vector2Int size)
    {
        tilemap.ClearAllTiles();
        int width = size.x;
        int height = size.y;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = cells[new Vector3Int(x, y)];
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }

    public Tile GetTile(Cell cell)
    {
        if (cell.revealed) return GetRevealedTile(cell);
        else if (cell.flagged) return tileFlagged;
        else return tileHidden;
    }

    Tile GetRevealedTile(Cell cell)
    {
        switch (cell.type)
        {
            case Cell.Type.BOMB: return tileBomb;
            case Cell.Type.NUMBER: return GetNumberTile(cell);
            default: return null;
        }
    }

    Tile GetNumberTile(Cell cell)
    {
        switch (cell.number)
        {
            default: return tileEmpty;
            case 1: return tile1;
            case 2: return tile2;
            case 3: return tile3;
            case 4: return tile4;
            case 5: return tile5;
            case 6: return tile6;
            case 7: return tile7;
            case 8: return tile8;
        }
    }
}
