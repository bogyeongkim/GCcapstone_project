using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class DbVisualizer : MonoBehaviour
{
    public Transform dbBar; // ������� Transform
    public float maxDbValue = 80f; // ���ú��� �ִ밪 (���� ����)
    public float minHeight = 0.1f; // ������� �ּ� ����
    public float scaleMultiplier = 0.1f; // ������ ����
    public UnityEngine.UI.Image barImage; // ������� Image ������Ʈ

    void Start()
    {
        // �ʱ� ����� ���̸� ���� (�ּ� ����)
        dbBar.localScale = new Vector3(1, 0, 1);
    }

    public void UpdateDbVisualization(float dbValue)
    {
        // ���ú� ���� ����Ͽ� ���� ����
        float normalizedValue = Mathf.Clamp(dbValue, 0, maxDbValue) / maxDbValue; // ���ú� �� ����ȭ
        dbBar.localScale = new Vector3(1, normalizedValue * scaleMultiplier, 1); // ����� ���� ����

        Color newColor;

        if (dbValue < 45)
            newColor = Color.green;
        else if(dbValue < 60)
            newColor = Color.yellow;
        else
            newColor = Color.red;

        // ����� ���� ����
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