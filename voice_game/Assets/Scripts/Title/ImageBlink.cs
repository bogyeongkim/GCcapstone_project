using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public class ImageBlink : MonoBehaviour
{
    public float blinkSpeed = 1.0f; // �����̴� �ӵ� ����
    public float minAlpha = 0.0f;   // �ּ� ���� ��
    public float maxAlpha = 1.0f;   // �ִ� ���� ��
    public float delayTime = 2.0f;  // ������Ʈ�� ��Ÿ���� �� ���� �ð� (��)

    private SpriteRenderer spriteRenderer;
    private bool fadingOut = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ���� �� ������Ʈ�� �����ϰ� ����
        Color color = spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;

        // �����̴� ȿ�� ���� �ڷ�ƾ ȣ��
        StartCoroutine(StartBlinkAfterDelay());
    }

    IEnumerator StartBlinkAfterDelay()
    {
        // ������ �ð�(delayTime)��ŭ ���
        yield return new WaitForSeconds(delayTime);

        // ������Ʈ�� ������ ��Ÿ������ ����
        //Color color = spriteRenderer.color;
        //color.a = maxAlpha;
        //spriteRenderer.color = color;

        // ���� �����̱� ����
        while (true)
        {
            yield return null;
            UpdateBlink();
        }
    }

    void UpdateBlink()
    {
        Color color = spriteRenderer.color;

        // alpha ���� ���� �Ǵ� ������Ű�� ������ ó��
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

        spriteRenderer.color = color; // ����� color �� ����
    }
}
