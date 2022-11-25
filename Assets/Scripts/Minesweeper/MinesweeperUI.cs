using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MinesweeperUI : BaseUI
{
    public static MinesweeperUI current;

    [SerializeField] MinesweeperCanvas canvas;

    [SerializeField] int minBoardSize = 4;
    [SerializeField] int maxBoardSize = 99;
    [SerializeField] int minBombs = 1;

    [SerializeField] int easyWidth = 10;
    [SerializeField] int easyHeight = 10;
    [SerializeField] int easyBombs = 10;
    
    [SerializeField] int mediumWidth = 16;
    [SerializeField] int mediumHeight = 16;
    [SerializeField] int mediumBombs = 40;

    [SerializeField] int hardWidth = 32;
    [SerializeField] int hardHeight = 16;
    [SerializeField] int hardBombs = 99;

    string FloatToTime(float raw)
    {
        int time = (int)raw;
        int sec = time % 60;
        int min = time / 60 % 60;
        int hour = time / 3600;
        return (hour > 0 ? hour + "h " : "") + ((min > 0 || hour > 0) ? min + "m " : "") + ((sec > 0 || min > 0 || hour > 0) ? sec + "s " : "");
    }

    private void Awake()
    {
        if (current == null) current = this;
    }
    private void Start()
    {
        Game.EndGame += ShowEndScreen;
    }

    private void Update()
    {
        if (canvas.bombCounter != null) canvas.bombCounter.text = Minesweeper.current.Bombs.ToString();
        canvas.timer.SetText(FloatToTime(Minesweeper.current.playTime));
    }
    void ShowEndScreen(bool won)
    {

        if (won) canvas.winScreen.SetActive(true);
        else canvas.gameOverScreen.SetActive(true);

        canvas.bombCounter.enabled = false;
        canvas.sideButtons.gameObject.SetActive(false);
    }

    public void ClickRestart()
    {
        canvas.winScreen.SetActive(false);
        canvas.gameOverScreen.SetActive(false);

        canvas.startScreen.SetActive(true);
        Minesweeper.current.board.tilemap.ClearAllTiles();
        Minesweeper.current.playTime = 1;
    }

    public event Action ClickedStart;
    public void ClickStart()
    {
        Minesweeper.current.SetGameValues(canvas.widthInput.text, canvas.heightInput.text, canvas.bombsInput.text);

        ShowGameUI();
        ClickedStart?.Invoke();
    }

    public void ShowGameUI()
    {
        canvas.startScreen.SetActive(false);
        canvas.winScreen.SetActive(false);
        canvas.gameOverScreen.SetActive(false);
        canvas.sideButtons.gameObject.SetActive(true);
        canvas.bombCounter.enabled = true;
        canvas.warningLabel.SetActive(false);
    }

    public void CheckInput()
    {
        int.TryParse(canvas.widthInput.text, out var width);
        int.TryParse(canvas.heightInput.text, out var height);
        int.TryParse(canvas.bombsInput.text, out var bombs);

        if (width < minBoardSize || width > maxBoardSize || height < minBoardSize || height > maxBoardSize || bombs < minBombs) ShowSizeWarning();
        else ClickStart();
    }

    public void SetDifficulty(bool toggleOn)
    {
        if (!toggleOn) return;

        var difficulty = canvas.gameDifficultyGroup.ActiveToggles().FirstOrDefault().name;
        switch (difficulty)
        {
            case "Easy":
                canvas.widthInput.text = easyWidth.ToString();
                canvas.heightInput.text = easyHeight.ToString();
                canvas.bombsInput.text = easyBombs.ToString();
                break;
            case "Medium":
                canvas.widthInput.text = mediumWidth.ToString();
                canvas.heightInput.text = mediumHeight.ToString();
                canvas.bombsInput.text = mediumBombs.ToString();
                break;
            case "Hard":
                canvas.widthInput.text = hardWidth.ToString();
                canvas.heightInput.text = hardHeight.ToString();
                canvas.bombsInput.text = hardBombs.ToString();
                break;
            default:
                canvas.widthInput.text = mediumWidth.ToString();
                canvas.heightInput.text = mediumHeight.ToString();
                canvas.bombsInput.text = mediumBombs.ToString();
                break;
        }
    }

    void ShowSizeWarning()
    {
        canvas.warningLabel.SetActive(true);
    }
}
