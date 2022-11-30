using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SudokuCell : MonoBehaviour
{
    [SerializeField] TMP_Text numberText;
    [SerializeField] TMP_Text annotationText;
    [SerializeField] Sprite sprite;
    [SerializeField] Sprite selectedSprite;

    bool firstChange = true;

    public SudokuCellData data = new();
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        var color = Color.white;
        if (data.original) color = new Color(0.596f, 0.549f, 0.384f, 1f);
        numberText.color = color;
        annotationText.color = color;
    }

    public void SetNumber(int number)
    {
        if (data.original) return;
        data.number = number;
        data.annotations.Clear();
        numberText.enabled = true;
        annotationText.enabled = false;
        if (number == 0) { if (!data.original) numberText.text = string.Empty; }
        else numberText.text = number.ToString();
    }

    public void SetAnnotation(int number)
    {
        if (data.original) return;
        if (data.annotations.Contains(number)) data.annotations.Remove(number);
        else data.annotations.Add(number);
        numberText.enabled = false;
        annotationText.enabled = true;
        string text = "";
        foreach (var annotation in data.annotations) text += annotation.ToString();
        for (int i = text.Length; i > 1; i--) text.Insert(i -1, " ");
        Debug.Log(text);
        numberText.text = data.annotations.ToString();
    }

    public void Select()
    {
        if (!data.selected)
        {
            spriteRenderer.sprite = selectedSprite;
            numberText.color = Color.black;
            annotationText.color = Color.black;
        }
        else
        {
            spriteRenderer.sprite = sprite;
            var color = Color.white;
            if (data.original) color = new Color(0.596f, 0.549f, 0.384f, 1f);
            numberText.color = color;
            annotationText.color = color;
        }
        data.selected = !data.selected;
    }
}
