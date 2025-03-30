using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberKeyboard : MonoBehaviour
{
    // Префаб кнопки числа
    public GameObject numberButtonPrefab;

    // Родительский объект для кнопок (Panel с GridLayout)
    public Transform keyboardParent;

    // Событие для передачи выбранного числа
    public delegate void NumberSelected(int number);
    public event NumberSelected OnNumberSelected;

    // Путь к спрайтам чисел в Resources
    private string numberSpritePath = "Numbers/";

    void Start()
    {
        if (numberButtonPrefab == null)
        {
            Debug.LogError("Number Button Prefab не назначен в инспекторе.");
            return;
        }

        if (keyboardParent == null)
        {
            Debug.LogError("Keyboard Parent не назначен в инспекторе.");
            return;
        }

        // Создаём кнопки чисел от 1 до 9
        for (int i = 1; i <= 9; i++)
        {
            CreateNumberButton(i);
        }
    }

    // Метод для создания кнопки числа
    void CreateNumberButton(int number)
    {
        // Инстанциируем кнопку из префаба
        GameObject buttonObj = Instantiate(numberButtonPrefab, keyboardParent);
        buttonObj.name = $"NumberButton_{number}";

        // Получаем компоненты Button и Image
        Button button = buttonObj.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError($"Prefab {numberButtonPrefab.name} не содержит компонента Button.");
            return;
        }

        Image buttonImage = buttonObj.GetComponent<Image>();
        if (buttonImage == null)
        {
            // Если Image не на корневом объекте, ищем в дочерних
            buttonImage = buttonObj.GetComponentInChildren<Image>();
            if (buttonImage == null)
            {
                Debug.LogError($"Prefab {numberButtonPrefab.name} не содержит компонента Image.");
                return;
            }
        }

        // Загружаем спрайт числа из Resources
        Sprite numberSprite = Resources.Load<Sprite>(numberSpritePath + number);
        if (numberSprite == null)
        {
            Debug.LogError($"Не удалось загрузить спрайт для числа {number} по пути {numberSpritePath + number}.");
            return;
        }

        // Устанавливаем спрайт на кнопку
        buttonImage.sprite = numberSprite;
        buttonImage.preserveAspect = true;

        // Добавляем слушатель нажатия на кнопку
        button.onClick.AddListener(() => NumberButtonClicked(number));
    }

    // Метод, вызываемый при нажатии на кнопку числа
    void NumberButtonClicked(int number)
    {
        // Вызываем событие и передаём выбранное число
        OnNumberSelected?.Invoke(number);
    }
}
