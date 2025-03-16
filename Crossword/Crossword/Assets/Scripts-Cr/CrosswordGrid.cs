using System;

public class CrosswordGrid
{
    public char[,] Grid; // Двумерный массив для хранения букв
    public int Rows;
    public int Columns;

    public CrosswordGrid(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Grid = new char[rows, columns];

        // Инициализируем пустыми символами
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Grid[i, j] = ' '; // Или используйте '\0' для обозначения пустой ячейки
            }
        }
    }

    // Метод для добавления слова в сетку
    public void AddWord(string word, int row, int col, bool isHorizontal)
    {
        for (int i = 0; i < word.Length; i++)
        {
            if (isHorizontal)
                Grid[row, col + i] = word[i];
            else
                Grid[row + i, col] = word[i];
        }
    }
}