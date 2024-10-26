using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.SceneManagement;
using TMPro;

public class DbVisualizer : MonoBehaviour
{
    public Transform dbBar; // 막대기의 Transform
    public float maxDbValue = 80f; // 데시벨의 최대값 (조정 가능)
    public float minHeight = 0.1f; // 막대기의 최소 높이
    public float scaleMultiplier = 0.1f; // 스케일 배율
    public UnityEngine.UI.Image barImage; // 막대기의 Image 컴포넌트
    string currentSceneName;
    public TextMeshProUGUI uiText;

    void Start()
    {
        dbBar.localScale = new Vector3(1, 0, 1);
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    public void UpdateDbVisualization(float dbValue)
    {
        float normalizedValue = Mathf.Clamp(dbValue, 0, maxDbValue) / maxDbValue;
        float targetScaleY = normalizedValue * scaleMultiplier;
        float smoothScaleY = Mathf.Lerp(dbBar.localScale.y, targetScaleY, 0.05f);

        // 막대기 길이 조정
        dbBar.localScale = new Vector3(1, smoothScaleY, 1);

        uiText.text = Mathf.FloorToInt(dbValue).ToString();

        Color newColor;

        if (currentSceneName == "stage1")
        {
            if (dbValue < 30)
                newColor = new Color32(188, 229, 92, 255);
            else if (dbValue < 40)
                newColor = new Color32(250, 237, 125, 255); //노랑
            else
                newColor = new Color32(222, 79, 79, 255);
        }
        else if (currentSceneName == "stage2")
        {
            if (dbValue < 30)
                newColor = new Color32(189,189,189, 255); //회색
            else if (dbValue < 50)
                newColor = new Color32(188, 229, 92, 255); //연두
            else if (dbValue < 55)
                newColor = new Color32(250, 237, 125, 255);
            else
                newColor = new Color32(222, 79, 79, 255);
        }
        else
        {
            newColor = new Color32(188, 229, 92, 255); //연두
        }




        // 막대기 색상 적용
        if (barImage != null)
        {
            barImage.color = newColor;
        }

    }

    public void SetZero()
    {
        dbBar.localScale = new Vector3(1, 0, 1);
        uiText.text = "";

    }
}