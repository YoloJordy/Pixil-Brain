using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinesweeperUI : MonoBehaviour
{
    public static MinesweeperUI current;

    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject startScreen;
    [SerializeField] GameObject sideButtons;

    [SerializeField] InputField widthInput;
    [SerializeField] InputField heightInput;
    [SerializeField] InputField bombsInput;
    [SerializeField] GameObject warningLabel;

    [SerializeField] TMP_Text fpsCounter;

    [SerializeField] int minBoardSize = 4;
    [SerializeField] int maxBoardSize = 99;
    [SerializeField] int minBombs = 1;

    int frameCounter;

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
        if (++frameCounter % 100 == 0)
        {
            fpsCounter.text = ((int)(1 / Time.deltaTime)).ToString();
        }
    }

    void ShowEndScreen(bool won)
    {
        if (won) winScreen.SetActive(true);
        else gameOverScreen.SetActive(true);
        sideButtons.SetActive(false);
    }

    public void ClickRestart()
    {
        winScreen.SetActive(false);
        gameOverScreen.SetActive(false);

        startScreen.SetActive(true);
        Minesweeper.current.board.tilemap.ClearAllTiles();
    }

    public event Action ClickedStart;
    public void ClickStart()
    {
        Minesweeper.current.SetGameValues(widthInput.text, heightInput.text, bombsInput.text);

        ShowGameUI();
        ClickedStart?.Invoke();
    }

    public void ShowGameUI()
    {
        startScreen.SetActive(false);
        winScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        sideButtons.SetActive(true);
        warningLabel.SetActive(false);
    }

    public void CheckInput()
    {
        int.TryParse(widthInput.text, out var width);
        int.TryParse(heightInput.text, out var height);
        int.TryParse(bombsInput.text, out var bombs);

        if (width < minBoardSize || width > maxBoardSize || height < minBoardSize || height > maxBoardSize || bombs < minBombs) ShowSizeWarning();
        else ClickStart();
    }

    void ShowSizeWarning()
    {
        warningLabel.SetActive(true);
    }
}
