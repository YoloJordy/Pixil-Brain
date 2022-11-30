using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SudokuUI : BaseUI
{
    public static SudokuUI current;

    [SerializeField] SudokuCanvas canvas;

    private void Awake()
    {
        if (current == null) current = this;
    }

    public void SetSelectedNumber (bool toggleOn)
    {
        if (!toggleOn)
        {
            Sudoku.current.ToggleOff();
            return;
        }

        var toggleName = canvas.numberToggles.ActiveToggles().FirstOrDefault().name;
        if (toggleName == null) return;
        Sudoku.current.NewToggleOn(toggleName);
    }

    public void TogglesOff()
    {
        canvas.numberToggles.SetAllTogglesOff();
    }
}
