using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPC_staff : MonoBehaviour
{

    private SoundAnalyzer2 soundAnalyzer;

    List<float> DBL = new List<float>();

    private int turn = 0; // �ι��� ���ϱ�

    private bool isSoft1 = false;
    private bool isSoft2 = false;

    private int drink_num = 0;


    //������Ʈ
    public GameObject staff_bubble1;
    public GameObject staff_bubble2;
    public GameObject player_bubble;
    public GameObject staff_smile;
    public GameObject staff_sad;
    public GameObject fairy;
    public GameObject blank;
    public GameObject microphone;
    public GameObject blank_image;
    public GameObject customer;
    public GameObject customer_angry;
    public GameObject customer_bubble;
    public GameObject retry_button;
    public GameObject arrow;
    public GameObject next_button;

    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    public GameObject bar;

    public GameObject[] drinkObjects; // ���� ������Ʈ �迭
    public GameObject[] drinks;
    public TextMeshProUGUI[] drinks_t;

    //�ؽ�Ʈ
    public TextMeshProUGUI staff_line1; // �ֹ�
    public TextMeshProUGUI staff_line2; // �� ���̽�
    public TextMeshProUGUI staff_line3; // ��ø� ��ٷ��ּ���
    public TextMeshProUGUI staff_line4; // �ò������
    public TextMeshProUGUI line1; // ���ڿ���
    public TextMeshProUGUI line2; // ������ �ɷ�
    public TextMeshProUGUI fairy1;
    public TextMeshProUGUI fairy2;
    public TextMeshProUGUI fairy3;
    public TextMeshProUGUI wait; // ��ٸ��� ��
    public TextMeshProUGUI customer_line;
    public TextMeshProUGUI cafe_board;
    public TextMeshProUGUI retry;
    public TextMeshProUGUI staff_bad1;
    public TextMeshProUGUI staff_bad2;
    public TextMeshProUGUI Sound;
    public TextMeshProUGUI value;

    //�����
    public AudioSource staff_audio1; 
    public AudioSource staff_audio2;
    public AudioSource staff_audio3;
    public AudioSource staff_audio4;
    
    public AudioSource[] staff_audio_d;

    public AudioSource a_staff_bad1;
    public AudioSource a_staff_bad2;

    public AudioSource a_fairy1;
    public AudioSource a_fairy2;
    public AudioSource a_fairy3;

    public AudioSource a_customer;

    private bool isConversationRunning = false;



    void Start()
    {

        // SoundAnalyzer ������Ʈ ã��
        soundAnalyzer = FindObjectOfType<SoundAnalyzer2>();
        if (soundAnalyzer != null)
        {
            soundAnalyzer.enabled = true; // ������Ʈ�� Ȱ��ȭ
        }
        if (soundAnalyzer == null)
        {
            UnityEngine.Debug.LogError("SoundAnalyzer ������Ʈ�� ã�� �� �����ϴ�.");
        }

        
    }

    void Update()
    {

    }

    // 플레이어가 실제로 클릭했을 때만 호출되는 진입점
    private void OnMouseDown()
    {
        // 이미 대화/녹음이 진행 중이면 추가 클릭 무시
        if (isConversationRunning)
            return;

        isConversationRunning = true;
        StartConversationStep();
    }

    // 내부에서 “다음 턴으로 진행”할 때도 공통으로 쓰는 메서드
    private void StartConversationStep()
    {
        // 원래 OnMouseDown 안에 있던 로직을 그대로 옮김
        arrow.GetComponent<Renderer>().enabled = false;
        staff_bubble1.GetComponent<Renderer>().enabled = true;

        if (turn == 0)
            staff_line1.GetComponent<TextMeshProUGUI>().enabled = true;
        else
            staff_line2.GetComponent<TextMeshProUGUI>().enabled = true;

        if (turn == 0)
            staff_audio1.Play();
        else
            staff_audio2.Play();

        // 녹음 시작 코루틴은 그대로 유지
        StartCoroutine(StartRecordingAfterAudioEnds(staff_audio1.clip.length + 1.0f));
    }

    private System.Collections.IEnumerator StartRecordingAfterAudioEnds(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // ������� ���� ������ ��ٸ�
        if (soundAnalyzer != null)
        {
            player_bubble.GetComponent<Renderer>().enabled = true; // ��ǳ��
            if (turn == 0)
                line1.GetComponent<TextMeshProUGUI>().enabled = true; //�ؽ�Ʈ
            else
                line2.GetComponent<TextMeshProUGUI>().enabled = true;
            microphone.GetComponent<Renderer>().enabled = true;
            soundAnalyzer.StartRecording(); // ����ũ �Է� ����
            StartCoroutine(WaitAndReceiveDbValues()); // �ڷ�ƾ ����
        }
    }

    IEnumerator WaitAndReceiveDbValues()
    {
        turn++;

        float sumDb = 0f;
        int countDb = 0;

        yield return new WaitForSeconds(5); // 5�� ��ٸ�

        if (!soundAnalyzer.isRecording) // ���� �������� Ȯ��
        {
            soundAnalyzer.ClearWarning();

            staff_bubble1.GetComponent<Renderer>().enabled = false; // ��ǳ��
            staff_line1.GetComponent<TextMeshProUGUI>().enabled = false; //�ؽ�Ʈ
            staff_line2.GetComponent<TextMeshProUGUI>().enabled = false; //�ؽ�Ʈ
            player_bubble.GetComponent<Renderer>().enabled = false; // ��ǳ��
            line1.GetComponent<TextMeshProUGUI>().enabled = false; //�ؽ�Ʈ
            line2.GetComponent<TextMeshProUGUI>().enabled = false; //�ؽ�Ʈ
            microphone.GetComponent<Renderer>().enabled = false;

            DBL = soundAnalyzer.GetDbValues();
            UnityEngine.Debug.Log("getdb");

            foreach (float dbValue in DBL)
            {
                if (dbValue > 30f) // 0�� �ƴ� ���� ��쿡�� ó��
                {
                    sumDb += dbValue;
                    countDb++;
                }
            }

            float averageDb = 0f;
            if (countDb > 0)
            {
                averageDb = sumDb / countDb; // 0�� �ƴ� ���� ��� ���ú� �� ���
                UnityEngine.Debug.Log("average :"+averageDb);
            }
            if (averageDb < 45) 
            {
                if (turn == 1)
                {
                    isSoft1 = true;
                    StartConversationStep();
                }
                else if (turn == 2)
                {
                    isSoft2 = true;
                    TriggerEvent_Wait();
                }
                
            }
            else if (averageDb > 60) 
            {
                TriggerEvent_loud(); // turn �������
            }
            else // ���ú� ���� ������ ���
            {
                if (turn == 1)
                {
                    StartConversationStep();
                }
                else if (turn == 2)
                {
                    TriggerEvent_Wait();
                }
            }
        }
        else
        {
            // ���� ������ -> ���� ����
            UnityEngine.Debug.Log("Recording is still in progress.");
        }
    }

    void TriggerEvent_Wait()
    {
        staff_line3.GetComponent<TextMeshProUGUI>().enabled = true; // ��ø� ��ٷ��ּ���
        staff_audio3.Play();
        staff_bubble1.GetComponent<Renderer>().enabled = true; // ��ǳ��
        StartCoroutine(WaitAndExecuteFunction(4.0f));
    }

    IEnumerator WaitAndExecuteFunction(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        staff_line3.GetComponent<TextMeshProUGUI>().enabled = false;
        Sound.GetComponent<TextMeshProUGUI>().enabled = false;
        value.GetComponent<TextMeshProUGUI>().enabled = false;
        bar.GetComponent<Image>().enabled = false;
        cafe_board.GetComponent<TextMeshProUGUI>().enabled = false;
        blank_image.GetComponent<Renderer>().enabled = true;
        wait.GetComponent<TextMeshProUGUI>().enabled = true;
        //������ ���� ������Ʈ�� 1�ʾ� ������ ��Ÿ������
        StartCoroutine(ShowDrinksSequentially());

        if (isSoft1 == true && isSoft2 == true) 
        {
            //������ ����
            drink_num = 0;
            ScoreManager.instance.AddScore2(2,1);
        }
        else if (isSoft1 == true && isSoft2 == false)
        {
            //������ ����
            drink_num = 1;
            ScoreManager.instance.AddScore2(2,2);
        }
        else if (isSoft1 == false && isSoft2 == true)
        {
            //������ ����
            drink_num = 2;
            ScoreManager.instance.AddScore2(2,2);
        }
        else
        {
            //������ ����
            drink_num = 3;
            ScoreManager.instance.AddScore2(2,3);
        }
        
    }

    IEnumerator ShowDrinksSequentially()
    {
        // �� ���� ������Ʈ�� ������� Ȱ��ȭ�ϰ� 1�ʾ� ����� �� ��Ȱ��ȭ
        for (int j = 0; j<3; j++)
        {
            for (int i = 0; i < drinkObjects.Length; i++)
            {
                drinkObjects[i].SetActive(true); // ���� ������Ʈ Ȱ��ȭ
                yield return new WaitForSeconds(1.0f); // 1�� ���
                drinkObjects[i].SetActive(false); // ���� ������Ʈ ��Ȱ��ȭ
            }
        }
        blank_image.GetComponent<Renderer>().enabled = false;
        wait.GetComponent<TextMeshProUGUI>().enabled = false;
        Sound.GetComponent<TextMeshProUGUI>().enabled = true;
        value.GetComponent<TextMeshProUGUI>().enabled = true;
        bar.GetComponent<Image>().enabled = true;
        cafe_board.GetComponent<TextMeshProUGUI>().enabled = true;
        staff_bubble1.GetComponent<Renderer>().enabled = false; // ��ǳ��
        StartCoroutine(Get_drink(1.0f));
    }

    void TriggerEvent_loud() // �ò����� �����ּ���
    {
        // �մ��̹���, ��ǳ��, �ʹ� �ò������!
        customer.GetComponent<Renderer>().enabled = true; // �մ� �̹���
        customer_angry.GetComponent<Renderer>().enabled = true;
        customer_bubble.GetComponent<Renderer>().enabled = true; // ��ǳ��
        customer_line.GetComponent<TextMeshProUGUI>().enabled = true; //�ؽ�Ʈ
        a_customer.Play();

        StartCoroutine(Retry_loud1(5.0f));
    }
    
    IEnumerator Retry_loud1(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        // �ٸ� �մԵ��� �������ϴ� �����ּ���̤�
        staff_bubble2.GetComponent<Renderer>().enabled = true; // ��ǳ��
        staff_audio4.Play();
        staff_smile.GetComponent<Renderer>().enabled = false;
        staff_sad.GetComponent<Renderer>().enabled = true;
        staff_line4.GetComponent<TextMeshProUGUI>().enabled = true; //�ؽ�Ʈ

        StartCoroutine(Retry_loud2(5.0f));
    }
    IEnumerator Retry_loud2(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        // ��Ҹ��� �ʹ� �ǳ���.. �ٽ� �ֹ��غ���~
        fairy.GetComponent<Renderer>().enabled = true;
        blank.GetComponent<Renderer>().enabled = true;
        blank_image.GetComponent<Renderer>().enabled = true;
        Sound.GetComponent<TextMeshProUGUI>().enabled = false;
        value.GetComponent<TextMeshProUGUI>().enabled = false;
        bar.GetComponent<Image>().enabled = false;
        cafe_board.GetComponent<TextMeshProUGUI>().enabled = false;
        fairy1.GetComponent<TextMeshProUGUI>().enabled = true;
        a_fairy1.Play();
        retry_button.SetActive(true);
        retry.GetComponent<TextMeshProUGUI>().enabled = true;

        customer.GetComponent<Renderer>().enabled = false; // �մ� �̹���
        customer_angry.GetComponent<Renderer>().enabled = false;
        customer_bubble.GetComponent<Renderer>().enabled = false; // ��ǳ��
        customer_line.GetComponent<TextMeshProUGUI>().enabled = false; //�ؽ�Ʈ
        staff_bubble2.GetComponent<Renderer>().enabled = false; // ��ǳ��
        staff_line4.GetComponent<TextMeshProUGUI>().enabled = false; //�ؽ�Ʈ
        staff_smile.GetComponent<Renderer>().enabled = true;
        staff_sad.GetComponent<Renderer>().enabled = false;
        arrow.GetComponent<Renderer>().enabled = true;


        turn = 0;
        isSoft1 = false;
        isSoft2 = false;
    }

    IEnumerator Get_drink(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        // ���� ����
        drinks[drink_num].SetActive(true); // ���� �̹���
        drinks[drink_num].transform.position = new Vector3(-3.32f,-1.86f,-0.7f);
        ScoreManager.instance.AddItem(drinks[drink_num]);
        drinks_t[drink_num].GetComponent<TextMeshProUGUI>().enabled = true; // ���
        staff_audio_d[drink_num].Play();
        staff_bubble1.GetComponent<Renderer>().enabled = true; // ��ǳ��
        
        if (drink_num == 3)
        {
            StartCoroutine(TriggerEvent_good(5.0f)); // ����
        }
        else
        {
            StartCoroutine(TriggerEvent_bad(4.0f));
        }
    }

    IEnumerator TriggerEvent_good(float waitTime) // ������ ���� ����
    {
        yield return new WaitForSeconds(waitTime);
        drinks_t[drink_num].GetComponent<TextMeshProUGUI>().enabled = false; // ���
        a_fairy2.Play();
        fairy.GetComponent<Renderer>().enabled = true;
        blank.GetComponent<Renderer>().enabled = true;
        blank_image.GetComponent<Renderer>().enabled = true; //������� �������� ���
        Sound.GetComponent<TextMeshProUGUI>().enabled = false;
        value.GetComponent<TextMeshProUGUI>().enabled = false;
        bar.GetComponent<Image>().enabled = false;
        cafe_board.GetComponent<TextMeshProUGUI>().enabled = false;
        fairy2.GetComponent<TextMeshProUGUI>().enabled = true;

        int stagescore = ScoreManager.instance.GetStageScore(2);
        UnityEngine.Debug.Log("스테이지2 점수 : " + stagescore);

        drinks[drink_num].transform.position = new Vector3(0f, -3f, -3f);

        star1.SetActive(true);
        star2.SetActive(true);
        star3.SetActive(true);

        yield return new WaitForSeconds(a_fairy2.clip.length + 2.0f);
        next_button.SetActive(true);
    }

    IEnumerator TriggerEvent_bad(float waitTime) // �������� ���� ����
    {
        yield return new WaitForSeconds(waitTime);
        drinks_t[drink_num].GetComponent<TextMeshProUGUI>().enabled = false; // ���
        staff_bad1.GetComponent<TextMeshProUGUI>().enabled = true;
        a_staff_bad1.Play();

        yield return new WaitForSeconds(4.0f);

        staff_bad1.GetComponent<TextMeshProUGUI>().enabled = false;
        staff_bad2.GetComponent<TextMeshProUGUI>().enabled = true;
        a_staff_bad2.Play();
        staff_smile.GetComponent<Renderer>().enabled = false;
        staff_sad.GetComponent<Renderer>().enabled = true;

        yield return new WaitForSeconds(5.0f);
        staff_bad2.GetComponent<TextMeshProUGUI>().enabled = false;
        staff_bubble1.GetComponent<Renderer>().enabled = false; // ��ǳ��
        fairy.GetComponent<Renderer>().enabled = true;
        blank.GetComponent<Renderer>().enabled = true;
        blank_image.GetComponent<Renderer>().enabled = true; //������� �������� ���
        Sound.GetComponent<TextMeshProUGUI>().enabled = false;
        value.GetComponent<TextMeshProUGUI>().enabled = false;
        bar.GetComponent<Image>().enabled = false;
        fairy3.GetComponent<TextMeshProUGUI>().enabled = true;
        a_fairy3.Play();

        int stagescore = ScoreManager.instance.GetStageScore(2);
        UnityEngine.Debug.Log("��������2 ���� : " + stagescore);

        drinks[drink_num].transform.position = new Vector3(0f, -3f, -3f);

        Sound.GetComponent<TextMeshProUGUI>().enabled = false;
        value.GetComponent<TextMeshProUGUI>().enabled = false;
        bar.GetComponent<Image>().enabled = false;
        cafe_board.GetComponent<TextMeshProUGUI>().enabled = false;

        if (stagescore == 1)
        {
            star2.SetActive(true);
        }
        else 
        {
            star1.SetActive(true);
            star3.SetActive(true);
        }
        yield return new WaitForSeconds(a_fairy3.clip.length + 2.0f);
        next_button.SetActive(true);
    }
}