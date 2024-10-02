using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Security.Cryptography;
using System.Collections.Specialized;

public class NPCMoveOnClick : MonoBehaviour
{

    private Transform player;
    private Animator playerAnimator;
    private GameObject player2;


    public float moveSpeed = 5f; // NPC 이동 속도
    private bool isMoving = false; // NPC 움직임 상태 체크
    private bool isNext = false; // 끝났는지 체크

    private SoundAnalyzer soundAnalyzer;

    List<float> DBL = new List<float>();

    private Vector3 targetPosition1;
    private Vector3 targetPosition2;
    private Vector3 setPosition;

    //텍스트 및 이미지 오브젝트
    public GameObject next_NPC;
    public GameObject microphone_icon;
    public GameObject fairy;
    public GameObject arrow02;
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    public GameObject next_button;

    
    public GameObject background;

    public GameObject star_1;
    public GameObject star_2;
    public GameObject star_3;
    public GameObject book_item;


    public TextMeshProUGUI fairy_text;
    public TextMeshProUGUI line;
    public TextMeshProUGUI response;
    public TextMeshProUGUI loud;
    public TextMeshProUGUI quiet;
    public TextMeshProUGUI good;
    public TextMeshProUGUI next_fairy_text;
    public TextMeshProUGUI SoundBar;

    public AudioSource a_fairy_text;
    public AudioSource a_response;
    public AudioSource a_loud;
    public AudioSource a_quiet;
    public AudioSource a_good;
    public AudioSource a_next_fairy_text;


    void Start()
    {
        FindPlayerObject();
        // SoundAnalyzer 컴포넌트 찾기
        soundAnalyzer = FindObjectOfType<SoundAnalyzer>();
        if (soundAnalyzer != null)
        {
            soundAnalyzer.enabled = true; // 컴포넌트를 활성화
        }
        if (soundAnalyzer == null)
        {
            UnityEngine.Debug.LogError("SoundAnalyzer 컴포넌트를 찾을 수 없습니다.");
        }
        //fairy_text.GetComponent<TextMeshProUGUI>().enabled = true;
        if (gameObject.name == "NPC_01")
            StartCoroutine(Fairy_NPC01());
        fairy.GetComponent<Renderer>().enabled = true;
        arrow02.GetComponent<Renderer>().enabled = true;
    }

    void FindPlayerObject()
    {
        // "Player" 이름의 오브젝트를 먼저 찾고, 없다면 "Player2" 이름의 오브젝트를 찾음
        GameObject playerObject = GameObject.Find("Player(Clone)") ?? GameObject.Find("Player2(Clone)");

        if (playerObject != null)
        {
            player2 = playerObject;
            player = playerObject.transform;

            // 만약 "Player" 오브젝트를 찾았다면, 하위의 "student" 오브젝트를 찾고 Animator를 가져옴
            if (playerObject.name == "Player(Clone)")
            {
                Transform studentTransform = player.Find("CollegeStudent");
                if (studentTransform != null)
                {
                    playerAnimator = studentTransform.GetComponent<Animator>();
                }
                else
                {
                    UnityEngine.Debug.LogError("Student object not found in the Player.");
                }
            }
            else // "Player2" 오브젝트인 경우, 해당 오브젝트의 Animator를 가져옴
            {
                playerAnimator = playerObject.GetComponent<Animator>();
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Player or Player2 object not found in the scene.");
        }
    }

    IEnumerator Fairy_NPC01()
    {
        yield return new WaitForSeconds(2);
        a_fairy_text.Play();
        A_click.a_click_bool = true;
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToPlayer();
        }

        if (isNext)
        {
            TriggerEvent_Next();
        }
    }

    // NPC 클릭시 호출되는 메소드
    private void OnMouseDown()
    {
        if (A_click.a_click_bool) 
        {
            A_click.a_click_bool = false;
            isMoving = true;
            UnityEngine.Debug.Log("click");
            arrow02.GetComponent<Renderer>().enabled = false;
            fairy_text.GetComponent<TextMeshProUGUI>().enabled = false;
            fairy.GetComponent<Renderer>().enabled = false;
        }
    }

