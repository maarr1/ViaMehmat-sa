using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float speed;
    private int direction = 1;
    // Start is called before the first frame update
    void Update()
    {
        transform.position += new Vector3(1, 0, 0) * Time.deltaTime * speed * direction;
    }

    public void SetDirection(int directionTo)
    {
        direction = directionTo;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
