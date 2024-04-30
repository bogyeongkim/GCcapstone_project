using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class NPCMoveOnClick : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform, Inspector에서 할당
    public float moveSpeed = 5f; // NPC 이동 속도
    private bool isMoving = false; // NPC 움직임 상태 체크

    public Animator playerAnimator; // 플레이어 애니메이션 변경

    private SoundAnalyzer soundAnalyzer;

    List<float> DBL = new List<float>();

    public GameObject text01;
    public GameObject text02;
    public GameObject microphone_icon;
    public GameObject fairy;
    public GameObject loud01;
    public GameObject arrow02;


    void Start()
    {
        // SoundAnalyzer 컴포넌트 찾기
        soundAnalyzer = FindObjectOfType<SoundAnalyzer>();
        if (soundAnalyzer == null)
        {
            UnityEngine.Debug.LogError("SoundAnalyzer 컴포넌트를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToPlayer();
        }
    }

    // NPC 클릭시 호출되는 메소드
    private void OnMouseDown()
    {
        isMoving = true;
        UnityEngine.Debug.Log("click");
        arrow02.GetComponent<Renderer>().enabled = false;
        text01.GetComponent<Renderer>().enabled = false;
        fairy.GetComponent<Renderer>().enabled = false;
    }

    // 플레이어 위치 근처로 NPC 이동
    void MoveToPlayer()
    {
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.position, step);

        // 플레이어 run
        playerAnimator.SetBool("isRun",true);

        // NPC가 플레이어 근처에 도달했는지 확인
        if (Vector3.Distance(transform.position, player.position) < 3.0f)
        {
            isMoving = false;
            playerAnimator.SetBool("isRun", false);
            // 이동 끝나면 대화 시작
            TriggerDialogue();
        }
    }

    void TriggerDialogue()
    {
        // "~책 어디있어요?" 화면에 텍스트 띄우기
        text02.GetComponent<Renderer>().enabled = true;
        microphone_icon.GetComponent<Renderer>().enabled = true;
        // 마이크 입력 받고 데시벨 값 받아오기
        UnityEngine.Debug.Log("trigger");
        soundAnalyzer.StartRecording();
        StartCoroutine(WaitAndReceiveDbValues()); // 코루틴 시작
    }

    IEnumerator WaitAndReceiveDbValues()
    {
        yield return new WaitForSeconds(5); // 5초 기다림

        if (!soundAnalyzer.isRecording) // 녹음 끝났는지 확인
        {
            DBL = soundAnalyzer.GetDbValues();
            UnityEngine.Debug.Log("getdb");

            /* 데시벨 값에 따른 처리 필요
             * 목소리 클 때() : 도서관에서는 조용히 말해야해. & 답변 & 다음 단계
             * 목소리 작을 때() : 잘 안 들려. 다시 말해줄래? & 재시도
             * 목소리 적절할 때() : 답변 & 다음 단계
             */
            foreach (float dbValue in DBL)
            {
                if (dbValue > 40) // 40보다 큰 값이 있는 경우
                {
                    TriggerEvent_loud(); // 이벤트를 발생시킴
                    break; // 하나라도 조건을 만족하는 경우 추가 검사 없이 loop 종료
                }
            }
        }
        else
        {
            // 녹음 진행중 -> 녹음 종료
            UnityEngine.Debug.Log("Recording is still in progress.");
        }
    }

    void TriggerEvent_loud()
    {
        text02.GetComponent<Renderer>().enabled = false;
        microphone_icon.GetComponent<Renderer>().enabled = false;
        loud01.GetComponent<Renderer>().enabled = true;
    }
}