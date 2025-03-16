using UnityEngine;
using UnityEngine.UI; // Для работы с UI
using System.Collections.Generic;

public class CrosswordManager : MonoBehaviour
{
    public GameObject cellPrefab; // Префаб для ячейки
    public Transform gridParent; // Родительский объект для сетки
    public CrosswordData crosswordData; // Ссылка на CrosswordData

    private CrosswordGrid crosswordGrid;

    private void Start()
    {
    crosswordGrid = new CrosswordGrid(12, 14); // Размер сетки 10x10

    crosswordGrid.AddWord("МУХ", 6, 11, true); // Горизонтально
    crosswordGrid.AddWord("БАХИЛЫ", 9, 8, true); // Горизонтально
    crosswordGrid.AddWord("ИНФОРМАТИКА", 1, 11, false); // Вертикально
    crosswordGrid.AddWord("СОВМЕСТНА", 5, 0, true); // Горизонтально
    crosswordGrid.AddWord("НУЛЮ", 7, 6, false); // Вертикально
    crosswordGrid.AddWord("ГАУСА", 11, 7, true); // Горизонтально
    crosswordGrid.AddWord("КАСКАДНАЯ", 7, 0, true); // Горизонтально
    crosswordGrid.AddWord("150", 0, 0, true); // Горизонтально
    crosswordGrid.AddWord("СПОТТИ", 1, 6, true); // Горизонтально
    crosswordGrid.AddWord("КОФЕ", 7, 0, false); // Вертикально
    crosswordGrid.AddWord("ИЗРАИЛЕВИЧ", 3, 0, true); // Горизонтально
    crosswordGrid.AddWord("ЛОПИТАЛЯ", 0, 8, false); // Вертикально
    crosswordGrid.AddWord("ТРИ", 1, 4, false); // Вертикально
    }
    private void CreateCrossword()
    {
        for (int i = 0; i < crosswordGrid.Rows; i++)
        {
            for (int j = 0; j < crosswordGrid.Columns; j++)
            {
                GameObject cell = Instantiate(cellPrefab, gridParent);
                Text cellText = cell.GetComponentInChildren<Text>();
                cellText.text = crosswordGrid.Grid[i, j].ToString(); // Устанавливаем букву в ячейку
            }
        }
    }

    private void DisplayQuestions()
    {
        foreach (var question in crosswordData.questions)
        {
            Debug.Log($"Вопрос: {question.Question} - Ответ: {question.Answer}");
           
        }
    }

    private int GetRandomRow()
    {
        return Random.Range(0, crosswordGrid.Rows);
    }

    private int GetRandomColumn()
    {
        return Random.Range(0, crosswordGrid.Columns);
    }
}