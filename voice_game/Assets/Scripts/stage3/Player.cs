using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Player : MonoBehaviour, IDamageAble
{
    [Header("References")]
    public SoundAnalyzer3 soundAnalyzer;
    private Rigidbody2D rb;
    private PlayerAnimation playerAnim;
    private BoxCollider2D col;

    [Header("Jump Settings")]
    public float minJumpPower = 2f;
    public float maxJumpPower = 15f;

    [Header("Sound Settings")]
    public float minJumpThreshold = 20f;
    public float maxDbReference = 70f;
    public float peakCheckDuration = 0.2f; // [핵심] 0.1초 동안 소리 크기를 측정함 (지연 시간)

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.05f;

    public bool isGrounded { get; private set; }
    public bool isCanMove = true;

    private bool isJumpProcessing = false; // 현재 점프를 준비 중인지 체크

    private void Awake()
    {
        if (soundAnalyzer == null) soundAnalyzer = FindObjectOfType<SoundAnalyzer3>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        playerAnim = GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
        if (GameManager.Instance.isGameEnd)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            return;
        }

        CheckGrounded();
        HandleSoundInput();
        UpdateAnimation();
    }

    private void HandleSoundInput()
    {
        if (soundAnalyzer == null || isJumpProcessing || !isGrounded) return;

        List<float> dbValues = soundAnalyzer.GetDbValues();

        if (dbValues != null && dbValues.Count > 0)
        {
            // 현재 프레임의 소리
            float currentDb = dbValues[dbValues.Count - 1];

            // [수정] 임계치를 넘으면 바로 점프하지 않고, "힘 모으기" 코루틴 시작
            if (currentDb > minJumpThreshold)
            {
                StartCoroutine(ProcessJumpWithPeakCheck(currentDb));
            }
        }
    }

    // [핵심 기능] 0.1초 동안 들어온 소리 중 가장 큰 값을 찾아 점프
    IEnumerator ProcessJumpWithPeakCheck(float initialDb)
    {
        isJumpProcessing = true; // 중복 실행 방지
        float maxDbSeen = initialDb; // 현재까지 확인된 최대 소리
        float timer = 0f;

        // 설정한 시간(0.1초) 동안 대기하며 더 큰 소리가 들어오는지 감시
        while (timer < peakCheckDuration)
        {
            timer += Time.deltaTime;

            // 대기 중에도 계속 소리 값을 확인해서 최댓값 갱신
            List<float> dbs = soundAnalyzer.GetDbValues();
            if (dbs != null && dbs.Count > 0)
            {
                float current = dbs[dbs.Count - 1];
                if (current > maxDbSeen)
                {
                    maxDbSeen = current; // 더 큰 소리가 들어오면 갱신!
                }
            }
            yield return null;
        }

        // 0.1초 뒤, 확인된 '가장 큰 소리(maxDbSeen)'로 점프 실행
        Jump(maxDbSeen);

        // 점프 후 잠시 대기 (바닥 체크가 바로 true로 뜨는 것 방지용 0.1초)
        yield return new WaitForSeconds(0.1f);
        isJumpProcessing = false;
    }

    private void Jump(float db)
    {
        float t = Mathf.InverseLerp(minJumpThreshold, maxDbReference, db);
        float jumpForce = Mathf.Lerp(minJumpPower, maxJumpPower, t);

        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckGrounded()
    {
        // 점프 준비 중일 때는 바닥 체크를 무시하거나, 점프 직후 로직 꼬임 방지
        if (isJumpProcessing) return;

        Bounds bounds = col.bounds;
        RaycastHit2D hit = Physics2D.BoxCast(
            bounds.center, bounds.size, 0f, Vector2.down, groundCheckDistance, groundLayer
        );
        isGrounded = hit.collider != null;
    }

    private void UpdateAnimation()
    {
        playerAnim.SetBool(AnimParam.isRun, isCanMove);
        // 점프 준비 중이거나 공중에 떠 있으면 점프 모션
        playerAnim.SetBool(AnimParam.isJump, !isGrounded || isJumpProcessing);
    }

    public void Damage(int value)
    {
        playerAnim.SetTrigger(AnimParam.hurt);
        GameManager.Instance.score--;
    }
}