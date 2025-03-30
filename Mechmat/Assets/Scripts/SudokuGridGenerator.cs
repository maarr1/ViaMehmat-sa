using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // ��������� ������������ ��� ��� TextMeshPro

public class SudokuGridGenerator : MonoBehaviour
{
    public GameObject blockPrefab; // ������ �����
    public GameObject cellPrefab;  // ������ ������
    public Transform gridParent;   // ������������ ������ ��� ������
    public int cellsToRemove = 40; // ���������� ����� ��� �������� (������)
    public NumberKeyboard numberKeyboard; // ������ �� NumberKeyboard

    // UI �������� ��� ����������� ���������� ��������
    public GameObject resultPanel; // ������ ��� ����������� ����������
    public Button closeButton;     // ������ �� ResultPanel ��� �������� � �������

    private TextMeshProUGUI resultText; // ����� �� ������ ����������
    private int[,] sudokuGrid = new int[9, 9];
    private List<GameObject> allCells = new List<GameObject>();
    private Dictionary<int, Sprite> numberSprites = new Dictionary<int, Sprite>();
    private Sprite emptySprite; // ������ ��� ������ �����
    private GameObject selectedCell = null; // ��������� ������

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
            Debug.LogError("�� ������� ������������� ������.");
        }

        // ������������� �� ������� ������ ����� �� ����������
        if (numberKeyboard != null)
        {
            numberKeyboard.OnNumberSelected += HandleNumberSelected;
            Debug.Log("�������� �� ������� ������ ����� ���������.");
        }
        else
        {
            Debug.LogError("NumberKeyboard �� �������� � ����������.");
        }

        // ����� � ��������� resultText �� �������� �������� resultPanel
        if (resultPanel != null)
        {
            resultText = resultPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (resultText == null)
            {
                Debug.LogError("ResultPanel �� �������� ���������� TextMeshProUGUI.");
            }
            else
            {
                resultPanel.SetActive(false);
                Debug.Log("ResultText ������� ������ � ��������.");
            }

            // ��������� ���������� ��� ������ ��������
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnCloseButtonClicked);
                Debug.Log("���������� ��� CloseButton ��������.");
            }
            else
            {
                Debug.LogError("CloseButton �� �������� � ����������.");
            }
        }
        else
        {
            Debug.LogError("ResultPanel �� �������� � ����������.");
        }
    }

    // ��������� ������� ����� � ������ ������ �� ����� Resources/Numbers
    void LoadNumberSprites()
    {
        for (int i = 1; i <= 9; i++)
        {
            Sprite sprite = Resources.Load<Sprite>("Numbers/" + i);
            if (sprite != null)
            {
                numberSprites.Add(i, sprite);
                Debug.Log($"������ ��� ����� {i} ��������.");
            }
            else
            {
                Debug.LogError($"�� ������� ��������� ������ ��� ����� {i}");
            }
        }

        // �������� ������� ������� (white.png)
        emptySprite = Resources.Load<Sprite>("Numbers/white");
        if (emptySprite != null)
        {
            Debug.Log("������ ������ ��������.");
        }
        else
        {
            Debug.LogError("�� ������� ��������� ������ ������ (white.png) �� ����� Resources/Numbers.");
        }
    }

    // ����� ��������� ������� ������ � �������������� backtracking
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
        // �������� ������ � �������
        for (int x = 0; x < 9; x++)
        {
            if (sudokuGrid[row, x] == num || sudokuGrid[x, col] == num)
                return false;
        }

        // �������� ����� 3x3
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
            // ������ ����
            GameObject blockObj = Instantiate(blockPrefab, gridParent);
            blockObj.name = "Block_" + (block + 1);

            // ���������� 9 ����� ������ �����
            for (int cell = 0; cell < 9; cell++)
            {
                int row = (block / 3) * 3 + cell / 3;
                int col = (block % 3) * 3 + cell % 3;

                GameObject cellObj = Instantiate(cellPrefab, blockObj.transform);
                cellObj.name = $"Cell_{row}_{col}";

                // �������� ���������� Image � Button
                Button cellButton = cellObj.GetComponent<Button>();
                if (cellButton == null)
                {
                    // ���� Button �� ������ �� �������� �������, ���� � ��������
                    cellButton = cellObj.GetComponentInChildren<Button>();
                }

                Image cellImage = cellObj.GetComponent<Image>();
                if (cellImage == null)
                {
                    // ���� Image �� ������ �� �������� �������, ���� � ��������
                    cellImage = cellObj.GetComponentInChildren<Image>();
                }

                if (cellImage == null || cellButton == null)
                {
                    Debug.LogError($"CellPrefab '{cellObj.name}' �� �������� ����������� ����������� Image � Button.");
                    continue;
                }

                if (sudokuGrid[row, col] != 0)
                {
                    // ������������� ����������� �����
                    if (numberSprites.ContainsKey(sudokuGrid[row, col]))
                    {
                        cellImage.sprite = numberSprites[sudokuGrid[row, col]];
                        cellImage.enabled = true;
                        Debug.Log($"������ {cellObj.name} ����������� � ������ {sudokuGrid[row, col]}.");
                        // ��������� ���������� ������� ��� ������
                        cellButton.onClick.AddListener(() => OnCellClicked(cellObj));
                    }
                    else
                    {
                        cellImage.enabled = false;
                        Debug.LogError($"������ ��� ����� {sudokuGrid[row, col]} �� ������.");
                    }

                    // ������ ������ ����������
                    cellButton.interactable = false;
                }
                else
                {
                    // ������������� ������ ������ (white.png)
                    if (emptySprite != null)
                    {
                        cellImage.sprite = emptySprite;
                        cellImage.enabled = true;
                        Debug.Log($"������ {cellObj.name} ����������� ��� ������.");
                    }
                    else
                    {
                        cellImage.sprite = null;
                        cellImage.enabled = false;
                        Debug.LogWarning($"������ ������ �� �������� ��� ������ {cellObj.name}.");
                    }

                    // ������ ������ �������� ��� �����
                    cellButton.interactable = true;

                    // ��������� ���������� ������� ��� ������
                    cellButton.onClick.AddListener(() => OnCellClicked(cellObj));
                    Debug.Log($"������ ��� ������ {cellObj.name} ������������.");
                }

                allCells.Add(cellObj);
            }
        }
    }

    // ����� �������� ��������� ���������� ����� ��� �������� ������
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
                // ������������� ������ ������ ������ �����
                if (emptySprite != null)
                {
                    cellImage.sprite = emptySprite;
                    Debug.Log($"������ {cell.name} ������� (���������� ������ ������).");
                }
                else
                {
                    cellImage.sprite = null;
                    cellImage.enabled = false;
                    Debug.LogWarning($"������ ������ �� �������� ��� ������ {cell.name}.");
                }

                // ������ ������ �������� ��� �����
                cellButton.interactable = true;

                // ������� ������ �� ������, ����� �� ������� � �����
                removableCells.RemoveAt(index);
                removed++;

                Debug.Log($"������ {cell.name} ������� � ������ ������������.");
            }
        }

        if (removed < count)
        {
            Debug.LogWarning($"�� ������� ������� ��������� ���������� �����. �������: {removed}");
        }
    }

    // ���������� ����� �� ������
    void OnCellClicked(GameObject cell)
    {
        // ������������� ��������� ������
        selectedCell = cell;

        // �������� ��������� ��������� ������ (�����������)
        HighlightSelectedCell();

        Debug.Log($"������ {cell.name} ������� ��� ����� �����.");
    }

    // ����� ��������� ���������� ����� �� ����������
    void HandleNumberSelected(int number)
    {
        Debug.Log($"��������� ���������� �����: {number}");
        if (selectedCell == null)
        {
            Debug.LogWarning("�� ������� ������ ��� ����� �����.");
            return;
        }

        Image cellImage = selectedCell.GetComponentInChildren<Image>();
        Button cellButton = selectedCell.GetComponentInChildren<Button>();

        if (number == 0)
        {
            // ������� ������ (���� ����������� ������ "Clear")
            if (emptySprite != null)
            {
                cellImage.sprite = emptySprite;
                cellButton.interactable = true; // ������ ������ ����� ��������
                Debug.Log($"������ {selectedCell.name} �������.");
            }
            else
            {
                cellImage.sprite = null;
                cellImage.enabled = false;
                Debug.LogWarning($"������ ������ �� �������� ��� ������ {selectedCell.name}.");
            }
        }
        else if (numberSprites.ContainsKey(number))
        {
            cellImage.sprite = numberSprites[number];
            //cellButton.interactable = false; // ������ ������ ���������� ����� ����� �����
            Debug.Log($"� ������ {selectedCell.name} ����������� ����� {number}.");
        }
        else
        {
            Debug.LogError($"������ ��� ����� {number} �� ������.");
        }

        // ������� ��������� � ������
        selectedCell = null;
        RemoveHighlightFromCells();

        // ���������, ��������� �� ��� ������
        if (AreAllCellsFilled())
        {
            Debug.Log("��� ������ ���������. �������� �������.");
            CheckSolution();
        }
    }

    // ����� ��������� ��������� ������ (�����������)
    void HighlightSelectedCell()
    {
        RemoveHighlightFromCells(); // ������� ��������� � ������ �����

        // �����������, ��� � ������ ���� ��������� Image ��� ��������� �����
        Image cellImage = selectedCell.GetComponentInChildren<Image>();
        if (cellImage != null)
        {
            cellImage.color = Color.yellow; // ��������, ������ ���� ��� ���������
        }
    }

    // ����� �������� ��������� �� ���� ����� (�����������)
    void RemoveHighlightFromCells()
    {
        foreach (GameObject cell in allCells)
        {
            Image cellImage = cell.GetComponentInChildren<Image>();
            if (cellImage != null && cellImage.sprite == emptySprite)
            {
                cellImage.color = Color.white; // ���������� �������� ����
            }
        }
    }

    // ����� ��������, ��������� �� ��� ������
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

    // ����� �������� ������������ ���������� ������
    void CheckSolution()
    {
        bool isCorrect = true;
        List<GameObject> incorrectCells = new List<GameObject>();

        foreach (GameObject cell in allCells)
        {
            string[] parts = cell.name.Split('_');
            if (parts.Length != 3)
            {
                Debug.LogError($"������������ ��� ������: {cell.name}");
                continue;
            }

            if (!int.TryParse(parts[1], out int row) || !int.TryParse(parts[2], out int col))
            {
                Debug.LogError($"������������ ���������� � ����� ������: {cell.name}");
                continue;
            }

            Image cellImage = cell.GetComponentInChildren<Image>();
            if (cellImage == null)
            {
                Debug.LogError($"������ {cell.name} �� �������� ���������� Image.");
                continue;
            }

            if (cellImage.sprite == emptySprite || cellImage.sprite == null)
            {
                // ������ ������, �� �� ��� ���������, ��� ��� ������ ���������
                Debug.LogWarning($"������ {cell.name} ������ ���� ���������, �� �����.");
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
            Debug.Log("�����������! �� ��������� ������ ������.");
            ShowResult("�����������! �� ��������� ������ ������.", Color.green);
        }
        else
        {
            Debug.Log("��������� ����� ������� �������. ����������, ��������� ��.");
            ShowResult("��������� ����� ������� �������. ����������, ��������� ��.", Color.red);

            // ������������ ������������ ������
            foreach (GameObject cell in incorrectCells)
            {
                Image cellImage = cell.GetComponentInChildren<Image>();
                if (cellImage != null)
                {
                    cellImage.color = Color.red; // ������� ��������� ��� ������������ �����
                }
            }
        }
    }

    // ����� ��� ��������� ����� �� �������
    int GetNumberFromSprite(Sprite sprite)
    {
        foreach (var pair in numberSprites)
        {
            if (pair.Value == sprite)
                return pair.Key;
        }
        return 0; // ���������� 0, ���� ������ �� ������
    }

    // ����� ����������� ���������� �������� � UI
    void ShowResult(string message, Color color)
    {
        if (resultPanel != null && resultText != null)
        {
            resultPanel.SetActive(true);
            resultText.text = message;
            resultText.color = color;
            Debug.Log($"��������� ���������: {message}");
        }
        else
        {
            Debug.LogWarning("Result Panel ��� Result Text �� �������.");
        }
    }

    // ����� ��������� ������� �� ������ "�������" �� ResultPanel
    void OnCloseButtonClicked()
    {
        Debug.Log("������ CloseButton ������. �������� ResultPanel � ������� ���������������� ������.");

        // �������� ResultPanel
        resultPanel.SetActive(false);

        // ������� ���������������� ������
        ClearUserCells();

        // ���������, ���� �� ������
        if (resultText.text.Contains("�����������"))
        {
            // ��������������� ������� �� ���������� �����
            TeleportBackToPreviousScene();
        }
    }

    // ����� ��� ������������ ������� �� ���������� ����� ����� GameManager
    void TeleportBackToPreviousScene()
    {
        if (GameManager.Instance != null)
        {
            Debug.Log("��������� �������� �� ���������� ����� ����� GameManager.");
            GameManager.Instance.ReturnToPreviousScene();
        }
        else
        {
            Debug.LogError("GameManager �� ������. ���������, ��� ������ GameManager ������������ � �����.");
        }
    }

    // ����� ��� ������� ���������������� �����
    void ClearUserCells()
    {
        foreach (GameObject cell in allCells)
        {
            Button cellButton = cell.GetComponentInChildren<Button>();
            Image cellImage = cell.GetComponentInChildren<Image>();

            // ���������, �������� �� ������ ���������������� (interactable)
            if (cellButton != null && cellButton.interactable)
            {
                // ������� ������ ������
                if (emptySprite != null)
                {
                    cellImage.sprite = emptySprite;
                    cellImage.enabled = true;
                }
                else
                {
                    cellImage.sprite = null;
                    cellImage.enabled = false;
                    Debug.LogWarning($"������ ������ �� �������� ��� ������ {cell.name}.");
                }

                // ������ ������ ����� �������� ��� �����
                cellButton.interactable = true;

                // ������� ���������, ���� ��� ����
                cellImage.color = Color.white;
            }
        }

        Debug.Log("��� ���������������� ������ �������.");
    }
}
