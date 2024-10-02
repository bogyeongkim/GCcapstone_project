using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class play_bgm : MonoBehaviour
{
    public GameObject targetObject;  // ������ ���¸� Ȯ���� ��� ������Ʈ
    private Renderer targetRenderer; // ��� ������Ʈ�� Renderer
    private AudioSource audioSource; // ���� ������Ʈ�� AudioSource

    void Start()
    {
        // ��� ������Ʈ�� Renderer�� ���� ������Ʈ�� AudioSource�� �����ɴϴ�.
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
            // ��� ������Ʈ�� �������ǰ� �ִ��� Ȯ���մϴ�.
            if (targetRenderer.isVisible)
            {
                // ��� ������Ʈ�� �������ǰ� �ִٸ� ������� �����մϴ�.
                if (audioSource.isPlaying)
                {
                    audioSource.Pause();
                }
            }
            else
            {
                // ��� ������Ʈ�� ���������� �ʰ� �ִٸ� ������� ����մϴ�.
                if (!audioSource.isPlaying)
                {
                    audioSource.UnPause();
                }
            }
        }
    }
}
