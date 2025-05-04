// InfiniteLevel.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;

public class InfiniteLevel : MonoBehaviour
{
    public enum ScrollDir { Left = -1, Right = 1 }

    [Header("Направление прокрутки")]
    public ScrollDir scrollDirection = ScrollDir.Left;

    [Header("=== Камера и фон ===")]
    public GameObject backgroundPrefab;
    public float scrollSpeed = 2f;
    public float bgMargin = 0.5f;
    public Camera targetCamera;

    [Header("=== Пары препятствий ===")]
    public GameObject obstaclePrefab;
    public float obstacleWidthOverride = 0f;

    [Header("=== Скалирование сложности ===")]
    [Range(0f, 1f)] public float startDifficultyPercent = 0f;
    [Range(0f, 1f)] public float endDifficultyPercent = 1f;
    [Tooltip("Порог очков для победы")]
    public int pipesToMaxDifficulty = 50;

    [Header("Диапазоны параметров сложности")]
    public Vector2 gapRange = new Vector2(6f, 3f);
    public Vector2 distRange = new Vector2(8f, 4f);
    public Vector2 deltaRange = new Vector2(0f, 2f);

    [Header("=== Игрок для X-позиции ===")]
    public Transform playerTransform;

    [Header("=== Счёт и UI ===")]
    public UnityEvent<int> onScoreChanged;
    public TMP_Text scoreText;

    [Header("=== Менеджер UI ===")]
    public UIManager uiManager;

    float halfCamW, halfCamH, tileWidth, obstacleWidth;
    List<Transform> bgTiles = new List<Transform>();
    List<Transform> obstacles = new List<Transform>();
    List<Transform> scoringPipes = new List<Transform>();
    float lastCenterY = 0f;
    int score = 0;
    bool isPlaying = false;
    Coroutine spawnRoutine;

    void Start()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        halfCamH = targetCamera.orthographicSize;
        halfCamW = halfCamH * targetCamera.aspect;

        // размер одного тайла
        var bgSR = backgroundPrefab.GetComponent<SpriteRenderer>();
        tileWidth = bgSR.bounds.size.x;

        float camX = targetCamera.transform.position.x;
        float camY = targetCamera.transform.position.y;
        int dir = (int)scrollDirection;  // -1 = Left, +1 = Right

        // ширина области + двойной margin
        float viewWidth = 2f * halfCamW + 2f * bgMargin;
        // добавляем +3 тайла "на запас"
        int count = Mathf.CeilToInt(viewWidth / tileWidth) + 3;

        // стартовая позиция первого тайла:
        // от противоположного края (camX - dir*(halfCamW+bgMargin))
        // сдвигаем на 1.5 тайла в сторону появления
        float startEdgeX = camX
            - dir * (halfCamW + bgMargin)
            - (-dir) * (tileWidth * 1.5f);

        for (int i = 0; i < count; i++)
        {
            // каждый следующий шагаем на tileWidth в сторону появления новых (-dir)
            float x = startEdgeX + (-dir) * (i * tileWidth);
            var go = Instantiate(backgroundPrefab,
                                 new Vector3(x, camY, 0f),
                                 Quaternion.identity,
                                 transform);
            bgTiles.Add(go.transform);
        }

        obstacleWidth = obstacleWidthOverride > 0f
                      ? obstacleWidthOverride
                      : obstaclePrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        // Передаём направление птице
        var flappy = playerTransform.GetComponent<FlappyController>();
        if (flappy != null)
            flappy.SetScrollDirection(scrollDirection);

