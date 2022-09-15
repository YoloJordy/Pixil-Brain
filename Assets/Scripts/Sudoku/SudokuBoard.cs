using System.Collections.Generic;
using UnityEngine;

public class SudokuBoard : BaseBoard
{
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject linePrefab;
    [SerializeField] Canvas canvas;
    public Vector3 cellSize = new (100, 100);
    public Dictionary<Vector3Int, SudokuCell> DrawBoard(Vector2Int size)
    {
        Dictionary<Vector3Int, SudokuCell> cells = new();
        int width = size.x;
        int height = size.y;

        Instantiate(linePrefab, new Vector3(tilemap.cellSize.x * 3, height * tilemap.cellSize.y / 2), Quaternion.identity, canvas.transform).transform.localScale = new Vector3(12, height * cellSize.y);
        Instantiate(linePrefab, new Vector3(tilemap.cellSize.x * 6, height * tilemap.cellSize.y / 2), Quaternion.identity, canvas.transform).transform.localScale = new Vector3(12, height * cellSize.y);
        Instantiate(linePrefab, new Vector3(width * tilemap.cellSize.x / 2, tilemap.cellSize.y * 3), Quaternion.identity, canvas.transform).transform.localScale = new Vector3(width * cellSize.x, 12);
        Instantiate(linePrefab, new Vector3(width * tilemap.cellSize.x / 2, tilemap.cellSize.y * 6), Quaternion.identity, canvas.transform).transform.localScale = new Vector3(width * cellSize.x, 12);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var position = new Vector3Int(x, y);
                cells.Add(position, Instantiate(tilePrefab, tilemap.GetCellCenterWorld(position), Quaternion.identity, canvas.transform).GetComponent<SudokuCell>());
            }
        }
        return cells;
    }
}
