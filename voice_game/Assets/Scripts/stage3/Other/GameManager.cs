using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool isGameEnd { get; set; }
    public int score { get; set; } // 소문자로 시작하는 변수명 사용

    public int obstacleCount;

    // stage3score를 저장할 변수 추가
    public int Stage3Score { get; private set; }

    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        isGameEnd = false;
        score = 10; // 시작 점수는 10점
        obstacleCount = 0; // 초기화 추가
        Stage3Score = 0; // 초기화 추가
    }

    public void PlusObstacleCount()
    {
        obstacleCount++;
        if (obstacleCount >= 10)
        {
            isGameEnd = true;
            CalculateStage3Score(); // 게임이 끝나면 stage3 점수를 계산
        }
    }

    // stage3score를 계산하는 메소드 추가
    private void CalculateStage3Score()
    {
        if (score >= 0 && score <= 1)
            Stage3Score = 0;
        else if (score >= 2 && score <= 4)
            Stage3Score = 1;
        else if (score >= 5 && score <= 7)
            Stage3Score = 2;
        else if (score >= 8 && score <= 10)
            Stage3Score = 3;
        /*
        if (Stage3Score == 1)
        {
            star1.SetActive(true);
        }
        if (Stage3Score == 2)
        {
            star2.SetActive(true);
        }
        if (Stage3Score == 3)
        {
            star3.SetActive(true);
        }*/

        ScoreManager.instance.AddScore(Stage3Score);
    }
}
