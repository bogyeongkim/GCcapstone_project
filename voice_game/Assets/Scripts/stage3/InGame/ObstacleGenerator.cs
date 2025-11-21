using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacleGenerator : MonoBehaviour
{
    public Vector3 spawnPosition;

    public int maxGenCount;
    public float reGenTime;

    private void Start()
    {
        StartCoroutine(SpawnObstacle());
    }

    private IEnumerator SpawnObstacle()
    {
        for (int i = 0; i < maxGenCount; i++)
        {
            // 리젠 시간이 될 때마다 높이만 랜덤으로 새로운 장애물 생성
            GameObject newObstacle = PoolManager.Instance.GetPool(0);
            newObstacle.transform.position = spawnPosition + new Vector3(0,Random.Range(-1.65f, 2.0f),0);

            yield return new WaitForSeconds(reGenTime);
        }
    }
}
