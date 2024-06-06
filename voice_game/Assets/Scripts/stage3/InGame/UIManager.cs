using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    public Text scoreText;

    public Transform hpImageParent;
    public List<GameObject> hpImageList = new List<GameObject>();

    public GameObject gameOverBg;

    private void Start()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        
    }

    private void Update()
    {
        scoreText.text = $"점수: {GameManager.Instance.score:#,##0}";

        if (!gameOverBg.activeSelf && GameManager.Instance.isGameEnd)
        {
            OnGameOver();
        }
    }

    /// <param name="curHp">현재체력</param>
    public void SetHpImage(int curHp)
    {
        int childCount = hpImageList.Count;

        if (childCount < curHp)
        {
            // 현재 체력만큼 이미지 추가
            for (int i = childCount; i < curHp; i++)
            {
                hpImageList.Add(PoolManager.Instance.GetPool(1, hpImageParent));
            }
        }

        else if (childCount > curHp)
        {
            // 현재 체력만큼 이미지 제거
            for (int i = childCount - 1; i >= curHp; i--)
            {
                GameObject temp = hpImageList[i];
                hpImageList.RemoveAt(i);
                temp.SetActive(false);
            }
        }
    }

    public void OnGameOver()
    {
        
    }
}
