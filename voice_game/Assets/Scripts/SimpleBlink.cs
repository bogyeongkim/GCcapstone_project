using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBlink : MonoBehaviour
{
    [Header("설정")]
    public Color blinkColor = new Color(0.7f, 1f, 0f); // 쨍한 연두색 (기본값)
    public float speed = 12f; // 깜빡이는 속도

    private SpriteRenderer sprite;
    private Color originalColor;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        // 만약 UI 이미지라면 위 줄을 image = GetComponent<Image>(); 로 바꾸세요

        if (sprite != null)
            originalColor = sprite.color; // 원래 색 저장
    }

    void Update()
    {
        if (sprite == null) return;

        // 0~1 사이를 부드럽게 계속 왕복하는 값 (PingPong)
        float t = Mathf.PingPong(Time.time * speed, 1f);

        // 원래색과 설정한 색을 t값에 따라 부드럽게 섞음 (Lerp)
        sprite.color = Color.Lerp(originalColor, blinkColor, t);
    }

    // 오브젝트가 꺼질 때(비활성화) 색깔을 원래대로 돌려놓음 (선택사항)
    void OnDisable()
    {
        if (sprite != null) sprite.color = originalColor;
    }
}