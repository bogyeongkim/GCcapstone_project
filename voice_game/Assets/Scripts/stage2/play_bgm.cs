using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class play_bgm : MonoBehaviour
{
    public GameObject targetObject;  // 렌더링 상태를 확인할 대상 오브젝트
    private Renderer targetRenderer; // 대상 오브젝트의 Renderer
    private AudioSource audioSource; // 현재 오브젝트의 AudioSource

    void Start()
    {
        // 대상 오브젝트의 Renderer와 현재 오브젝트의 AudioSource를 가져옵니다.
        if (targetObject != null)
        {
            targetRenderer = targetObject.GetComponent<Renderer>();
        }
        else
        {
            UnityEngine.Debug.LogError("Target object is not assigned.");
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (targetRenderer != null)
        {
            // 대상 오브젝트가 렌더링되고 있는지 확인합니다.
            if (targetRenderer.isVisible)
            {
                // 대상 오브젝트가 렌더링되고 있다면 오디오를 중지합니다.
                if (audioSource.isPlaying)
                {
                    audioSource.Pause();
                }
            }
            else
            {
                // 대상 오브젝트가 렌더링되지 않고 있다면 오디오를 재생합니다.
                if (!audioSource.isPlaying)
                {
                    audioSource.UnPause();
                }
            }
        }
    }
}
