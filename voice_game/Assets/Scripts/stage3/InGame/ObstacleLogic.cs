using System;
using System.Collections;
using UnityEngine;

public class ObstacleLogic : MonoBehaviour
{
    public float moveSpeed;

    private bool isDamaged = false;
    
    private void OnEnable()
    {
        isDamaged = false;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.isGameEnd) return;
        
        transform.position += Time.fixedDeltaTime * moveSpeed * Vector3.left;

        if (transform.position.x <= -10)
        {
            this.gameObject.SetActive(false);
            GameManager.Instance.PlusObstacleCount();
        }
    }

    // 벽에 부딪히면 호출
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isDamaged)
            return;

        if (!other.TryGetComponent(out IDamageAble damage))
            return;

        isDamaged = true;
        damage.Damage(1);
    }
}
