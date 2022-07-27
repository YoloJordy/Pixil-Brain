using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class Minesweeper : Game
{
    [SerializeField] int width = 16;
    [SerializeField] int height = 16;
    public static Minesweeper current;

    [SerializeField] int totalBombs = 40;
    [SerializeField] TMP_Text bombText;
    [SerializeField] float totalExplodeTime = 2;
    [SerializeField] float floodTimeBeforeYield = 50;

    bool floodingTiles = false;

    int bombs;
    int unRevealedCells;
    int Bombs
    {
        get { return bombs; }
        set
        {
            bombs = value;
            if (bombText != null) bombText.text = bombs.ToString();
        }
    }

    static bool isFlagInput = false;

    Dictionary<Vector3Int, Cell> cells = new();
    Dictionary<Vector3Int, Cell> startingCells = new();

    public static void SwitchToInput(bool newIsFlagInput)
    {
        isFlagInput = newIsFlagInput;
    }

    public void SetGameValues(string width, string height, string bombAmount)
    {
        int.TryParse(width, out current.width);
        int.TryParse(height, out current.height);
        int.TryParse(bombAmount, out current.totalBombs);
    }

    private void Awake()
    {
        if (current == null) current = this;
        board = GetComponentInChildren<Board>();
    }

    private void Start()
    {
        InputHandler.current.Tapped += InputTapped;
        InputHandler.current.Held += InputHeld;
        MinesweeperUI.current.ClickedStart += NewGame;
    }

    protected override void NewGame()
    {
        Bombs = totalBombs;
        GenerateCells();
        board.DrawBoard(cells, new Vector2Int(width, height));
        size = new Vector2(width * board.tilemap.cellSize.x, height * board.tilemap.cellSize.y);
        InvokeBeginGame();

        Invoke(nameof(SetStateStart), 0.1f);
    }
    void SetStateStart() 
    { 
        state = State.START;
        InputHandler.current.TakingInput = true;
    }

    //generates new board
    void StartGame(Vector3Int startCellPosition)
    {
        GenerateCells();
        GenerateBombs(startCellPosition);
        GenerateNumbers();

        board.DrawBoard(cells, new Vector2Int(width, height));

        state = State.PLAYING;
    }

    //initializes cells
    void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new()
                {
                    position = new Vector3Int(x, y, 0),
                    type = Cell.Type.NUMBER,
                };

                if (cells.ContainsKey(cell.position)) cells[cell.position] = cell;
                else cells.Add(cell.position, cell);
            }
        }
        unRevealedCells = (width * height) - Bombs;
    }

    //set some cells to bombs
    void GenerateBombs(Vector3Int startCellPosition)
    {
        startingCells = AdjacentCells(startCellPosition);
        startingCells.Add(startCellPosition, cells[startCellPosition]);
        if (totalBombs >= cells.Count - startingCells.Count || (width == 1 && height == 1))
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var key = new Vector3Int(x, y);
                    var cell = cells[key];
                    SetCell(BombCell, cell.position);
                }
            }
            foreach (var pair in startingCells) { SetCell(NumberCell, pair.Value.position); }

            if (width == 1 && height == 1)
            {
                Bombs = 0;
                unRevealedCells = 1;
            }
            else
            {
                Bombs = cells.Count - startingCells.Count;
                unRevealedCells = startingCells.Count;
            }
            return;
        }

        for (int i = 0; i < totalBombs; i++)
        {
            SetRandomBombCell(startCellPosition);
        }
        Bombs = totalBombs;
    }

    //sets a random empty cell to a bomb
    void SetRandomBombCell(Vector3Int startCellPosition)
    {
        var cell = cells[new Vector3Int(Random.Range(0, width), Random.Range(0, height))];

        if (cell.position == startCellPosition) { SetRandomBombCell(startCellPosition); return; }

        foreach (var pair in startingCells) if (cell.position == pair.Value.position) { SetRandomBombCell(startCellPosition); return; }

        if (cell.type == Cell.Type.NUMBER && cell.position != startCellPosition) SetCell(BombCell, cell.position);
        else SetRandomBombCell(startCellPosition);
    }

    void GenerateNumbers()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var key = new Vector3Int(x, y);
                if (!cells.ContainsKey(key)) continue;
                var cell = cells[key];

                if (cell.type == Cell.Type.BOMB) continue;

                var newCell = cell;
                newCell.number = CountCellNumber(cell);
                cells[cell.position] = newCell;
            }
        }
    }

    int CountCellNumber(Cell cell)
    {
        int count = 0;

        foreach (var pair in AdjacentCells(cell.position))
        {
            if (pair.Value.type == Cell.Type.BOMB) count++;
        }
        return count;
    }

    Dictionary<Vector3Int, Cell> AdjacentCells(Vector3Int cellPosition)
    {
        Dictionary<Vector3Int, Cell> adjacentCells = new();
        for (int ix = -1; ix <= 1; ix++)
        {
            for (int iy = -1; iy <= 1; iy++)
            {
                if (ix == 0 && iy == 0) continue;

                Vector3Int adjacent = new(cellPosition.x + ix, cellPosition.y + iy);
                if (!IsValidCell(adjacent)) continue;

                adjacentCells.Add(adjacent, cells[adjacent]);
            }
        }
        return adjacentCells;
    }

    protected override void InputTapped(Vector3 position)
    {
        if (state == State.GAMEOVER || state == State.WIN || floodingTiles) return;
        var cellPosition = GetClickedCellPosition(position);
        if (!IsValidCell(cellPosition)) return;

        if (!isFlagInput)
        {
            var cell = cells[cellPosition];

            if (state == State.START) StartGame(cellPosition);
            if (cell.revealed) StartCoroutine(ClickRevealedCell(cellPosition));

            SetCell(RevealCell, cellPosition);
        }
        else SetCell(FlagCell, GetClickedCellPosition(position));
    }

    protected override void InputHeld(Vector3 position)
    {
        if (state == State.GAMEOVER || state == State.WIN || floodingTiles) return;
        var cellPosition = GetClickedCellPosition(position);
        if (!IsValidCell(cellPosition) || cells[cellPosition].revealed) return;

        Handheld.Vibrate();
        SetCell(FlagCell, cellPosition);
    }

    //update the values of a cells
    void SetCell(Func<Cell, Cell> changeFunc, Vector3Int cellPosition)
    {
        if (!IsValidCell(cellPosition)) return;

        var cell = cells[cellPosition];

        cell = changeFunc(cell);

        board.tilemap.SetTile(cellPosition, board.GetTile(cell));
        cells[cellPosition] = cell;
        if (unRevealedCells == 0) CheckWin();
    }

    //when the cell you clicked has been revealed
    IEnumerator ClickRevealedCell(Vector3Int cellPosition)
    {
        var clickedCell = cells[cellPosition];
        var adjacentCells = AdjacentCells(cellPosition);
        var adjacentFlags = 0;
        var adjacentUnrevealedCells = 0;
        
        foreach (var pair in adjacentCells)
        {
            if (pair.Value.flagged) adjacentFlags++;
            if (!pair.Value.revealed) adjacentUnrevealedCells++;
        }

        if (clickedCell.number <= adjacentFlags)
        {
            foreach (var pair in adjacentCells) if (!pair.Value.flagged && !pair.Value.revealed) { SetCell(RevealCell, pair.Value.position); }
        }
        else if (clickedCell.number >= adjacentUnrevealedCells)
        {
            foreach (var pair in adjacentCells) if (!pair.Value.flagged && !pair.Value.revealed) { SetCell(FlagCell, pair.Value.position); }
        }
        yield return null;
    }

    //what value you want updated
    Cell RevealCell(Cell cell)
    {
        if (state == State.PLAYING)
        {
            if (cell.flagged) return cell;
            if (cell.type == Cell.Type.BOMB) Explode(cell.position);
        }

        //if tile is empty reveal all cells around
        if (cell.type != Cell.Type.BOMB && cell.number == 0 && !cell.revealed)
        {
            StartCoroutine(Flood(cell));
        }
        
        if(!cell.revealed) unRevealedCells--;
        cell.revealed = true;
        return cell;
    }
    IEnumerator Flood(Cell cell)
    {
        floodingTiles = true;
        Stopwatch stopwatch = new();
        Queue<Cell> queue = new();
        Dictionary<Vector3Int, Cell> floodedCells = new();
        queue.Enqueue(cell);
        floodedCells.Add(cell.position, cell);

        stopwatch.Start();
        while (queue.Count != 0)
        {
            var queuedCell = queue.Dequeue();
            SetCell(RevealCellFlood, queuedCell.position);

            if (queuedCell.number > 0) continue;

            for (int ix = -1; ix <= 1; ix++)
            {
                for (int iy = -1; iy <= 1; iy++)
                {
                    if (ix == 0 && iy == 0) continue;

                    Vector3Int adjacent = new(queuedCell.position.x + ix, queuedCell.position.y + iy);
                    if (floodedCells.ContainsKey(adjacent)) continue;
                    if (!IsValidCell(adjacent)) continue;
                    queue.Enqueue(cells[adjacent]);
                    floodedCells.Add(adjacent, cells[adjacent]);
                    if (stopwatch.ElapsedMilliseconds % floodTimeBeforeYield == 0 && stopwatch.ElapsedMilliseconds != 0)
                    {
                        Debug.Log("Yielding");
                        yield return null;
                    }
                }
            }
        }
        stopwatch.Stop();
        Debug.Log("Elapsed Time: " + stopwatch.ElapsedMilliseconds);
        floodedCells.Clear();
        floodingTiles = false;
    }
    Cell RevealCellFlood(Cell cell)
    {
        unRevealedCells--;
        cell.revealed = true;

        return cell;
    }
    Cell FlagCell(Cell cell)
    {
        if (cell.flagged) Bombs++; else Bombs--;
        cell.flagged = !cell.flagged;

        return cell;
    }
    Cell BombCell(Cell cell)
    {
        cell.type = Cell.Type.BOMB;
        return cell;
    }
    Cell NumberCell(Cell cell)
    {
        cell.type = Cell.Type.NUMBER;
        return cell;
    }

    Vector3Int GetClickedCellPosition(Vector3 position) => board.tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(position));

    bool IsValidCell(Vector3Int cellPosition) => cellPosition.x >= 0 && cellPosition.x < width && cellPosition.y >= 0 && cellPosition.y < height; 

    void CheckWin()
    {
        unRevealedCells = 0;
        foreach (var pair in cells) if (!pair.Value.revealed && pair.Value.type == Cell.Type.NUMBER) unRevealedCells++;

        if (unRevealedCells == 0) Invoke(nameof(Win), 0.3f);
    }

    void Win()
    {
        state = State.WIN;
        Camera.main.transform.position = new Vector3(width / 2, height / 2, Camera.main.transform.position.z);
        InputHandler.current.TakingInput = false;

        InvokeEndGame(true);
    }

    void Explode(Vector3Int cellPosition)
    {
        state = State.GAMEOVER;
        SetCell(RevealCell, cellPosition);
        Camera.main.transform.position = new Vector3(width / 2, height / 2, Camera.main.transform.position.z);
        InputHandler.current.TakingInput = false;

        StartCoroutine(ExplodeAll());
    }

    IEnumerator ExplodeAll()
    {
        yield return new WaitForSeconds(0.5f);

        var waitTime = totalExplodeTime / totalBombs;

        Stopwatch stopwatch = new();
        stopwatch.Start();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var key = new Vector3Int(x, y);
                if (!cells.ContainsKey(key)) continue;
                var cell = cells[key];

                if (cell.type == Cell.Type.BOMB)
                {
                    SetCell(RevealCell, cell.position);
                    if (waitTime > stopwatch.ElapsedMilliseconds / 1000) yield return new WaitForSeconds(waitTime - (stopwatch.ElapsedMilliseconds / 1000));
                    else continue;
                    stopwatch.Restart();
                }
            }
        }
        stopwatch.Stop();
        InvokeEndGame(false);
    }
}
