using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

public class ExitGame : MonoBehaviour
{
    // �� �Լ��� ��ư�� Ŭ���� �� ȣ��˴ϴ�.
    public void QuitGame()
    {
        // �����Ϳ��� ���� ���� ���
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // ���� ���忡���� ���ø����̼��� �����մϴ�.
        Application.Quit();
        #endif
    }
}
