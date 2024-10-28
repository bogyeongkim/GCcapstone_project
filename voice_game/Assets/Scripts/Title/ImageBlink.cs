using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public class ImageBlink : MonoBehaviour
{
    public float blinkSpeed = 1.0f; // 깜빡이는 속도 조절
    public float minAlpha = 0.0f;   // 최소 알파 값
    public float maxAlpha = 1.0f;   // 최대 알파 값
    public float delayTime = 2.0f;  // 오브젝트가 나타나기 전 지연 시간 (초)

    private SpriteRenderer spriteRenderer;
    private bool fadingOut = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 시작 시 오브젝트를 투명하게 설정
        Color color = spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;

        // 깜빡이는 효과 시작 코루틴 호출
        StartCoroutine(StartBlinkAfterDelay());
    }

    IEnumerator StartBlinkAfterDelay()
    {
        // 지정된 시간(delayTime)만큼 대기
        yield return new WaitForSeconds(delayTime);

        // 오브젝트를 완전히 나타나도록 설정
        //Color color = spriteRenderer.color;
        //color.a = maxAlpha;
        //spriteRenderer.color = color;

        // 이후 깜빡이기 시작
        while (true)
        {
            yield return null;
            UpdateBlink();
        }
    }

    void UpdateBlink()
    {
        Color color = spriteRenderer.color;

        // alpha 값을 감소 또는 증가시키며 깜빡임 처리
        if (fadingOut)
        {
            color.a -= blinkSpeed * Time.deltaTime;
            if (color.a <= minAlpha)
            {
                color.a = minAlpha;
                fadingOut = false;
            }
        }
        else
        {
            color.a += blinkSpeed * Time.deltaTime;
            if (color.a >= maxAlpha)
            {
                color.a = maxAlpha;
                fadingOut = true;
            }
        }

        spriteRenderer.color = color; // 변경된 color 값 적용
    }
}
