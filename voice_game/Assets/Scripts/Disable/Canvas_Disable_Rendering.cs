using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canvas_Disable_Rendering : MonoBehaviour
{
    void Start()
    {
        // �� ��ũ��Ʈ�� �پ��ִ� ���� ������Ʈ�� Canvas ������Ʈ�� ã�Ƽ�
        Canvas canvas = GetComponent<Canvas>();

        // Canvas ������Ʈ�� ��Ȱ��ȭ�մϴ�.
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }
}
