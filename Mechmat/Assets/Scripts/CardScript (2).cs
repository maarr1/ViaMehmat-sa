// CardScript.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardScript : MonoBehaviour, IPointerClickHandler
{
    public enum State { Closed, Opened, Matched }

    [HideInInspector] public int cardValue;
    [HideInInspector] public bool initialized = false;
    [HideInInspector] public Sprite faceSprite;
    [HideInInspector] public Sprite backSprite;
    [HideInInspector] public Color fallbackColor;
    [HideInInspector] public Sprite fallbackSprite;

    [Header("������������ Image � �������")]
    public Image cardImage;

    [HideInInspector] public State state = State.Closed;
    [HideInInspector] public float flipSpeed = 0.4f;

    /// <summary>
    /// �������������: ��� ����� ������� ��������.
    /// </summary>
    public void SetupGraphics()
    {
        cardImage.sprite = backSprite;
        cardImage.color = Color.white;
        state = State.Closed;
    }

    /// <summary>
    /// ������� �����: ��������� ���� ��� ���+��������.
    /// </summary>
    public void Reveal()
    {
        if (state != State.Closed) return;
        state = State.Opened;

        if (faceSprite != null)
        {
            cardImage.sprite = faceSprite;
            cardImage.color = Color.white;
        }
        else
        {
            cardImage.sprite = fallbackSprite;
            cardImage.color = fallbackColor;
        }
    }

    /// <summary>
    /// ������ ������� (��� ������������).
    /// </summary>
    public void Hide()
    {
        if (state != State.Opened) return;
        state = State.Closed;
        cardImage.sprite = backSprite;
        cardImage.color = Color.white;
    }

    /// <summary>
    /// ������ � ���� (��� ����������).
    /// </summary>
    public void SetMatched()
    {
        state = State.Matched;
        cardImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// ����� ���� � ������� � GameManager.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!initialized || state != State.Closed) return;
        GameManager2.Instance.OnCardClicked(this);
    }
}
