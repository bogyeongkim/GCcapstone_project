using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Go_Next : MonoBehaviour
{
    string nextscene;

    void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "Title")
        {
            nextscene = "Menu";
        }
        else if (currentSceneName == "Menu")
        {
            nextscene = "intro";
        }
        else if (currentSceneName == "intro")
        {
            nextscene = "info_stage1";
        }
        else if (currentSceneName == "info_stage1")
        {
            nextscene = "stage1";
        }
        else if (currentSceneName == "stage1")
        {
            nextscene = "info_stage2";
        }
        else if (currentSceneName == "info_stage2")
        {
            nextscene = "stage2";
        }
        else if (currentSceneName == "stage2")
        {
            nextscene = "info_stage3";
        }
        else if (currentSceneName == "info_stage3")
        {
            nextscene = "stage3";
        }
        else if (currentSceneName == "stage3")
        {
            nextscene = "EndingScene";
        }
    }

    public void Load_Next()
    {
        SceneManager.LoadScene(nextscene);
        
    }
}
