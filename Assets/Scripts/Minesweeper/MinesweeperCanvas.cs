using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinesweeperCanvas : BaseCanvas
{
    public GameObject gameOverScreen;
    public GameObject winScreen;
    public GameObject startScreen;
    public GameObject settingsScreen;
    public ToggleGroup sideButtons;

    public ToggleGroup gameDifficultyGroup;
    public TMP_InputField widthInput;
    public TMP_InputField heightInput;
    public TMP_InputField bombsInput;
    public GameObject warningLabel;

    public TMP_Text fpsCounter;
    public TMP_Text timer;
    public TMP_Text bombCounter;
}
