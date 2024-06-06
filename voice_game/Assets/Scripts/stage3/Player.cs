using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IDamageAble
{
    public SoundAnalyzer3 SoundAnalyzer3; // SoundAnalyzer 스크립트 연결
    private UIManager uiManager; // UIManager 연결

    public float maxJumpPower = 20f; // 최대 점프 파워
    public float minJumpThreshold = 20f; // 최소 소리 임계치
    public float groundCheckDistance = 0.01f; // 바닥 체크 거리
    // public int health; // 플레이어 체력
    public LayerMask groundLayer; // 바닥 레이어
    private Vector3 originalPosition; // 시작했을때의 위치

    private Rigidbody2D rb;
    private PlayerAnimation playerAnim; // PlayerAnimation 스크립트 연결
    
    public bool isGrounded; // 바닥에 닿았는지 여부를 나타내는 플래그
    public bool isCanMove; // 움직일 수 있는지 체크

    private bool isGameEndProcessed = false; // 게임 종료 처리 여부

    private List<float> latestDbValues = new List<float>(); // 최근 소리 데시벨 값 저장
    
    private void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName.ToLower() == "stage1") // 대소문자 구분 없이 비교
        {
            UnityEngine.Debug.Log("end");
            this.enabled = false;
            return;
        }

        SoundAnalyzer3 = FindObjectOfType<SoundAnalyzer3>();
        uiManager = FindObjectOfType<UIManager>();
        
        playerAnim = GetComponent<PlayerAnimation>();
        rb = GetComponent<Rigidbody2D>();

        originalPosition = transform.position;

        Init();
    }

    private void Init()
    {
        // health = 3;
        // uiManager.SetHpImage(health);
    }

    private void Update()
    {
        if (GameManager.Instance.isGameEnd) 
        {
            if (!isGameEndProcessed)
            {
                Debug.Log("Game Ended. Stage 3 Score: " + GameManager.Instance.Stage3Score);
                isGameEndProcessed = true;
            }
            return;
        }

        // 바닥 체크
        CheckGrounded();

        List<float> dbValues = SoundAnalyzer3.GetDbValues();

        if (dbValues.Count > 4) // 리스트에 최소 5개의 값이 있어야 함
        {
            // dbValues 리스트에서 가장 최근의 소리 데시벨 값이 아니라, 최근 이후 4번째의 소리 데시벨 값을 targetDb로 선택
            float targetDb = dbValues[^5]; // ^5(끝에서 5번쨰) C# 8.0의 인덱스 문법 사용

            if (targetDb > minJumpThreshold && isGrounded)
            {
                Jump(targetDb);
            }
        }

        playerAnim.SetBool(AnimParam.isRun, isCanMove);
        playerAnim.SetBool(AnimParam.isJump, !isGrounded);
    }


    private void Jump(float db)
    {
        // 최대 점프 파워와 소리 데시벨에 비례하여 점프 파워 계산
        float jumpPower = maxJumpPower * ((db / 60f));
        rb.velocity = new Vector2(rb.velocity.x, jumpPower);
    }

    private void CheckGrounded()
    {
        // 바닥 체크를 위해 레이캐스트 사용
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);

        // 만약 레이캐스트가 땅을 감지하면 isGrounded를 true로 설정
        isGrounded = hit.transform != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }

    public void Damage(int value)
    {
        playerAnim.SetTrigger(AnimParam.hurt);

        GameManager.Instance.score--;
    }
}
