using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    [System.NonSerialized] public Tilemap tilemap;

    [SerializeField] string resourcesPath;
    [SerializeField] string[] fileNames = { "1", "2", "3", "4", "5", "6", "7", "8", "empty", "flag", "hidden", "bomb" };
    Dictionary<string, Tile> tiles = new();

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();

        GetResources();
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
        else if (cell.flagged) return tiles["flag"];
        else return tiles["hidden"];
    }

    Tile GetRevealedTile(Cell cell)
    {
        switch (cell.type)
        {
            case Cell.Type.BOMB: return tiles["bomb"];
            case Cell.Type.NUMBER: return GetNumberTile(cell);
            default: return null;
        }
    }

    Tile GetNumberTile(Cell cell)
    {
        switch (cell.number)
        {
            default: return tiles["empty"];
            case 1: return tiles["1"];
            case 2: return tiles["2"];
            case 3: return tiles["3"];
            case 4: return tiles["4"];
            case 5: return tiles["5"];
            case 6: return tiles["6"];
            case 7: return tiles["7"];
            case 8: return tiles["8"];
        }
    }

    void GetResources()
    {
        foreach(string filename in fileNames)
        {
            tiles.Add(filename, Resources.Load<Tile>(resourcesPath + "/" + filename));
            Debug.Log(tiles[filename].sprite);
        }
    }
}
