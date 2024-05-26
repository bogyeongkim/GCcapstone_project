using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DragonReaction : MonoBehaviour
{
    public GameObject reaction1; //1-3Á¡
    public GameObject reaction2; //4-6Á¡
    public GameObject reaction3; //7-9Á¡

    // Start is called before the first frame update
    void Start()
    {
        reaction1.SetActive(false);
        reaction2.SetActive(false);
        reaction3.SetActive(false);

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
