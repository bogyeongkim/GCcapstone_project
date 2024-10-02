using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Items : MonoBehaviour
{
    private Vector3 offScreenPosition = new Vector3(20, 20, 20);

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        // �� ��ȯ �� ��ġ �̵�
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �� �ε� �� ������Ʈ�� ȭ�� ������ �̵�
        transform.position = offScreenPosition;
    }
}
