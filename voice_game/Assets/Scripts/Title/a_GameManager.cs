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
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 게임 오브젝트가 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 중복된 인스턴스가 있다면 파괴
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
        // 에디터에서 실행 중일 경우
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // 실제 빌드에서는 애플리케이션을 종료합니다.
        Application.Quit();
        #endif
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        exitButton.gameObject.SetActive(false);
        resumeButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true);
    }

    void RestartGame()
    {
        SceneManager.LoadScene("Menu");
    }
}