    // 플레이어 위치 근처로 NPC 이동
    void MoveToPlayer()
    {
        float step = moveSpeed * Time.deltaTime;
        Vector3 targetPosition = new Vector3(-7.0f, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        // 플레이어 run
        playerAnimator.SetBool("isRun",true);

        if (Vector3.Distance(transform.position, targetPosition) < 2.5f)
        {
            isMoving = false;
            UnityEngine.Debug.Log("isRun???");
            playerAnimator.SetBool("isRun", false);
            // 이동 끝나면 대화 시작
            TriggerDialogue();
        }
    }

    void TriggerDialogue()
    {
        // "~책 어디있어요?" 화면에 텍스트 띄우기
        line.GetComponent<TextMeshProUGUI>().enabled = true;
        microphone_icon.GetComponent<Renderer>().enabled = true;
        // 마이크 입력 받고 데시벨 값 받아오기
        UnityEngine.Debug.Log("trigger");
        soundAnalyzer.StartRecording();
        StartCoroutine(WaitAndReceiveDbValues()); // 코루틴 시작
    }

    IEnumerator WaitAndReceiveDbValues()
    {
        bool isLoud = false;
        float sumDb = 0f;
        int countDb = 0;

        yield return new WaitForSeconds(5); // 5초 기다림

        if (!soundAnalyzer.isRecording) // 녹음 끝났는지 확인
        {
            line.GetComponent<TextMeshProUGUI>().enabled = false;
            microphone_icon.GetComponent<Renderer>().enabled = false;
            
            DBL = soundAnalyzer.GetDbValues();
            UnityEngine.Debug.Log("getdb");

            /* 데시벨 값에 따른 처리 필요
             * 목소리 클 때() : 도서관에서는 조용히 말해야해. & 답변 & 다음 단계
             * 목소리 작을 때() : 잘 안 들려. 다시 말해줄래? & 재시도
             * 목소리 적절할 때() : 답변 & 다음 단계
             */
            foreach (float dbValue in DBL)
            {
                if (dbValue > 50) // 50보다 큰 값이 있는 경우
                {
                    TriggerEvent_loud(); // 이벤트를 발생시킴
                    isLoud = true;
                    break; // 하나라도 조건을 만족하는 경우 추가 검사 없이 loop 종료
                }
                else if (dbValue > 0)
                {
                    sumDb += dbValue;
                    countDb++;
                }
            }
            
            if (countDb == 0) 
            {
                countDb = 1;
            }

            if (!isLoud) // 45를 초과하는 값이 없는 경우
            {
                float averageDb = sumDb / countDb; // 평균 데시벨 값 계산
                if (averageDb < 10) // 평균 데시벨 값이 10보다 작은 경우
                {
                    TriggerEvent_TooSoft();
                }
                else // 데시벨 값이 적절한 경우
                {
                    ScoreManager.instance.AddScore2(1,1);
                    int stagescore = ScoreManager.instance.GetStageScore(1);
                    if (stagescore == 1) 
                    {
                        star1.SetActive(true);
                    }
                    else if (stagescore == 2)
                    {
                        star2.SetActive(true);
                    }
                    else
                    {
                        star3.SetActive(true);
                    }
                    TriggerEvent_good();
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
        loud.GetComponent<TextMeshProUGUI>().enabled = true;
        a_loud.Play();
        fairy.GetComponent<Renderer>().enabled = true;
        StartCoroutine(WaitAndTriggerAppropriate(4.0f));// 3초 후 TriggerEvent_Appropriate 호출
    }

    void TriggerEvent_good()
    {
        good.GetComponent<TextMeshProUGUI>().enabled = true;
        a_good.Play();
        fairy.GetComponent<Renderer>().enabled = true;
        StartCoroutine(WaitAndTriggerAppropriate(4.0f));// 3초 후 TriggerEvent_Appropriate 호출
    }

    IEnumerator WaitAndTriggerAppropriate(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // waitTime만큼 대기
        good.GetComponent<TextMeshProUGUI>().enabled = false;
        loud.GetComponent<TextMeshProUGUI>().enabled = false;
        fairy.GetComponent<Renderer>().enabled = false;
        
        TriggerEvent_Appropriate(); // TriggerEvent_Appropriate 호출
    }

    void TriggerEvent_TooSoft()
    {
        quiet.GetComponent<TextMeshProUGUI>().enabled = true;
        a_quiet.Play();
        fairy.GetComponent<Renderer>().enabled = true;
        StartCoroutine(WaitAndTriggerRetry(5.0f));
    }

    IEnumerator WaitAndTriggerRetry(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // waitTime만큼 대기
        quiet.GetComponent<TextMeshProUGUI>().enabled = false;
        fairy.GetComponent<Renderer>().enabled = false;
        TriggerDialogue();
    }

    void TriggerEvent_Appropriate()
    {
        response.GetComponent<TextMeshProUGUI>().enabled = true;
        a_response.Play();
        
        int stagescore = ScoreManager.instance.GetStageScore(1);
        UnityEngine.Debug.Log("현재 점수 : " + stagescore);
        StartCoroutine(WaitAndTriggerNext(4.0f));
    }

    IEnumerator WaitAndTriggerNext(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // waitTime만큼 대기
        response.GetComponent<TextMeshProUGUI>().enabled = false;
        //next_NPC.SetActive(true);
        targetPosition1 = new Vector3(transform.position.x - 8.0f, transform.position.y, transform.position.z);
        targetPosition2 = new Vector3(next_NPC.transform.position.x - 8.0f, next_NPC.transform.position.y, next_NPC.transform.position.z);
        isNext = true;
    }

    void TriggerEvent_Next()
    {
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition1, step);
        next_NPC.transform.position = Vector3.MoveTowards(next_NPC.transform.position, targetPosition2, step);

        // 플레이어 run
        playerAnimator.SetBool("isRun", true);

        // NPC 이동 확인
        if (Vector3.Distance(transform.position, targetPosition1) < 0.01f)
        {
            isNext = false;
            playerAnimator.SetBool("isRun", false);
            UnityEngine.Debug.Log("Next!");
            next_fairy_text.GetComponent<TextMeshProUGUI>().enabled = true;
            a_next_fairy_text.Play();
            A_click.a_click_bool = true;

            fairy.GetComponent<Renderer>().enabled = true;

            if (gameObject.name == "NPC_03")
            {
                //fairy.GetComponent<Renderer>().enabled = true;
                int stagescore = ScoreManager.instance.GetStageScore(1);
                UnityEngine.Debug.Log("스테이지1 총 점수 : "+ stagescore);
                player2.SetActive(false);
                background.GetComponent<Renderer>().enabled = true;
                book_item.GetComponent<Renderer>().enabled = true;
                ScoreManager.instance.AddItem(book_item);
                book_item.transform.position = new Vector3(0f, -3f, -3f);
                StartCoroutine(Next_Button(a_next_fairy_text.clip.length + 1.5f));

                SoundBar.GetComponent<TextMeshProUGUI>().enabled = false;

                if (stagescore == 1)
                {
                    star_2.SetActive(true);
                }
                else if (stagescore == 2)
                {
                    star_1.SetActive(true);
                    star_3.SetActive(true);
                }
                else if (stagescore == 3)
                {
                    star_1.SetActive(true);
                    star_2.SetActive(true);
                    star_3.SetActive(true);
                }

            }
            else { arrow02.GetComponent<Renderer>().enabled = true; }
            
        }
    }
    IEnumerator Next_Button(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        next_button.SetActive(true);
    }
}