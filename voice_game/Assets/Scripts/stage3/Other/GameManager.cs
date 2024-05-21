using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool isGameEnd { get; set; }
    public int score { get; set; }

    public int obstacleCount;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        isGameEnd = false;
        score = 10;
    }

    public void PlusObstacleCount()
    {
        obstacleCount++;
        if (obstacleCount >= 10)
            isGameEnd = true;
    }
}
