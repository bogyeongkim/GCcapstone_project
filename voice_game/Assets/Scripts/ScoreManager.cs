using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; 
    
    private int totalScore = 0; // 총 점수

    private List<GameObject> collectedItems = new List<GameObject>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
            // 씬 바뀌어도 오브젝트 유지, 초반 한번만 오브젝트 생성&할당, 이후 모든 스테이지에서 접근 가능
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int score)
    {
        totalScore += score;
    }

    public int GetTotalScore()
    {
        return totalScore;
    }

    // 게임 결과 처리
    public void CalculateResult()
    {
        // 총 점수에 따른 결과 처리 구현, 엔딩씬에 불러와서 처리
        if (totalScore >= 10)
        {
            UnityEngine.Debug.Log("");
        }
        else
        {
            UnityEngine.Debug.Log("");
        }
    }

    // 아이템 저장
    public void AddItem(GameObject item)
    {
        collectedItems.Add(item);
    }

    // 아이템 불러오기
    public List<GameObject> GetCollectedItems()
    {
        return collectedItems;
    }
}
