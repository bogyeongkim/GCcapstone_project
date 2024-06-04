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

        /*
        if (currentSceneName == "intro")
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
        */
        if (currentSceneName == "stage1")
        {
            nextscene = "stage2";
        }
        else if (currentSceneName == "stage2")
        {
            nextscene = "EndingScene";
            UnityEngine.Debug.Log("goending");
        }
        else
        {
            UnityEngine.Debug.Log("No scene");
        }
    }

    public void Load_Next()
    {
        SceneManager.LoadScene(nextscene);
        
    }
}
