using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCMoveOnClick : MonoBehaviour
{
    public Transform player; // �÷��̾��� Transform, Inspector���� �Ҵ�

    public float moveSpeed = 5f; // NPC �̵� �ӵ�
    private bool isMoving = false; // NPC ������ ���� üũ
    private bool isNext = false; // �������� üũ

    public Animator playerAnimator; // �÷��̾� �ִϸ��̼� ����

    private SoundAnalyzer soundAnalyzer;

    List<float> DBL = new List<float>();

    private Vector3 targetPosition1;
    private Vector3 targetPosition2;

    //�ؽ�Ʈ �� �̹��� ������Ʈ
    public GameObject next_NPC;
    public GameObject microphone_icon;
    public GameObject fairy;
    public GameObject arrow02;
    public GameObject star1;
    public GameObject star2;
    public GameObject star3;


    public TextMeshProUGUI fairy_text;
    public TextMeshProUGUI line;
    public TextMeshProUGUI response;
    public TextMeshProUGUI loud;
    public TextMeshProUGUI quiet;
    public TextMeshProUGUI next_fairy_text;


    void Start()
    {
        
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
        fairy.GetComponent<Renderer>().enabled = true;
        arrow02.GetComponent<Renderer>().enabled = true;
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
        isMoving = true;
        UnityEngine.Debug.Log("click");
        arrow02.GetComponent<Renderer>().enabled = false;
        fairy_text.GetComponent<TextMeshProUGUI>().enabled = false;
        fairy.GetComponent<Renderer>().enabled = false;
        
    }

    // �÷��̾� ��ġ ��ó�� NPC �̵�
    void MoveToPlayer()
    {
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.position, step);

        // �÷��̾� run
        playerAnimator.SetBool("isRun",true);

        // NPC�� �÷��̾� ��ó�� �����ߴ��� Ȯ��
        if (Vector3.Distance(transform.position, player.position) < 3.0f)
        {
            isMoving = false;
            playerAnimator.SetBool("isRun", false);
            // �̵� ������ ��ȭ ����
            TriggerDialogue();
        }
    }

    void TriggerDialogue()
    {
        // "~å ����־��?" ȭ�鿡 �ؽ�Ʈ ����
        line.GetComponent<TextMeshProUGUI>().enabled = true;
        microphone_icon.GetComponent<Renderer>().enabled = true;
        // ����ũ �Է� �ް� ���ú� �� �޾ƿ���
        UnityEngine.Debug.Log("trigger");
        soundAnalyzer.StartRecording();
        StartCoroutine(WaitAndReceiveDbValues()); // �ڷ�ƾ ����
    }

    IEnumerator WaitAndReceiveDbValues()
    {
        bool isLoud = false;
        float sumDb = 0f;
        int countDb = DBL.Count;

        yield return new WaitForSeconds(5); // 5�� ��ٸ�

        if (!soundAnalyzer.isRecording) // ���� �������� Ȯ��
        {
            line.GetComponent<TextMeshProUGUI>().enabled = false;
            microphone_icon.GetComponent<Renderer>().enabled = false;
            
            DBL = soundAnalyzer.GetDbValues();
            UnityEngine.Debug.Log("getdb");

            /* ���ú� ���� ���� ó�� �ʿ�
             * ��Ҹ� Ŭ ��() : ������������ ������ ���ؾ���. & �亯 & ���� �ܰ�
             * ��Ҹ� ���� ��() : �� �� ���. �ٽ� �����ٷ�? & ��õ�
             * ��Ҹ� ������ ��() : �亯 & ���� �ܰ�
             */
            foreach (float dbValue in DBL)
            {
                if (dbValue > 50) // 40���� ū ���� �ִ� ���
                {
                    TriggerEvent_loud(); // �̺�Ʈ�� �߻���Ŵ
                    isLoud = true;
                    break; // �ϳ��� ������ �����ϴ� ��� �߰� �˻� ���� loop ����
                }
                sumDb += dbValue;
            }

            if (!isLoud) // 40�� �ʰ��ϴ� ���� ���� ���
            {
                float averageDb = sumDb / DBL.Count; // ��� ���ú� �� ���
                if (averageDb < 5) // ��� ���ú� ���� 5���� ���� ���
                {
                    TriggerEvent_TooSoft();
                }
                else // ���ú� ���� ������ ���
                {
                    ScoreManager.instance.AddScore(1);
                    int stagescore = ScoreManager.instance.GetTotalScore();
                    if (stagescore == 1) 
                    {
                        star1.GetComponent<Renderer>().enabled = true;
                    }
                    else if (stagescore == 2)
                    {
                        star2.GetComponent<Renderer>().enabled = true;
                    }
                    else
                    {
                        star3.GetComponent<Renderer>().enabled = true;
                    }
                    TriggerEvent_Appropriate();
                }
            }
        }
        else
        {
            // ���� ������ -> ���� ����
            UnityEngine.Debug.Log("Recording is still in progress.");
        }
    }

    void TriggerEvent_loud()
    {
        loud.GetComponent<TextMeshProUGUI>().enabled = true;
        fairy.GetComponent<Renderer>().enabled = true;
        StartCoroutine(WaitAndTriggerAppropriate(4.0f));// 3�� �� TriggerEvent_Appropriate ȣ��
    }

    IEnumerator WaitAndTriggerAppropriate(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // waitTime��ŭ ���
        loud.GetComponent<TextMeshProUGUI>().enabled = false;
        fairy.GetComponent<Renderer>().enabled = false;
        
        TriggerEvent_Appropriate(); // TriggerEvent_Appropriate ȣ��
    }

    void TriggerEvent_TooSoft()
    {
        quiet.GetComponent<TextMeshProUGUI>().enabled = true;
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
        
        int stagescore = ScoreManager.instance.GetTotalScore();
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
        isNext = true;
    }

    void TriggerEvent_Next()
    {
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition1, step);
        next_NPC.transform.position = Vector3.MoveTowards(next_NPC.transform.position, targetPosition2, step);

        // �÷��̾� run
        playerAnimator.SetBool("isRun", true);

        // NPC �̵� Ȯ��
        if (Vector3.Distance(transform.position, targetPosition1) < 0.01f)
        {
            isNext = false;
            playerAnimator.SetBool("isRun", false);
            UnityEngine.Debug.Log("Next!");
            next_fairy_text.GetComponent<TextMeshProUGUI>().enabled = true;
            fairy.GetComponent<Renderer>().enabled = true;

            if (gameObject.name == "NPC03")
            {
                //end_text.GetComponent<TextMeshProUGUI>().enabled = true;
                //fairy.GetComponent<Renderer>().enabled = true;
                int stagescore = ScoreManager.instance.GetTotalScore();
                UnityEngine.Debug.Log("��������1 �� ���� : "+ stagescore);
            }
        }

        
    }
}