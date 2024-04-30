using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRendering : MonoBehaviour
{
    void Start()
    {
        // �� ��ũ��Ʈ�� �پ��ִ� ���� ������Ʈ�� ������ ������Ʈ�� ã�Ƽ�
        Renderer renderer = GetComponent<Renderer>();

        // �������� ��Ȱ��ȭ�մϴ�.
        if (renderer != null)
        {
            renderer.enabled = false;
        }
    }
}
