using System;
using UnityEngine;
using UnityEngine.UI;

public class MinesweeperUI : MonoBehaviour
{
    public static MinesweeperUI current;

    [SerializeField] MinesweeperCanvas portraitCanvas;
    [SerializeField] MinesweeperCanvas landscapeCanvas;

    MinesweeperCanvas Canvas
    {
        get 
        {
            var isLandscape = Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight;
            return isLandscape ? landscapeCanvas : portraitCanvas; 
        }
        set 
        { 
            portraitCanvas = value; 
            landscapeCanvas = value;
        }
    }

    ScreenOrientation prevOrientation;

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

    int frameCounter;

    private void Awake()
    {
        if (current == null) current = this;
    }
    private void Start()
    {
        Game.EndGame += ShowEndScreen;
        SwapOrientation();
    }

    private void Update()
    {
        if (prevOrientation != Screen.orientation)
        {
            SwapOrientation();
            SyncCanvasData();
        }

        if (++frameCounter % 100 == 0)
        {
            Canvas.fpsCounter.text = ((int)(1 / Time.deltaTime)).ToString();
        }
        if (Canvas.bombCounter != null) Canvas.bombCounter.text = Minesweeper.current.Bombs.ToString();
    }

    void ShowEndScreen(bool won)
    {
        if (won) Canvas.winScreen.SetActive(true);
        else Canvas.gameOverScreen.SetActive(true);

        Canvas.bombCounter.enabled = false;
        Canvas.sideButtons.gameObject.SetActive(false);
    }

    public void ClickRestart()
    {
        Canvas.winScreen.SetActive(false);
        Canvas.gameOverScreen.SetActive(false);

        Canvas.startScreen.SetActive(true);
        Minesweeper.current.board.tilemap.ClearAllTiles();
    }

    public event Action ClickedStart;
    public void ClickStart()
    {
        Minesweeper.current.SetGameValues(Canvas.widthInput.text, Canvas.heightInput.text, Canvas.bombsInput.text);

        ShowGameUI();
        ClickedStart?.Invoke();
    }

    public void ShowGameUI()
    {
        Canvas.startScreen.SetActive(false);
        Canvas.winScreen.SetActive(false);
        Canvas.gameOverScreen.SetActive(false);
        Canvas.sideButtons.gameObject.SetActive(true);
        Canvas.bombCounter.enabled = true;
        Canvas.warningLabel.SetActive(false);
    }

    public void CheckInput()
    {
        int.TryParse(Canvas.widthInput.text, out var width);
        int.TryParse(Canvas.heightInput.text, out var height);
        int.TryParse(Canvas.bombsInput.text, out var bombs);

        if (width < minBoardSize || width > maxBoardSize || height < minBoardSize || height > maxBoardSize || bombs < minBombs) ShowSizeWarning();
        else ClickStart();
    }

    public void SetDifficulty(bool toggleOn)
    {
        if (!toggleOn) return;

        var difficulty = Canvas.gameDifficultyGroup.GetFirstActiveToggle().name;
        switch (difficulty)
        {
            case "Easy":
                Canvas.widthInput.text = easyWidth.ToString();
                Canvas.heightInput.text = easyHeight.ToString();
                Canvas.bombsInput.text = easyBombs.ToString();
                break;
            case "Medium":
                Canvas.widthInput.text = mediumWidth.ToString();
                Canvas.heightInput.text = mediumHeight.ToString();
                Canvas.bombsInput.text = mediumBombs.ToString();
                break;
            case "Hard":
                Canvas.widthInput.text = hardWidth.ToString();
                Canvas.heightInput.text = hardHeight.ToString();
                Canvas.bombsInput.text = hardBombs.ToString();
                break;
            default:
                Canvas.widthInput.text = mediumWidth.ToString();
                Canvas.heightInput.text = mediumHeight.ToString();
                Canvas.bombsInput.text = mediumBombs.ToString();
                break;
        }
    }

    void ShowSizeWarning()
    {
        Canvas.warningLabel.SetActive(true);
    }

    public void SwapOrientation()
    {
        if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
        {
            landscapeCanvas.gameObject.SetActive(true);
            portraitCanvas.gameObject.SetActive(false);
        }
        else
        {
            portraitCanvas.gameObject.SetActive(true);
            landscapeCanvas.gameObject.SetActive(false);
        }
        prevOrientation = Screen.orientation;
    }

    void SyncCanvasData()
    {
        MinesweeperCanvas canvasOne;
        MinesweeperCanvas canvasTwo;

        if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
        {
            canvasOne = landscapeCanvas;
            canvasTwo = portraitCanvas;
        }
        else
        {
            canvasOne = portraitCanvas;
            canvasTwo = landscapeCanvas;
        }

        canvasOne.bombCounter.text = canvasTwo.bombCounter.text;
        canvasOne.sideButtons.gameObject.SetActive(canvasTwo.sideButtons.gameObject.activeSelf);

        var sideButtonOne = canvasOne.sideButtons.GetComponentsInChildren<Toggle>(true)[0];
        var sideButtonName = sideButtonOne.name;
        var sideButtonTwo = canvasTwo.sideButtons.GetComponentsInChildren<Toggle>(true)[0].name == sideButtonName ? canvasTwo.sideButtons.GetComponentsInChildren<Toggle>(true)[0] : canvasTwo.sideButtons.GetComponentsInChildren<Toggle>(true)[1];
        sideButtonOne.isOn = sideButtonTwo.isOn;

        canvasOne.startScreen.SetActive(canvasTwo.startScreen.activeSelf);
        canvasOne.widthInput.text = canvasTwo.widthInput.text;
        canvasOne.heightInput.text = canvasTwo.heightInput.text;
        canvasOne.bombsInput.text = canvasTwo.bombsInput.text;
        canvasOne.warningLabel.SetActive(canvasTwo.warningLabel.activeSelf);

        var togglesOne = canvasOne.gameDifficultyGroup.GetComponentsInChildren<Toggle>(true);
        var togglesTwo = canvasTwo.gameDifficultyGroup.GetComponentsInChildren<Toggle>(true);
        for(int i = 0; i < togglesOne.Length; i++) if (togglesTwo[i].isOn) togglesOne[i].isOn = true;

        canvasOne.gameOverScreen.SetActive(canvasTwo.gameOverScreen.activeSelf);
        canvasOne.winScreen.SetActive(canvasTwo.winScreen.activeSelf);

        if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight) landscapeCanvas = canvasOne;
        else portraitCanvas = canvasOne;

        CameraController.current.ReCalculateMaxSize();
    }
}
