using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class TMP_Disable_Rendering : MonoBehaviour
{
    void Start()
    {
        // �� ��ũ��Ʈ�� �پ��ִ� ���� ������Ʈ�� Canvas ������Ʈ�� ã�Ƽ�
        TextMeshProUGUI tmp = GetComponent<TextMeshProUGUI>();

        // Canvas ������Ʈ�� ��Ȱ��ȭ�մϴ�.
        if (tmp != null)
        {
            tmp.enabled = false;
        }
    }
}
