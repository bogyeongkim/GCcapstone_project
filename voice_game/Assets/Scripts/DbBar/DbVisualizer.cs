using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class DbVisualizer : MonoBehaviour
{
    public Transform dbBar; // 막대기의 Transform
    public float maxDbValue = 80f; // 데시벨의 최대값 (조정 가능)
    public float minHeight = 0.1f; // 막대기의 최소 높이
    public float scaleMultiplier = 0.1f; // 스케일 배율
    public UnityEngine.UI.Image barImage; // 막대기의 Image 컴포넌트

    void Start()
    {
        // 초기 막대기 높이를 설정 (최소 높이)
        dbBar.localScale = new Vector3(1, 0, 1);
    }

    public void UpdateDbVisualization(float dbValue)
    {
        // 데시벨 값에 비례하여 길이 조정
        float normalizedValue = Mathf.Clamp(dbValue, 0, maxDbValue) / maxDbValue; // 데시벨 값 정규화
        dbBar.localScale = new Vector3(1, normalizedValue * scaleMultiplier, 1); // 막대기 길이 조정

        Color newColor;

        if (dbValue < 45)
            newColor = Color.green;
        else if(dbValue < 60)
            newColor = Color.yellow;
        else
            newColor = Color.red;

        // 막대기 색상 적용
        if (barImage != null)
        {
            barImage.color = newColor;
        }

    }

    public void SetZero()
    {
        dbBar.localScale = new Vector3(1, 0, 1);
    }
}