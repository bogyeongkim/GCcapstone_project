using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGame : MonoBehaviour
{
    // ---------------------
    // 1) 게임 종료
    // ---------------------
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }

    // ---------------------
    // 2) 게임 일시 정지
    // ---------------------
    public void PauseGame()
    {
        Time.timeScale = 0f;        // 게임 시간 정지
        Debug.Log("Game Paused");
    }

    // ---------------------
    // 3) 게임 재개
    // ---------------------
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Game Resumed");
    }

    // ---------------------
    // 4) 게임 완전히 처음부터 다시 시작
    // ---------------------
    public void RestartGame()
    {
        // TimeScale이 0 상태(일시정지)일 수 있으므로 원래대로 되돌림
        Time.timeScale = 1f;

        // 현재 빌드에서 첫 번째 씬(게임 시작 씬)을 다시 로드
        SceneManager.LoadScene(0);

        Debug.Log("Game Restarted (Scene 0 Reloaded)");
    }
}
