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

    private bool isLoud = false;

    //������Ʈ
    public GameObject staff_bubble;
    public GameObject player_bubble;
    public GameObject fairy;
    public GameObject blank;
    public GameObject microphone;
    public GameObject blank_image;

    public GameObject[] drinkObjects; // ���� ������Ʈ �迭

    //�ؽ�Ʈ
    public TextMeshProUGUI staff_line1;
    public TextMeshProUGUI staff_line2;
    public TextMeshProUGUI line1;
    public TextMeshProUGUI wait;
    public TextMeshProUGUI cafe_board;

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
        UnityEngine.Debug.Log("click");
        // ������� �ֹ��Ͻðھ��? �Ҹ�&��ǳ�� ���
        staff_bubble.GetComponent<Renderer>().enabled = true; // ��ǳ��
        staff_line1.GetComponent<TextMeshProUGUI>().enabled = true; //�ؽ�Ʈ
        staff_audio1.Play(); //�����
        StartCoroutine(StartRecordingAfterAudioEnds(staff_audio1.clip.length + 1.0f));//����� ����������
    }

    private System.Collections.IEnumerator StartRecordingAfterAudioEnds(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // ������� ���� ������ ��ٸ�
        if (soundAnalyzer != null)
        {
            player_bubble.GetComponent<Renderer>().enabled = true; // ��ǳ��
            line1.GetComponent<TextMeshProUGUI>().enabled = true; //�ؽ�Ʈ
            microphone.GetComponent<Renderer>().enabled = true;
            soundAnalyzer.StartRecording(); // ����ũ �Է� ����
            StartCoroutine(WaitAndReceiveDbValues()); // �ڷ�ƾ ����
        }
    }

    IEnumerator WaitAndReceiveDbValues()
    {
        float sumDb = 0f;
        int countDb = 0;

        yield return new WaitForSeconds(5); // 5�� ��ٸ�

        if (!soundAnalyzer.isRecording) // ���� �������� Ȯ��
        {
            staff_bubble.GetComponent<Renderer>().enabled = false; // ��ǳ��
            staff_line1.GetComponent<TextMeshProUGUI>().enabled = false; //�ؽ�Ʈ
            player_bubble.GetComponent<Renderer>().enabled = false; // ��ǳ��
            line1.GetComponent<TextMeshProUGUI>().enabled = false; //�ؽ�Ʈ
            microphone.GetComponent<Renderer>().enabled = false;

            DBL = soundAnalyzer.GetDbValues();
            UnityEngine.Debug.Log("getdb");

            /* ���ú� ���� ���� ó�� �ʿ�
             * ��Ҹ� Ŭ ��() : ī�信���� �ò����� �ϸ� �ȵſ� & ó������ ��õ�
             * ��Ҹ� ���� ��() : �ٸ� ���� ����
             * ��Ҹ� ������ ��() : ������ ���� ����
             */
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
            }
            if (averageDb < 20) 
            {
                TriggerEvent_TooSoft();
            }
            else if (averageDb > 60) 
            {
                isLoud = true;
                TriggerEvent_Wait();
            }
            else // ���ú� ���� ������ ���
            {
                TriggerEvent_Wait();
            }
        }
        else
        {
            // ���� ������ -> ���� ����
            UnityEngine.Debug.Log("Recording is still in progress.");
        }
    }

    void TriggerEvent_TooSoft()
    {
        //�մ� �ò�������! ������ �����ּ���!
    }

    void TriggerEvent_Wait()
    {
        staff_line2.GetComponent<TextMeshProUGUI>().enabled = true; // ��ø� ��ٷ��ּ���
        staff_bubble.GetComponent<Renderer>().enabled = true; // ��ǳ��
        //staff_audio2.Play(); //�����
        StartCoroutine(WaitAndExecuteFunction(4.0f));
    }

    IEnumerator WaitAndExecuteFunction(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        staff_line2.GetComponent<TextMeshProUGUI>().enabled = false;
        cafe_board.GetComponent<TextMeshProUGUI>().enabled = false;
        blank_image.GetComponent<Renderer>().enabled = true;
        wait.GetComponent<TextMeshProUGUI>().enabled = true;
        //������ ���� ������Ʈ�� 1�ʾ� ������ ��Ÿ������ ����
        StartCoroutine(ShowDrinksSequentially());

        if (isLoud) 
        {
            TriggerEvent_loud();
        }
        else
        {
            TriggerEvent_good();
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
        
    }

    void TriggerEvent_loud()
    {

    }

    void TriggerEvent_good()
    {

    }
}