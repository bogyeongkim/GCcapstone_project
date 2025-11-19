using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class a_GameManager : MonoBehaviour
{
    public static a_GameManager instance;

    public Button pauseButton;
    public Button resumeButton;
    public Button exitButton;
    public Button restartButton;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ���� �ٲ� ���� ������Ʈ�� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ��� �ν��Ͻ��� �ִٸ� �ı�
        }
    }


    void Start()
    {
        exitButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        pauseButton.onClick.AddListener(PauseGame);
        exitButton.onClick.AddListener(ExitGame);
        resumeButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        exitButton.gameObject.SetActive(true);
        resumeButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

    void ExitGame()
    {
        // �����Ϳ��� ���� ���� ���
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // ���� ��忡���� ���ø����̼��� �����մϴ�.
        Application.Quit();
        #endif
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        exitButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }

    void RestartGame()
    {
        SceneManager.LoadScene("Title");
    }
}
