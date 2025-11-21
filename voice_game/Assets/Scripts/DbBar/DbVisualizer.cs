using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.SceneManagement;
using TMPro;

public class DbVisualizer : MonoBehaviour
{
    public Transform dbBar; // ������� Transform
    public float maxDbValue = 80f; // ���ú��� �ִ밪 (���� ����)
    public float minHeight = 0.1f; // ������� �ּ� ����
    public float scaleMultiplier = 0.1f; // ������ ����
    public UnityEngine.UI.Image barImage; // ������� Image ������Ʈ
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

        // ����� ���� ����
        dbBar.localScale = new Vector3(1, smoothScaleY, 1);

        uiText.text = Mathf.FloorToInt(dbValue).ToString();

        Color newColor;

        if (currentSceneName == "stage1")
        {
            if (dbValue < 30)
                newColor = new Color32(188, 229, 92, 255);
            else if (dbValue < 40)
                newColor = new Color32(250, 237, 125, 255); //���
            else
                newColor = new Color32(222, 79, 79, 255);
        }
        else if (currentSceneName == "stage2")
        {
            if (dbValue < 20)
                newColor = new Color32(189,189,189, 255); //ȸ��
            else if (dbValue < 50)
                newColor = new Color32(188, 229, 92, 255); //����
            else if (dbValue < 65)
                newColor = new Color32(250, 237, 125, 255);
            else
                newColor = new Color32(222, 79, 79, 255);
        }
        else
        {
            newColor = new Color32(188, 229, 92, 255); //����
        }




        // ����� ���� ����
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