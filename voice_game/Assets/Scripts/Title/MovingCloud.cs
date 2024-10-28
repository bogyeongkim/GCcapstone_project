using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCloud : MonoBehaviour
{
    public float speed = 0.5f;
    public float startPositionX = -22f;
    public float endPositionX = 21f;

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        if (transform.position.x > endPositionX)
        {
            transform.position = new Vector2(startPositionX, transform.position.y);
        }
    }
}
