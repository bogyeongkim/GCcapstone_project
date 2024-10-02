using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow2 : MonoBehaviour
{
    private Transform parentTransform;

    void Start()
    {
        // 부모 오브젝트의 Transform 가져오기
        parentTransform = transform.parent;
    }

    void Update()
    {
        // 부모의 위치를 따르되 Y값은 -0.7로 설정
        if (parentTransform != null)
        {
            Vector3 newPosition = parentTransform.position;
            newPosition.y -= 2.45f; 
            transform.position = newPosition;
        }
    }
}
