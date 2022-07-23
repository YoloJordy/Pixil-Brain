using System;
using UnityEngine;
using UnityEngine.UI;

public class MinesweeperUI : MonoBehaviour
{
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject startScreen;

    [SerializeField] InputField widthInput;
    [SerializeField] InputField heightInput;
    [SerializeField] InputField bombsInput;
    [SerializeField] GameObject warningLabel;

    [SerializeField] int minBoardSize = 4;
    [SerializeField] int maxBoardSize = 99;
    [SerializeField] int minBombs = 1;

    public static MinesweeperUI current;

    private void Awake()
    {
        if (current == null) current = this;
    }
    private void Start()
    {
        Minesweeper.current.EndGame += ShowEndScreen;
    }

    void ShowEndScreen(bool won)
    {
        if (won) winScreen.SetActive(true);
        else gameOverScreen.SetActive(true);
    }

    public void ClickRestart()
    {
        winScreen.SetActive(false);
        gameOverScreen.SetActive(false);

        startScreen.SetActive(true);
    }

    public event Action ClickedStart;
    public void ClickStart()
    {
        Minesweeper.current.SetGameValues(widthInput.text, heightInput.text, bombsInput.text);

        startScreen.SetActive(false);
        warningLabel.SetActive(false);
        ClickedStart?.Invoke();
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
