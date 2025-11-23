using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; 
    
    private int totalScore = 0; // �� ����
    private Dictionary<int, int> stageScores = new Dictionary<int, int>(); // ���������� ���� ����

    private List<GameObject> collectedItems = new List<GameObject>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
            // �� �ٲ� ������Ʈ ����, �ʹ� �ѹ��� ������Ʈ ����&�Ҵ�, ���� ��� ������������ ���� ����
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

    // ���� �߰� (������������)
    public void AddScore2(int stage, int score)
    {
        // �������� ���� �߰�
        totalScore += score;

        // ���������� ���� �߰� �Ǵ� ������Ʈ
        if (stageScores.ContainsKey(stage))
        {
            stageScores[stage] += score;
        }
        else
        {
            stageScores[stage] = score;
        }
    }

    public int GetTotalScore()
    {
        return totalScore;
    }

    // Ư�� �������� ���� ��ȯ
    public int GetStageScore(int stage)
    {
        if (stageScores.ContainsKey(stage))
        {
            return stageScores[stage];
        }
        else
        {
            return 0; // �������� ������ ���ٸ� 0 ��ȯ
        }
    }

    // ��� �������� ���� ��ȯ
    public Dictionary<int, int> GetAllStageScores()
    {
        return new Dictionary<int, int>(stageScores);
    }

    // ���� ��� ó��
    public void CalculateResult()
    {
        // �� ������ ���� ��� ó�� ����, �������� �ҷ��ͼ� ó��
        if (totalScore >= 10)
        {
            UnityEngine.Debug.Log("");
        }
        else
        {
            UnityEngine.Debug.Log("");
        }
    }

    // ������ ����
    public void AddItem(GameObject item)
    {
        collectedItems.Add(item);
    }

    // ������ �ҷ�����
    public List<GameObject> GetCollectedItems()
    {
        return collectedItems;
    }
}
