using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ObstacleRandomizer : MonoBehaviour
{
    public Sprite[] sprites;
    void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
