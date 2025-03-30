using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Добавляем пространство имён для TextMeshPro

public class SudokuGridGenerator : MonoBehaviour
{
    public GameObject blockPrefab; // Префаб блока
    public GameObject cellPrefab;  // Префаб ячейки
    public Transform gridParent;   // Родительский объект для блоков
    public int cellsToRemove = 40; // Количество ячеек для удаления (задача)
    public NumberKeyboard numberKeyboard; // Ссылка на NumberKeyboard

    // UI элементы для отображения результата проверки
    public GameObject resultPanel; // Панель для отображения результата
    public Button closeButton;     // Кнопка на ResultPanel для закрытия и очистки

    private TextMeshProUGUI resultText; // Текст на панели результата
    private int[,] sudokuGrid = new int[9, 9];
    private List<GameObject> allCells = new List<GameObject>();
    private Dictionary<int, Sprite> numberSprites = new Dictionary<int, Sprite>();
    private Sprite emptySprite; // Спрайт для пустых ячеек
    private GameObject selectedCell = null; // Выбранная ячейка

    void Start()
    {
        LoadNumberSprites();

        if (GenerateSudoku())
        {
            GenerateBlocks();
            RemoveCells(cellsToRemove);
        }
        else
        {
            Debug.LogError("Не удалось сгенерировать судоку.");
        }

        // Подписываемся на событие выбора числа из клавиатуры
        if (numberKeyboard != null)
        {
            numberKeyboard.OnNumberSelected += HandleNumberSelected;
            Debug.Log("Подписка на событие выбора числа выполнена.");
        }
        else
        {
            Debug.LogError("NumberKeyboard не назначен в инспекторе.");
        }

        // Найти и назначить resultText из дочерних объектов resultPanel
        if (resultPanel != null)
        {
            resultText = resultPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (resultText == null)
            {
                Debug.LogError("ResultPanel не содержит компонента TextMeshProUGUI.");
            }
            else
            {
                resultPanel.SetActive(false);
                Debug.Log("ResultText успешно найден и назначен.");
            }

            // Назначаем обработчик для кнопки закрытия
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseButtonClicked);
                Debug.Log("Обработчик для CloseButton назначен.");
            }
            else
            {
                Debug.LogError("CloseButton не назначен в инспекторе.");
            }
        }
        else
        {
            Debug.LogError("ResultPanel не назначен в инспекторе.");
        }
    }

    // Загружает спрайты чисел и пустой спрайт из папки Resources/Numbers
    void LoadNumberSprites()
    {
        for (int i = 1; i <= 9; i++)
        {
            Sprite sprite = Resources.Load<Sprite>("Numbers/" + i);
            if (sprite != null)
            {
                numberSprites.Add(i, sprite);
                Debug.Log($"Спрайт для числа {i} загружен.");
            }
            else
            {
                Debug.LogError($"Не удалось загрузить спрайт для числа {i}");
            }
        }

        // Загрузка пустого спрайта (white.png)
        emptySprite = Resources.Load<Sprite>("Numbers/white");
        if (emptySprite != null)
        {
            Debug.Log("Пустой спрайт загружен.");
        }
        else
        {
            Debug.LogError("Не удалось загрузить пустой спрайт (white.png) из папки Resources/Numbers.");
        }
    }

    // Метод генерации полного судоку с использованием backtracking
    bool GenerateSudoku()
    {
        return FillSudoku(0, 0);
    }

    bool FillSudoku(int row, int col)
    {
        if (row == 9)
            return true;

        if (col == 9)
            return FillSudoku(row + 1, 0);

        List<int> numbers = new List<int>();
        for (int i = 1; i <= 9; i++) numbers.Add(i);
        Shuffle(numbers);

        foreach (int number in numbers)
        {
            if (IsSafe(row, col, number))
            {
                sudokuGrid[row, col] = number;
                if (FillSudoku(row, col + 1))
                    return true;
                sudokuGrid[row, col] = 0;
            }
        }

        return false;
    }

    void Shuffle(List<int> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    bool IsSafe(int row, int col, int num)
    {
        // Проверка строки и столбца
        for (int x = 0; x < 9; x++)
        {
            if (sudokuGrid[row, x] == num || sudokuGrid[x, col] == num)
                return false;
        }

        // Проверка блока 3x3
        int startRow = row - row % 3;
        int startCol = col - col % 3;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (sudokuGrid[startRow + i, startCol + j] == num)
                    return false;

        return true;
    }

    void GenerateBlocks()
    {
        for (int block = 0; block < 9; block++)
        {
            // Создаём блок
            GameObject blockObj = Instantiate(blockPrefab, gridParent);
            blockObj.name = "Block_" + (block + 1);

            // Генерируем 9 ячеек внутри блока
            for (int cell = 0; cell < 9; cell++)
            {
                int row = (block / 3) * 3 + cell / 3;
                int col = (block % 3) * 3 + cell % 3;

                GameObject cellObj = Instantiate(cellPrefab, blockObj.transform);
                cellObj.name = $"Cell_{row}_{col}";

                // Получаем компоненты Image и Button
                Button cellButton = cellObj.GetComponent<Button>();
                if (cellButton == null)
                {
                    // Если Button не найден на корневом объекте, ищем в дочерних
                    cellButton = cellObj.GetComponentInChildren<Button>();
                }

                Image cellImage = cellObj.GetComponent<Image>();
                if (cellImage == null)
                {
                    // Если Image не найден на корневом объекте, ищем в дочерних
                    cellImage = cellObj.GetComponentInChildren<Image>();
                }

                if (cellImage == null || cellButton == null)
                {
                    Debug.LogError($"CellPrefab '{cellObj.name}' не содержит необходимых компонентов Image и Button.");
                    continue;
                }

                if (sudokuGrid[row, col] != 0)
                {
                    // Устанавливаем изображение числа
                    if (numberSprites.ContainsKey(sudokuGrid[row, col]))
                    {
                        cellImage.sprite = numberSprites[sudokuGrid[row, col]];
                        cellImage.enabled = true;
                        Debug.Log($"Ячейка {cellObj.name} установлена с числом {sudokuGrid[row, col]}.");
                        // Добавляем обработчик события для кнопки
                        cellButton.onClick.AddListener(() => OnCellClicked(cellObj));
                    }
                    else
                    {
                        cellImage.enabled = false;
                        Debug.LogError($"Спрайт для числа {sudokuGrid[row, col]} не найден.");
                    }

                    // Делаем кнопку неактивной
                    cellButton.interactable = false;
                }
                else
                {
                    // Устанавливаем пустой спрайт (white.png)
                    if (emptySprite != null)
                    {
                        cellImage.sprite = emptySprite;
                        cellImage.enabled = true;
                        Debug.Log($"Ячейка {cellObj.name} установлена как пустая.");
                    }
                    else
                    {
                        cellImage.sprite = null;
                        cellImage.enabled = false;
                        Debug.LogWarning($"Пустой спрайт не загружен для ячейки {cellObj.name}.");
                    }

                    // Делаем кнопку активной для ввода
                    cellButton.interactable = true;

                    // Добавляем обработчик события для кнопки
                    cellButton.onClick.AddListener(() => OnCellClicked(cellObj));
                    Debug.Log($"Кнопка для ячейки {cellObj.name} активирована.");
                }

                allCells.Add(cellObj);
            }
        }
    }

    // Метод удаления заданного количества ячеек для создания задачи
    void RemoveCells(int count)
    {
        int removed = 0;
        System.Random rng = new System.Random();
        List<GameObject> removableCells = new List<GameObject>(allCells);

        while (removed < count && removableCells.Count > 0)
        {
            int index = rng.Next(removableCells.Count);
            GameObject cell = removableCells[index];
            Image cellImage = cell.GetComponentInChildren<Image>();
            Button cellButton = cell.GetComponentInChildren<Button>();

            if (cellImage.sprite != null && cellImage.sprite != emptySprite)
            {
                // Устанавливаем пустой спрайт вместо числа
                if (emptySprite != null)
                {
                    cellImage.sprite = emptySprite;
                    Debug.Log($"Ячейка {cell.name} очищена (установлен пустой спрайт).");
                }
                else
                {
                    cellImage.sprite = null;
                    cellImage.enabled = false;
                    Debug.LogWarning($"Пустой спрайт не загружен для ячейки {cell.name}.");
                }

                // Делаем кнопку активной для ввода
                cellButton.interactable = true;

                // Удаляем ячейку из списка, чтобы не удалять её снова
                removableCells.RemoveAt(index);
                removed++;

                Debug.Log($"Ячейка {cell.name} очищена и кнопка активирована.");
            }
        }

        if (removed < count)
        {
            Debug.LogWarning($"Не удалось удалить требуемое количество ячеек. Удалено: {removed}");
        }
    }

    // Обработчик клика по ячейке
    void OnCellClicked(GameObject cell)
    {
        // Устанавливаем выбранную ячейку
        selectedCell = cell;

        // Включаем подсветку выбранной ячейки (опционально)
        HighlightSelectedCell();

        Debug.Log($"Ячейка {cell.name} выбрана для ввода числа.");
    }

    // Метод обработки выбранного числа из клавиатуры
    void HandleNumberSelected(int number)
    {
        Debug.Log($"Обработка выбранного числа: {number}");
        if (selectedCell == null)
        {
            Debug.LogWarning("Не выбрана ячейка для ввода числа.");
            return;
        }

        Image cellImage = selectedCell.GetComponentInChildren<Image>();
        Button cellButton = selectedCell.GetComponentInChildren<Button>();

        if (number == 0)
        {
            // Очистка ячейки (если реализована кнопка "Clear")
            if (emptySprite != null)
            {
                cellImage.sprite = emptySprite;
                cellButton.interactable = true; // Делаем кнопку снова активной
                Debug.Log($"Ячейка {selectedCell.name} очищена.");
            }
            else
            {
                cellImage.sprite = null;
                cellImage.enabled = false;
                Debug.LogWarning($"Пустой спрайт не загружен для ячейки {selectedCell.name}.");
            }
        }
        else if (numberSprites.ContainsKey(number))
        {
            cellImage.sprite = numberSprites[number];
            //cellButton.interactable = false; // Делаем кнопку неактивной после ввода числа
            Debug.Log($"В ячейку {selectedCell.name} установлено число {number}.");
        }
        else
        {
            Debug.LogError($"Спрайт для числа {number} не найден.");
        }

        // Снимаем выделение с ячейки
        selectedCell = null;
        RemoveHighlightFromCells();

        // Проверяем, заполнены ли все ячейки
        if (AreAllCellsFilled())
        {
            Debug.Log("Все ячейки заполнены. Проверка решения.");
            CheckSolution();
        }
    }

    // Метод подсветки выбранной ячейки (опционально)
    void HighlightSelectedCell()
    {
        RemoveHighlightFromCells(); // Убираем подсветку с других ячеек

        // Предположим, что у ячейки есть компонент Image для изменения цвета
        Image cellImage = selectedCell.GetComponentInChildren<Image>();
        if (cellImage != null)
        {
            cellImage.color = Color.yellow; // Например, желтый цвет для подсветки
        }
    }

    // Метод удаления подсветки со всех ячеек (опционально)
    void RemoveHighlightFromCells()
    {
        foreach (GameObject cell in allCells)
        {
            Image cellImage = cell.GetComponentInChildren<Image>();
            if (cellImage != null && cellImage.sprite == emptySprite)
            {
                cellImage.color = Color.white; // Возвращаем исходный цвет
            }
        }
    }

    // Метод проверки, заполнены ли все ячейки
    bool AreAllCellsFilled()
    {
        foreach (GameObject cell in allCells)
        {
            Image cellImage = cell.GetComponentInChildren<Image>();
            if (cellImage.sprite == emptySprite || cellImage.sprite == null)
            {
                return false;
            }
        }
        return true;
    }

    // Метод проверки правильности заполнения судоку
    void CheckSolution()
    {
        bool isCorrect = true;
        List<GameObject> incorrectCells = new List<GameObject>();

        foreach (GameObject cell in allCells)
        {
            string[] parts = cell.name.Split('_');
            if (parts.Length != 3)
            {
                Debug.LogError($"Некорректное имя ячейки: {cell.name}");
                continue;
            }

            if (!int.TryParse(parts[1], out int row) || !int.TryParse(parts[2], out int col))
            {
                Debug.LogError($"Некорректные координаты в имени ячейки: {cell.name}");
                continue;
            }

            Image cellImage = cell.GetComponentInChildren<Image>();
            if (cellImage == null)
            {
                Debug.LogError($"Ячейка {cell.name} не содержит компонента Image.");
                continue;
            }

            if (cellImage.sprite == emptySprite || cellImage.sprite == null)
            {
                // Ячейка пустая, но мы уже проверили, что все ячейки заполнены
                Debug.LogWarning($"Ячейка {cell.name} должна быть заполнена, но пуста.");
                isCorrect = false;
                incorrectCells.Add(cell);
                continue;
            }

            int userNumber = GetNumberFromSprite(cellImage.sprite);
            if (userNumber != sudokuGrid[row, col])
            {
                isCorrect = false;
                incorrectCells.Add(cell);
            }
        }

        if (isCorrect)
        {
            Debug.Log("Поздравляем! Вы правильно решили судоку.");
            ShowResult("Поздравляем! Вы правильно решили судоку.", Color.green);
        }
        else
        {
            Debug.Log("Некоторые числа введены неверно. Пожалуйста, исправьте их.");
            ShowResult("Некоторые числа введены неверно. Пожалуйста, исправьте их.", Color.red);

            // Подсвечиваем неправильные ячейки
            foreach (GameObject cell in incorrectCells)
            {
                Image cellImage = cell.GetComponentInChildren<Image>();
                if (cellImage != null)
                {
                    cellImage.color = Color.red; // Красная подсветка для неправильных ячеек
                }
            }
        }
    }

    // Метод для получения числа из спрайта
    int GetNumberFromSprite(Sprite sprite)
    {
        foreach (var pair in numberSprites)
        {
            if (pair.Value == sprite)
                return pair.Key;
        }
        return 0; // Возвращаем 0, если спрайт не найден
    }

    // Метод отображения результата проверки в UI
    void ShowResult(string message, Color color)
    {
        if (resultPanel != null && resultText != null)
        {
            resultPanel.SetActive(true);
            resultText.text = message;
            resultText.color = color;
            Debug.Log($"Результат отображен: {message}");
        }
        else
        {
            Debug.LogWarning("Result Panel или Result Text не найдены.");
        }
    }

    // Метод обработки нажатия на кнопку "Закрыть" на ResultPanel
    void OnCloseButtonClicked()
    {
        Debug.Log("Кнопка CloseButton нажата. Скрываем ResultPanel и очищаем пользовательские ячейки.");

        // Скрываем ResultPanel
        resultPanel.SetActive(false);

        // Очищаем пользовательские ячейки
        ClearUserCells();

        // Проверяем, была ли победа
        if (resultText.text.Contains("Поздравляем"))
        {
            // Телепортируемся обратно на предыдущую сцену
            TeleportBackToPreviousScene();
        }
    }

    // Метод для телепортации обратно на предыдущую сцену через GameManager
    void TeleportBackToPreviousScene()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log("Инициация возврата на предыдущую сцену через GameManager.");
            GameManager.Instance.ReturnToPreviousScene();
        }
        else
        {
            Debug.LogError("GameManager не найден. Убедитесь, что объект GameManager присутствует в сцене.");
        }
    }

    // Метод для очистки пользовательских ячеек
    void ClearUserCells()
    {
        foreach (GameObject cell in allCells)
        {
            Button cellButton = cell.GetComponentInChildren<Button>();
            Image cellImage = cell.GetComponentInChildren<Image>();

            // Проверяем, является ли ячейка пользовательской (interactable)
            if (cellButton != null && cellButton.interactable)
            {
                // Очищаем спрайт ячейки
                if (emptySprite != null)
                {
                    cellImage.sprite = emptySprite;
                    cellImage.enabled = true;
                }
                else
                {
                    cellImage.sprite = null;
                    cellImage.enabled = false;
                    Debug.LogWarning($"Пустой спрайт не загружен для ячейки {cell.name}.");
                }

                // Делаем кнопку снова активной для ввода
                cellButton.interactable = true;

                // Снимаем подсветку, если она была
                cellImage.color = Color.white;
            }
        }

        Debug.Log("Все пользовательские ячейки очищены.");
    }
}
