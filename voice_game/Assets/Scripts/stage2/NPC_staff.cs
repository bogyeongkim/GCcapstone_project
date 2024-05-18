using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

    public GameObject[] drinkObjects; // ���� ������Ʈ �迭

    //�ؽ�Ʈ
    public TextMeshProUGUI staff_line1; // �ֹ�
    public TextMeshProUGUI staff_line2; // �� ���̽�
    public TextMeshProUGUI staff_line3; // ��ø� ��ٷ��ּ���
    public TextMeshProUGUI staff_line4; // �ò�������
    public TextMeshProUGUI line1; // ���ڿ���
    public TextMeshProUGUI line2; // ������ �ɷ�
    public TextMeshProUGUI fairy1;
    public TextMeshProUGUI wait; // ��ٸ��� ��
    public TextMeshProUGUI customer_line;
    public TextMeshProUGUI cafe_board;
    public TextMeshProUGUI retry;

    //�����
    public AudioSource staff_audio1;



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

    private void OnMouseDown()
    {
        // ������� �ֹ��Ͻðھ��? �Ҹ�&��ǳ�� ���
        staff_bubble1.GetComponent<Renderer>().enabled = true; // ��ǳ��
        if (turn == 0)
            staff_line1.GetComponent<TextMeshProUGUI>().enabled = true; //�ؽ�Ʈ
        else
            staff_line2.GetComponent<TextMeshProUGUI>().enabled = true;
        if (turn == 0) 
            staff_audio1.Play(); //�����
        else
            staff_audio1.Play(); //����� 2�� ���� �ʿ�****************************************************
        StartCoroutine(StartRecordingAfterAudioEnds(staff_audio1.clip.length + 1.0f));//����� ����������
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
                if (dbValue != 0f) // 0�� �ƴ� ���� ��쿡�� ó��
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
            if (averageDb < 30) 
            {
                if (turn == 1)
                {
                    isSoft1 = true;
                    OnMouseDown();
                }
                else if (turn == 2)
                {
                    isSoft2 = true;
                    TriggerEvent_Wait();
                }
                
            }
            else if (averageDb > 35) 
            {
                TriggerEvent_loud(); // turn �������
            }
            else // ���ú� ���� ������ ���
            {
                if (turn == 1)
                {
                    OnMouseDown();
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
        staff_bubble1.GetComponent<Renderer>().enabled = true; // ��ǳ��
        //staff_audio2.Play(); //�����
        StartCoroutine(WaitAndExecuteFunction(4.0f));
    }

    IEnumerator WaitAndExecuteFunction(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        staff_line3.GetComponent<TextMeshProUGUI>().enabled = false;
        cafe_board.GetComponent<TextMeshProUGUI>().enabled = false;
        blank_image.GetComponent<Renderer>().enabled = true;
        wait.GetComponent<TextMeshProUGUI>().enabled = true;
        //������ ���� ������Ʈ�� 1�ʾ� ������ ��Ÿ������
        StartCoroutine(ShowDrinksSequentially());

        if (isSoft1 == true && isSoft2 == true) 
        {
            //������ ����
        }
        else if (isSoft1 == true && isSoft2 == false)
        {
            //������ ����
        }
        else if (isSoft1 == false && !isSoft2 == true)
        {
            //������ ����
        }
        else
        {
            TriggerEvent_good(); // ������ ����
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
        cafe_board.GetComponent<TextMeshProUGUI>().enabled = true;

    }

    void TriggerEvent_loud() // �ò������ �����ּ���
    {
        // �մ��̹���, ��ǳ��, �ʹ� �ò�������!
        customer.GetComponent<Renderer>().enabled = true; // �մ� �̹���
        customer_angry.GetComponent<Renderer>().enabled = true;
        customer_bubble.GetComponent<Renderer>().enabled = true; // ��ǳ��
        customer_line.GetComponent<TextMeshProUGUI>().enabled = true; //�ؽ�Ʈ
        
        StartCoroutine(Retry_loud1(5.0f));
    }
    
    IEnumerator Retry_loud1(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        // �ٸ� �մԵ��� �������ϴ� �����ּ���̤�
        staff_bubble2.GetComponent<Renderer>().enabled = true; // ��ǳ��
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
        fairy1.GetComponent<TextMeshProUGUI>().enabled = true;
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

        turn = 0;
        isSoft1 = false;
        isSoft2 = false;
    }

    
    void TriggerEvent_TooSoft() // �ٸ� ���� ����
    {
        // �ٸ� ���� ����, �ֹ��Ͻ� ������� ���Խ��ϴ�~
        // ��? ������ �ֹ��ߴٱ���?
        // ���� : ��Ҹ��� �ʹ� �۾Ƽ� �� �ȵ�ȳ��� �ٽ� �ֹ��غ���~
    }
    
    void TriggerEvent_good() // ������ ���� ����
    {
        // �ùٸ� ���� ����, �ֹ��Ͻ� ������ ���Խ��ϴ�~
        // ���߾�!
    }

}