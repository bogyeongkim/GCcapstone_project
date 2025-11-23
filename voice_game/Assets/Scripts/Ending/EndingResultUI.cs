using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndingResultUI : MonoBehaviour
{
    [System.Serializable]
    public class StageRowUI
    {
        [Header("스테이지 이름 (도서관 / 카페 / 용의 골짜기)")]
        public TextMeshProUGUI stageNameText;

        [Header("별 표시 (★☆☆ / ★★☆ / ★★★)")]
        public TextMeshProUGUI starText;

        [Header("스테이지 결과 문구")]
        public TextMeshProUGUI commentText;
    }

    [Header("결과 패널 전체 오브젝트 (SetActive로 켜고 끔)")]
    public GameObject panelRoot;

    [Header("스테이지별 UI (0=도서관, 1=카페, 2=용의 골짜기)")]
    public StageRowUI[] stageRows = new StageRowUI[3];

    [Header("전체 총평 TMP 텍스트")]
    public TextMeshProUGUI totalCommentText;

    void Awake()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false); // 시작 시 숨김

        /*
        Dictionary<int, int> testScores = new Dictionary<int, int>();
        testScores[1] = 3;
        testScores[2] = 3;
        testScores[3] = 3;

        Show(testScores);
        */
    }

    /// <summary>
    /// stageScores: 1=도서관, 2=카페, 3=용의 골짜기
    /// </summary>
    public void Show(Dictionary<int, int> stageScores)
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);

        int totalScore = 0;

        // 1~3 스테이지 순서대로 매핑
        for (int stageKey = 1; stageKey <= 3; stageKey++)
        {
            int index = stageKey - 1;
            if (index >= stageRows.Length) continue;

            var row = stageRows[index];
            if (row == null) continue;

            // 점수 가져오기 (기본값 1)
            int score = 1;
            if (stageScores != null && stageScores.ContainsKey(stageKey))
                score = stageScores[stageKey];

            totalScore += score;

            // UI 채우기
            if (row.stageNameText != null)
                row.stageNameText.text = GetStageName(stageKey);

            if (row.starText != null)
                row.starText.text = GetStarString(score);

            if (row.commentText != null)
                row.commentText.text = GetStageComment(stageKey, score);
        }

        if (totalCommentText != null)
            totalCommentText.text = GetTotalComment(totalScore);
    }

    // ---------------------
    //  헬퍼 메서드들
    // ---------------------

    string GetStageName(int stageKey)
    {
        switch (stageKey)
        {
            case 1: return "[도서관]";
            case 2: return "[카페]";
            case 3: return "[용의 골짜기]";
        }
        return $"Stage {stageKey}";
    }

    string GetStarString(int score)
    {
        if (score == 3) return "★★★";
        if (score == 2) return "★★☆";
        return "★☆☆";
    }

    string GetStageComment(int stageKey, int score)
    {
        // 도서관
        if (stageKey == 1)
        {
            if (score == 3) return "도서관에서 속삭이듯 조용한 목소리로 \n잘 말했어!";
            if (score == 2) return "가끔 목소리가 살짝 커졌지만,\n조용히 말하려고 노력했어.";
            return "도서관에서는 조금 더 작은 목소리가 필요해.\n다음엔 속삭이듯 말해봐!";
        }

        // 카페
        if (stageKey == 2)
        {
            if (score == 3) return "카페에서 딱 좋은 목소리로 음료를 주문했어!";
            if (score == 2) return "조금 작거나 큰 순간도 있었지만 잘했어!";
            return "직원이 알아듣기 조금 어려웠을 수도 있어.\n다음에는 조금 더 또렷하게 말해보자.";
        }

        // 용의 골짜기
        if (stageKey == 3)
        {
            if (score == 3) return "목소리를 멋지게 조절했어!\n점프를 정말 잘 하는 걸?";
            if (score == 2) return "잘했어!\n조금 더 신경써서 목소리를 조절해볼까?";
            return "조금 더 신경써서 목소리를 내면,\n더 좋은 결과가 나올거야!";
        }

        return "";
    }

    string GetTotalComment(int totalScore)
    {
        if (totalScore >= 7)
            return "목소리 조절 마스터!\n어디서든 멋진 목소리 예절을 보여줄 수 있겠어!";

        if (totalScore >= 4)
            return "잘했어! 조금 더 연습하면 다음 모험 땐 더 잘할거야!";

        return "충분히 멋졌어! 오늘 연습을 기억했다가,\n다음 모험에서 한 번 더 도전해봐!";
    }

    public void ClosePanel()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    public void ShowCurrentResult()
    {
        // ScoreManager에서 현재까지의 스테이지 점수 가져오기
        var scores = ScoreManager.instance.GetAllStageScores();
        Show(scores);   // 우리가 이미 만든 Show(...) 재사용
    }
}