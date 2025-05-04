// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager2 : MonoBehaviour
{
    public static GameManager2 Instance { get; private set; }

    [Header("Префаб и контейнер области")]
    [SerializeField] private GameObject cardPrefab;
    [Tooltip("RectTransform — область, в которой будут размещены карты")]
    [SerializeField] private RectTransform cardsContainer;

    [Header("Сетка карт")]
    [SerializeField] private int columns = 4;
    [SerializeField] private int rows = 4;

    [Header("Графика карты")]
    [Tooltip("Лицевые спрайты для первых пар")]
    public Sprite[] cardFace;
    [Tooltip("Спрайт для оборотной стороны")]
    public Sprite cardBack;
    [Tooltip("Цвета-заполнители, если спрайтов меньше, чем пар")]
    public Color[] fallbackColors;
    [Tooltip("Спрайт-накладка на цветной фон, если спрайтов не хватает")]
    public Sprite fallbackSprite;

    [Header("Ограничение по ошибкам")]
    [Tooltip("Сколько неверных пар можно открыть до проигрыша")]
    [SerializeField] private int maxAttempts = 3;
    [Tooltip("UI-текст, где выводим оставшиеся попытки")]
    [SerializeField] private Text attemptsText;

    private readonly List<CardScript> _openCards = new List<CardScript>();
    private readonly List<CardScript> _allCards = new List<CardScript>();
    private bool _acceptInput = true;
    private int _matchesRemaining;
    private int _attemptsRemaining;
    private UIManager _ui;  // ссылка на UIManager

    private void Awake()
    {
        // Синглтон
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Найти UIManager
        _ui = FindObjectOfType<UIManager>();
        if (_ui == null)
            Debug.LogError("UIManager не найден на сцене!");

        // Проверяем, назначен ли attemptsText
        if (attemptsText == null)
            Debug.LogError("GameManager2: поле attemptsText не назначено в инспекторе!");
    }

    private void Start()
    {
        // Инициализируем счётчик попыток и UI
        _attemptsRemaining = maxAttempts;
        UpdateAttemptsUI();

        // Генерируем и раскладываем карточки
        GenerateAndShuffleCards();
    }

    private void GenerateAndShuffleCards()
    {
        int total = columns * rows;
        if (total % 2 != 0)
        {
            Debug.LogError($"[GameManager2] Нужно чётное число ячеек: {total}.");
            return;
        }

        _matchesRemaining = total / 2;

        // Подготовка списка пар
        var values = new List<int>(total);
        for (int v = 1; v <= _matchesRemaining; v++)
        {
            values.Add(v);
            values.Add(v);
        }

        // Вычисляем позиции в контейнере
        Rect rect = cardsContainer.rect;
        Vector2 start = new Vector2(rect.xMin, rect.yMax);
        float stepX = rect.width / (columns - 1);
        float stepY = rect.height / (rows - 1);

        // Инстанцируем карточки
        for (int i = 0; i < total; i++)
        {
            int col = i % columns;
            int row = i / columns;
            var go = Instantiate(cardPrefab, cardsContainer);
            go.transform.localPosition = new Vector3(
                start.x + col * stepX,
                start.y - row * stepY,
                0f
            );
            _allCards.Add(go.GetComponent<CardScript>());
        }

        // Перемешиваем и назначаем графику
        var rnd = new System.Random();
        foreach (var cs in _allCards)
        {
            int idx = rnd.Next(values.Count);
            int val = values[idx];
            values.RemoveAt(idx);

            cs.cardValue = val;
            cs.backSprite = cardBack;

            if (val <= cardFace.Length)
            {
                cs.faceSprite = cardFace[val - 1];
                cs.fallbackColor = Color.clear;
                cs.fallbackSprite = null;
            }
            else
            {
                cs.faceSprite = null;
                cs.fallbackColor = fallbackColors[(val - cardFace.Length - 1) % fallbackColors.Length];
                cs.fallbackSprite = fallbackSprite;
            }

            cs.initialized = true;
            cs.SetupGraphics();
        }
    }

    /// <summary>
    /// Обрабатывает клик из CardScript.
    /// </summary>
    public void OnCardClicked(CardScript card)
    {
        if (!_acceptInput || _openCards.Contains(card) || card.state != CardScript.State.Closed)
            return;

        _openCards.Add(card);
        card.Reveal();

        if (_openCards.Count == 2)
            StartCoroutine(HandleOpenCards());
    }

    private IEnumerator HandleOpenCards()
    {
        _acceptInput = false;
        yield return new WaitForSeconds(_openCards[0].flipSpeed + 0.1f);

        var a = _openCards[0];
        var b = _openCards[1];
        bool match = (a.cardValue == b.cardValue);

        if (match)
        {
            a.SetMatched();
            b.SetMatched();
            _matchesRemaining--;

            if (_matchesRemaining == 0)
                _ui.ShowVictory();
        }
        else
        {
            a.Hide();
            b.Hide();

            _attemptsRemaining--;
            if (_attemptsRemaining > 0)
            {
                UpdateAttemptsUI();
            }
            else
            {
                _ui.ShowGameOver();
            }
        }

        _openCards.Clear();
        _acceptInput = true;
    }

    /// <summary>
    /// Обновляет UI-текст оставшихся попыток.
    /// </summary>
    private void UpdateAttemptsUI()
    {
        if (attemptsText != null)
            attemptsText.text = $"Осталось попыток: {_attemptsRemaining}";
    }

    /// <summary>
    /// Метод для UIManager – возврат на предыдущую сцену или главное меню.
    /// </summary>
    public void ReturnToPreviousScene()
    {
        SceneManager.LoadScene("menuScene");
    }
}
