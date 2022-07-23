using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Minesweeper : Game
{
    [SerializeField] int width = 16;
    [SerializeField] int height = 16;
    public static Minesweeper current;

    [SerializeField] int totalBombs = 40;
    [SerializeField] TMP_Text bombText;
    [SerializeField] float totalExplodeTime = 2;

    int iterations = 0;

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

    const int startCellCount = 9;

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

        InputHandler.current.TakingInput = true;
        Camera.main.transform.position = new Vector3(width / 2, height / 2, Camera.main.transform.position.z);

        Invoke(nameof(SetStateStart), 0.1f);
    }
    void SetStateStart() { state = State.START; }

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
            for(int y = 0; y < height; y++)
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
        unRevealedCells = (width * height) - totalBombs;
    }

    //set some cells to bombs
    void GenerateBombs(Vector3Int startCellPosition)
    {
        startingCells = AdjacentCells(startCellPosition);
        if (totalBombs >= cells.Count - startCellCount) 
        {
            var bombAmount = 0;
            foreach (var pair in cells) { SetCell(BombCell, pair.Value.position); bombAmount++; }
            foreach (var pair in AdjacentCells(startCellPosition)) { SetCell(NumberCell, pair.Value.position); bombAmount--; }
            SetCell(NumberCell, startCellPosition);
            bombAmount--;
            Bombs = bombAmount;
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
        var update = cells;
        foreach(var pair in cells)
        {
            var cell = pair.Value;
            if (cell.type == Cell.Type.BOMB) continue;

            var newCell = cell;
            newCell.number = CountCellNumber(cell);
            update[cell.position] = newCell;
        }
        cells = update;
    }

    int CountCellNumber(Cell cell)
    {
        int count = 0;

        foreach(var pair in AdjacentCells(cell.position))
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
        if (state == State.GAMEOVER || state == State.WIN) return;
        var cellPosition = GetClickedCellPosition(position);
        if (!IsValidCell(cellPosition)) return;

        if (!isFlagInput)
        {
            var cell = cells[cellPosition];

            if (state == State.START) StartGame(cellPosition);
            if (cell.revealed) ClickRevealedCell(cellPosition);

            SetCell(RevealCell, cellPosition);
        }
        else SetCell(FlagCell, GetClickedCellPosition(position));
    }
    
    protected override void InputHeld(Vector3 position)
    {
        if (state == State.GAMEOVER || state == State.WIN) return;
        var cellPosition = GetClickedCellPosition(position);
        if (!IsValidCell(cellPosition)) return;

        Handheld.Vibrate();
        SetCell(FlagCell, cellPosition);
    }

    //update the values of a cells
    void SetCell(Func<Cell, Cell> changeCell, Vector3Int cellPosition)
    {
        if (!IsValidCell(cellPosition)) return;

        var cell = cells[cellPosition];

        cell = changeCell(cell);

        board.tilemap.SetTile(cellPosition, board.GetTile(cell));
        cells[cellPosition] = cell;
    }

    //when the cell you clicked has been revealed
    void ClickRevealedCell(Vector3Int cellPosition)
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
            foreach (var pair in adjacentCells) if (!pair.Value.flagged && !pair.Value.revealed) SetCell(RevealCell, pair.Value.position);
        }
        else if (clickedCell.number >= adjacentUnrevealedCells)
        {
            foreach (var pair in adjacentCells) if (!pair.Value.flagged && !pair.Value.revealed) SetCell(FlagCell, pair.Value.position);
        }
    }

    //what value you want updated
    Cell RevealCell(Cell cell)
    {
        if (state == State.PLAYING)
        {
            if (cell.flagged) return cell;
            if (cell.type == Cell.Type.BOMB) Explode(cell.position);
        }

        if (!cell.revealed) unRevealedCells--;
        cell.revealed = true;
        cells[cell.position] = cell;

        //if tile is empty reveal all cells around
        if (cell.type != Cell.Type.BOMB && cell.number == 0)
        {
            foreach (var pair in AdjacentCells(cell.position))
            {
                if (!pair.Value.revealed) SetCell(RevealCell, pair.Value.position);
            }
        }
        if (unRevealedCells == 0) CheckWin();

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

    Vector3Int GetClickedCellPosition(Vector3 position)
    {
        return board.tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(position));
    }

    bool IsValidCell(Vector3Int cellPosition)
    {
        return cellPosition.x >= 0 && cellPosition.x < width && cellPosition.y >= 0 && cellPosition.y < height; 
    }

    void CheckWin()
    {
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

        StartCoroutine(nameof(ExplodeAll));
    }

    IEnumerator ExplodeAll()
    {
        yield return new WaitForSeconds(0.5f);

        var waitTime = totalExplodeTime / totalBombs;

        foreach (var pair in cells)
        {
            if (pair.Value.type == Cell.Type.BOMB)
            {
                SetCell(RevealCell, pair.Value.position);
                yield return new WaitForSeconds(waitTime - Time.deltaTime);
            }
        }

        InvokeEndGame(false);
    }
}
