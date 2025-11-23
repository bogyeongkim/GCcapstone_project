using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DragonReaction : MonoBehaviour
{
    public GameObject reaction1; //1-3��
    public GameObject reaction2; //4-6��
    public GameObject reaction3; //7-9��

    public void React()
    {
        int totalScore = ScoreManager.instance.GetTotalScore();
        ShowReaction(totalScore);
    }

    void ShowReaction(int score)
    {
        if (score >= 1 && score <= 3)
        {
            reaction1.SetActive(true);
        }
        else if (score >= 4 && score <= 6)
        {
            reaction2.SetActive(true);
        }
        else if (score >= 7 && score <= 9)
        {
            reaction3.SetActive(true);
        }
        else
        {
            UnityEngine.Debug.Log("Invalid score range");
        }
    }
}
