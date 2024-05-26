using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public class ExitGame : MonoBehaviour
{
    // 이 함수는 버튼이 클릭될 때 호출됩니다.
    public void QuitGame()
    {
        // 에디터에서 실행 중일 경우
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // 실제 빌드에서는 애플리케이션을 종료합니다.
        Application.Quit();
        #endif
    }
}
