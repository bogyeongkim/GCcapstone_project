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


    public float moveSpeed = 5f; // NPC �̵� �ӵ�
    private bool isMoving = false; // NPC ������ ���� üũ
    private bool isNext = false; // �������� üũ

    private SoundAnalyzer soundAnalyzer;

    List<float> DBL = new List<float>();

    private Vector3 targetPosition1;
    private Vector3 targetPosition2;
    private Vector3 targetPosition3;
    private Vector3 setPosition;

    //�ؽ�Ʈ �� �̹��� ������Ʈ
    public GameObject next_NPC;
    public GameObject microphone_icon;
    public GameObject fairy;
    public GameObject arrow02;
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;
    public GameObject next_button;
    public GameObject bgi;

    public GameObject background;

    public GameObject star_1;
    public GameObject star_2;
    public GameObject star_3;
    public GameObject book_item;
    public GameObject bar;

    public TextMeshProUGUI fairy_text;
    public TextMeshProUGUI line;
    public TextMeshProUGUI response;
    public TextMeshProUGUI loud;
    public TextMeshProUGUI quiet;
    public TextMeshProUGUI good;
    public TextMeshProUGUI next_fairy_text;
    public TextMeshProUGUI SoundBar;
    public TextMeshProUGUI value;

    public AudioSource a_fairy_text;
    public AudioSource a_response;
    public AudioSource a_loud;
    public AudioSource a_quiet;
    public AudioSource a_good;
    public AudioSource a_next_fairy_text;


    void Start()
    {
        FindPlayerObject();
        // SoundAnalyzer ������Ʈ ã��
        soundAnalyzer = FindObjectOfType<SoundAnalyzer>();
        if (soundAnalyzer != null)
        {
            soundAnalyzer.enabled = true; // ������Ʈ�� Ȱ��ȭ
        }
        if (soundAnalyzer == null)
        {
            UnityEngine.Debug.LogError("SoundAnalyzer ������Ʈ�� ã�� �� �����ϴ�.");
        }
        //fairy_text.GetComponent<TextMeshProUGUI>().enabled = true;
        if (gameObject.name == "NPC_01")
            StartCoroutine(Fairy_NPC01());
        fairy.GetComponent<Renderer>().enabled = true;
        arrow02.GetComponent<Renderer>().enabled = true;
    }

    void FindPlayerObject()
    {
        // "Player" �̸��� ������Ʈ�� ���� ã��, ���ٸ� "Player2" �̸��� ������Ʈ�� ã��
        GameObject playerObject = GameObject.Find("Player(Clone)") ?? GameObject.Find("Player2(Clone)");

        if (playerObject != null)
        {
            player2 = playerObject;
            player = playerObject.transform;

            // ���� "Player" ������Ʈ�� ã�Ҵٸ�, ������ "student" ������Ʈ�� ã�� Animator�� ������
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
            else // "Player2" ������Ʈ�� ���, �ش� ������Ʈ�� Animator�� ������
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

    // NPC Ŭ���� ȣ��Ǵ� �޼ҵ�
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

    // �÷��̾� ��ġ ��ó�� NPC �̵�
    void MoveToPlayer()
    {
        /////////////////////////////////////////////////////////////
        float step = moveSpeed * Time.deltaTime;
        Vector3 targetPosition = new Vector3(-7.0f, transform.position.y, transform.position.z);
        Vector3 targetPosition4 = new Vector3(bgi.transform.position.x - 7.0f, bgi.transform.position.y, bgi.transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        bgi.transform.position = Vector3.MoveTowards(bgi.transform.position, targetPosition4, step);

        // �÷��̾� run
        playerAnimator.SetBool("isRun",true);

        if (Vector3.Distance(transform.position, targetPosition) < 2.5f)
        {
            isMoving = false;
            UnityEngine.Debug.Log("isRun???");
            playerAnimator.SetBool("isRun", false);
            // �̵� ������ ��ȭ ����
            TriggerDialogue();
        }
    }

    void TriggerDialogue()
    {
        line.GetComponent<TextMeshProUGUI>().enabled = true;
        microphone_icon.GetComponent<Renderer>().enabled = true;
        UnityEngine.Debug.Log("trigger");
        StartCoroutine(WaitAndReceiveDbValues());
    }

    IEnumerator WaitAndReceiveDbValues()
    {
        bool isLoud = false;
        float sumDb = 0f;
        int countDb = 0;

        // 1) 항상 새로운 녹음 시작
        soundAnalyzer.StartRecording();

        // 2) SoundAnalyzer에서 5초가 지나면 isRecording을 false로 바꿈
        //    그 때까지 계속 기다리기
        yield return new WaitUntil(() => !soundAnalyzer.isRecording);

        // 3) 여기 도달했다는 건 "이번 5초 녹음이 끝났다"는 뜻
        line.GetComponent<TextMeshProUGUI>().enabled = false;
        microphone_icon.GetComponent<Renderer>().enabled = false;

        DBL = soundAnalyzer.GetDbValues();
        UnityEngine.Debug.Log("getdb");

        foreach (float dbValue in DBL)
        {
            if (dbValue > 35)
            {
                UnityEngine.Debug.Log("Loud detected: " + dbValue + " dB");
                TriggerEvent_loud();
                isLoud = true;
                break;
            }
            else if (dbValue > 3)
            {
                sumDb += dbValue;
                countDb++;
            }
        }

        if (countDb == 0)
            countDb = 1;

        if (!isLoud)
        {
            float averageDb = sumDb / countDb;
            UnityEngine.Debug.Log("Average dB: " + averageDb + " dB");

            if (averageDb < 8)
            {
                // TooSoft → 조용하다는 안내 후, WaitAndTriggerRetry(5초)에서 TriggerDialogue 재호출
                TriggerEvent_TooSoft();
            }
            else
            {
                // 적절한 소리 크기 → 점수 / 별 처리 그대로
                ScoreManager.instance.AddScore2(1, 1);
                int stagescore = ScoreManager.instance.GetStageScore(1);
                if (stagescore == 1) star1.SetActive(true);
                else if (stagescore == 2) star2.SetActive(true);
                else star3.SetActive(true);

                TriggerEvent_good();
            }
        }
    }

    void TriggerEvent_loud()
    {
        loud.GetComponent<TextMeshProUGUI>().enabled = true;
        a_loud.Play();
        fairy.GetComponent<Renderer>().enabled = true;
        StartCoroutine(WaitAndTriggerAppropriate(4.0f));// 3�� �� TriggerEvent_Appropriate ȣ��
    }

    void TriggerEvent_good()
    {
        good.GetComponent<TextMeshProUGUI>().enabled = true;
        a_good.Play();
        fairy.GetComponent<Renderer>().enabled = true;
        StartCoroutine(WaitAndTriggerAppropriate(4.0f));// 3�� �� TriggerEvent_Appropriate ȣ��
    }

    IEnumerator WaitAndTriggerAppropriate(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // waitTime��ŭ ���
        good.GetComponent<TextMeshProUGUI>().enabled = false;
        loud.GetComponent<TextMeshProUGUI>().enabled = false;
        fairy.GetComponent<Renderer>().enabled = false;
        
        TriggerEvent_Appropriate(); // TriggerEvent_Appropriate ȣ��
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
        yield return new WaitForSeconds(waitTime); // waitTime��ŭ ���
        quiet.GetComponent<TextMeshProUGUI>().enabled = false;
        fairy.GetComponent<Renderer>().enabled = false;
        TriggerDialogue();
    }

    void TriggerEvent_Appropriate()
    {
        response.GetComponent<TextMeshProUGUI>().enabled = true;
        a_response.Play();
        
        int stagescore = ScoreManager.instance.GetStageScore(1);
        UnityEngine.Debug.Log("���� ���� : " + stagescore);
        StartCoroutine(WaitAndTriggerNext(4.0f));
    }

    IEnumerator WaitAndTriggerNext(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // waitTime��ŭ ���
        response.GetComponent<TextMeshProUGUI>().enabled = false;
        //next_NPC.SetActive(true);
        targetPosition1 = new Vector3(transform.position.x - 8.0f, transform.position.y, transform.position.z);
        targetPosition2 = new Vector3(next_NPC.transform.position.x - 8.0f, next_NPC.transform.position.y, next_NPC.transform.position.z);
        targetPosition3 = new Vector3(bgi.transform.position.x - 8.0f, bgi.transform.position.y, bgi.transform.position.z);
        isNext = true;
    }

    void TriggerEvent_Next()
    {
        /////////////////////////////////////////////////////////////
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition1, step);
        next_NPC.transform.position = Vector3.MoveTowards(next_NPC.transform.position, targetPosition2, step);
        bgi.transform.position = Vector3.MoveTowards(bgi.transform.position, targetPosition3, step);

        // �÷��̾� run
        playerAnimator.SetBool("isRun", true);

        // NPC �̵� Ȯ��
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
                UnityEngine.Debug.Log("��������1 �� ���� : "+ stagescore);
                player2.SetActive(false);
                background.GetComponent<Renderer>().enabled = true;
                book_item.GetComponent<Renderer>().enabled = true;
                ScoreManager.instance.AddItem(book_item);
                book_item.transform.position = new Vector3(0f, -3f, -3f);
                StartCoroutine(Next_Button(a_next_fairy_text.clip.length + 1.5f));

                SoundBar.GetComponent<TextMeshProUGUI>().enabled = false;
                value.GetComponent<TextMeshProUGUI>().enabled = false;
                bar.GetComponent < Image >().enabled = false;


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