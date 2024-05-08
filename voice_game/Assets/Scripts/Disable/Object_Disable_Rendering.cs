using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Disable_Rendering : MonoBehaviour
{
    void Start()
    {
        // 이 스크립트가 붙어있는 게임 오브젝트의 렌더러 컴포넌트를 찾아서
        Renderer renderer = GetComponent<Renderer>();

        // 렌더러를 비활성화합니다.
        if (renderer != null)
        {
            renderer.enabled = false;
        }
    }
}
