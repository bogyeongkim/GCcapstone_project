using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageAble
{
    public SoundAnalyzer3 SoundAnalyzer3; // SoundAnalyzer 스크립트 연결
    private UIManager uiManager; // UIManager 연결

    public float maxJumpPower = 20f; // 최대 점프 파워
    public float minJumpThreshold = 20f; // 최소 소리 임계치
    public float groundCheckDistance = 0.1f; // 바닥 체크 거리
    // public int health; // 플레이어 체력
    public LayerMask groundLayer; // 바닥 레이어
    private Vector3 originalPosition; // 시작했을때의 위치

    private Rigidbody2D rb;
    private PlayerAnimation playerAnim; // PlayerAnimation 스크립트 연결
    
    public bool isGrounded; // 바닥에 닿았는지 여부를 나타내는 플래그
    public bool isCanMove; // 움직일 수 있는지 체크

    
    private void Start()
    {
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
        if (GameManager.Instance.isGameEnd) return;
        
        // 바닥 체크
        CheckGrounded();
        
        List<float> dbValues = SoundAnalyzer3.GetDbValues();

        if (dbValues.Count > 0)
        {
            float latestDb = dbValues[^1]; // 가장 최근의 소리 데시벨 값

            if (latestDb > minJumpThreshold && isGrounded)
            {
                Jump(latestDb);
            }
        }

        playerAnim.SetBool(AnimParam.isRun, isCanMove);
        playerAnim.SetBool(AnimParam.isJump, !isGrounded);
    }

    private void Jump(float db)
    {
        // 최대 점프 파워와 소리 데시벨에 비례하여 점프 파워 계산
        // 현재 maxJumpPower는 20, 소리 데시벨 값이 최대 점프 파워에 비례해 결정
        float jumpPower = maxJumpPower * ((db / 100f));
        // rb는 Rigidbody2D 컴포넌트, rb.velocity는 객체의 현재 속도 벡터
        // 새로운 속도 벡터를 설정, 위로 점프하게 함. x축 속도 유지, y축 속도만 변경
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
        
        // health -= value;
        // uiManager.SetHpImage(health);
        // if (health > 0)
        //     return;

        // uiManager.OnGameOver();
    }
}
