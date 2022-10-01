using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SudokuUI : MonoBehaviour
{
    [SerializeField] SudokuCanvas canvas;
    public void SetSelectedNumber (bool toggleOn)
    {
        if (!toggleOn) return;

        var toggleName = canvas.numberToggles.ActiveToggles().FirstOrDefault().name;
        if (toggleName == null) return;
        Sudoku.current.NumberToggleSelected(toggleName);
    }
}