        UpdateScoreUI();
    }




    public void StartGame()
    {
        if (isPlaying) return;
        isPlaying = true;
        spawnRoutine = StartCoroutine(SpawnObstaclePairs());
    }

    IEnumerator SpawnObstaclePairs()
    {
        lastCenterY = 0f;
        while (isPlaying)
        {
            float t = pipesToMaxDifficulty > 0
                ? Mathf.Clamp01((float)score / pipesToMaxDifficulty)
                : 1f;
            float difficulty = Mathf.Lerp(startDifficultyPercent, endDifficultyPercent, t);

            float gapSize = Mathf.Lerp(gapRange.x, gapRange.y, difficulty);
            float hDist = Mathf.Lerp(distRange.x, distRange.y, difficulty);
            float maxDY = Mathf.Lerp(deltaRange.x, deltaRange.y, difficulty);

            SpawnPair(gapSize, maxDY);
            yield return new WaitForSeconds(hDist / scrollSpeed);
        }
    }

    void SpawnPair(float gapSize, float maxDY)
    {
        float minC = -halfCamH + gapSize * 0.5f;
        float maxC = halfCamH - gapSize * 0.5f;
        float rawY = Random.Range(minC, maxC);
        float centerY = Mathf.Clamp(rawY, lastCenterY - maxDY, lastCenterY + maxDY);
        lastCenterY = centerY;

        int dir = (int)scrollDirection;
        // трубы появляются с противоположной стороны движения
        float spawnX = targetCamera.transform.position.x - dir * (halfCamW + obstacleWidth * 0.5f);

        // Верхняя труба
        var topGO = Instantiate(obstaclePrefab,
            new Vector3(spawnX, centerY + gapSize * 0.5f, 0f),
            Quaternion.Euler(0, 0, dir < 0 ? 180f : 0f),
            transform);
        AdjustObstacle(topGO.transform, true);
        obstacles.Add(topGO.transform);
        scoringPipes.Add(topGO.transform);

        // Нижняя труба
        var botGO = Instantiate(obstaclePrefab,
            new Vector3(spawnX, centerY - gapSize * 0.5f, 0f),
            Quaternion.identity,
            transform);
        AdjustObstacle(botGO.transform, false);
        obstacles.Add(botGO.transform);
    }

    void Update()
    {
        if (!isPlaying) return;

        int dir = (int)scrollDirection;
        float dx = dir * scrollSpeed * Time.deltaTime;
        Vector3 camPos = targetCamera.transform.position;

        // Прокрутка фоновых тайлов
        foreach (var t in bgTiles)
            t.position += Vector3.right * dx;

        // Ре-юз фоновых тайлов
        float edge = camPos.x + dir * (halfCamW + bgMargin);
        var first = bgTiles[0];
        bool outOfView = dir < 0
            ? first.position.x + tileWidth * 0.5f < edge
            : first.position.x - tileWidth * 0.5f > edge;
        if (outOfView)
        {
            var last = bgTiles[bgTiles.Count - 1];
            // сдвигаем первый на другой край, противоположный движению
            first.position = last.position + Vector3.right * (-dir) * tileWidth;
            bgTiles.RemoveAt(0);
            bgTiles.Add(first);
        }

        // Движение и удаление труб
        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            var o = obstacles[i];
            o.position += Vector3.right * dx;
            bool remove = dir < 0
                ? o.position.x + obstacleWidth * 0.5f < camPos.x - halfCamW
                : o.position.x - obstacleWidth * 0.5f > camPos.x + halfCamW;
            if (remove)
            {
                Destroy(o.gameObject);
                obstacles.RemoveAt(i);
            }
        }

        // Подсчёт очков и проверка победы
        for (int i = scoringPipes.Count - 1; i >= 0; i--)
        {
            var pipe = scoringPipes[i];
            bool passed = dir < 0
                ? pipe.position.x < playerTransform.position.x
                : pipe.position.x > playerTransform.position.x;
            if (passed)
            {
                score++;
                onScoreChanged?.Invoke(score);
                UpdateScoreUI();
                scoringPipes.RemoveAt(i);
                if (score >= pipesToMaxDifficulty)
                {
                    Win();
                    return;
                }
            }
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    public void Lose()
    {
        if (!isPlaying) return;
        isPlaying = false;
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        uiManager.ShowGameOver();
    }

    void Win()
    {
        isPlaying = false;
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        uiManager.ShowVictory();
    }

    void AdjustObstacle(Transform obs, bool isTop)
    {
        float camTop = targetCamera.transform.position.y + halfCamH;
        float camBottom = targetCamera.transform.position.y - halfCamH;
        var sr = obs.GetComponent<SpriteRenderer>();
        float origH = sr.sprite.bounds.size.y;

        float desiredH = isTop
            ? camTop - obs.position.y
            : obs.position.y - camBottom;
        float scaleY = desiredH / origH;
        obs.localScale = new Vector3(obs.localScale.x, scaleY, 1f);

        float halfNewH = origH * scaleY * 0.5f;
        float newY = isTop
            ? camTop - halfNewH
            : camBottom + halfNewH;
        obs.position = new Vector3(obs.position.x, newY, obs.position.z);
    }
}
