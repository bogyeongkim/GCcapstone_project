using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    private Transform parentTransform;

    void Start()
    {
        // �θ� ������Ʈ�� Transform ��������
        parentTransform = transform.parent;
    }

    void Update()
    {
        // �θ��� ��ġ�� ������ Y���� -0.7�� ����
        if (parentTransform != null)
        {
            Vector3 newPosition = parentTransform.position;
            newPosition.y -= 2.3f; // Y���� -0.7�� ����
            newPosition.x += 0.3f; 
            transform.position = newPosition;
        }
    }
}
