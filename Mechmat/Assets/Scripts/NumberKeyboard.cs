using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberKeyboard : MonoBehaviour
{
    // ������ ������ �����
    public GameObject numberButtonPrefab;

    // ������������ ������ ��� ������ (Panel � GridLayout)
    public Transform keyboardParent;

    // ������� ��� �������� ���������� �����
    public delegate void NumberSelected(int number);
    public event NumberSelected OnNumberSelected;

    // ���� � �������� ����� � Resources
    private string numberSpritePath = "Numbers/";

    void Start()
    {
        if (numberButtonPrefab == null)
        {
            Debug.LogError("Number Button Prefab �� �������� � ����������.");
            return;
        }

        if (keyboardParent == null)
        {
            Debug.LogError("Keyboard Parent �� �������� � ����������.");
            return;
        }

        // ������ ������ ����� �� 1 �� 9
        for (int i = 1; i <= 9; i++)
        {
            CreateNumberButton(i);
        }
    }

    // ����� ��� �������� ������ �����
    void CreateNumberButton(int number)
    {
        // ������������� ������ �� �������
        GameObject buttonObj = Instantiate(numberButtonPrefab, keyboardParent);
        buttonObj.name = $"NumberButton_{number}";

        // �������� ���������� Button � Image
        Button button = buttonObj.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError($"Prefab {numberButtonPrefab.name} �� �������� ���������� Button.");
            return;
        }

        Image buttonImage = buttonObj.GetComponent<Image>();
        if (buttonImage == null)
        {
            // ���� Image �� �� �������� �������, ���� � ��������
            buttonImage = buttonObj.GetComponentInChildren<Image>();
            if (buttonImage == null)
            {
                Debug.LogError($"Prefab {numberButtonPrefab.name} �� �������� ���������� Image.");
                return;
            }
        }

        // ��������� ������ ����� �� Resources
        Sprite numberSprite = Resources.Load<Sprite>(numberSpritePath + number);
        if (numberSprite == null)
        {
            Debug.LogError($"�� ������� ��������� ������ ��� ����� {number} �� ���� {numberSpritePath + number}.");
            return;
        }

        // ������������� ������ �� ������
        buttonImage.sprite = numberSprite;
        buttonImage.preserveAspect = true;

        // ��������� ��������� ������� �� ������
        button.onClick.AddListener(() => NumberButtonClicked(number));
    }

    // �����, ���������� ��� ������� �� ������ �����
    void NumberButtonClicked(int number)
    {
        // �������� ������� � ������� ��������� �����
        OnNumberSelected?.Invoke(number);
    }
}
