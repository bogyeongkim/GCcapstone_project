using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canvas_Disable_Rendering : MonoBehaviour
{
    void Start()
    {
        // 이 스크립트가 붙어있는 게임 오브젝트의 Canvas 컴포넌트를 찾아서
        Canvas canvas = GetComponent<Canvas>();

        // Canvas 컴포넌트를 비활성화합니다.
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }
}
