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

    private int turn = 0; // 두번의 말하기

    private bool isSoft1 = false;
    private bool isSoft2 = false;


    //오브젝트
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

    public GameObject[] drinkObjects; // 음료 오브젝트 배열

    //텍스트
    public TextMeshProUGUI staff_line1; // 주문
    public TextMeshProUGUI staff_line2; // 핫 아이스
    public TextMeshProUGUI staff_line3; // 잠시만 기다려주세요
    public TextMeshProUGUI staff_line4; // 시끄러워요
    public TextMeshProUGUI line1; // 초코우유
    public TextMeshProUGUI line2; // 따뜻한 걸로
    public TextMeshProUGUI fairy1;
    public TextMeshProUGUI wait; // 기다리는 중
    public TextMeshProUGUI customer_line;
    public TextMeshProUGUI cafe_board;
    public TextMeshProUGUI retry;

    //오디오
    public AudioSource staff_audio1;



    void Start()
    {

        // SoundAnalyzer 컴포넌트 찾기
        soundAnalyzer = FindObjectOfType<SoundAnalyzer2>();
        if (soundAnalyzer != null)
        {
            soundAnalyzer.enabled = true; // 컴포넌트를 활성화
        }
        if (soundAnalyzer == null)
        {
            UnityEngine.Debug.LogError("SoundAnalyzer 컴포넌트를 찾을 수 없습니다.");
        }

        
    }

    void Update()
    {

    }

    private void OnMouseDown()
    {
        // 어서오세요 주문하시겠어요? 소리&말풍선 출력
        staff_bubble1.GetComponent<Renderer>().enabled = true; // 말풍선
        if (turn == 0)
            staff_line1.GetComponent<TextMeshProUGUI>().enabled = true; //텍스트
        else
            staff_line2.GetComponent<TextMeshProUGUI>().enabled = true;
        if (turn == 0) 
            staff_audio1.Play(); //오디오
        else
            staff_audio1.Play(); //오디오 2로 수정 필요****************************************************
        StartCoroutine(StartRecordingAfterAudioEnds(staff_audio1.clip.length + 1.0f));//오디오 끝날때까지
    }

    private System.Collections.IEnumerator StartRecordingAfterAudioEnds(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // 오디오가 끝날 때까지 기다림
        if (soundAnalyzer != null)
        {
            player_bubble.GetComponent<Renderer>().enabled = true; // 말풍선
            if (turn == 0)
                line1.GetComponent<TextMeshProUGUI>().enabled = true; //텍스트
            else
                line2.GetComponent<TextMeshProUGUI>().enabled = true;
            microphone.GetComponent<Renderer>().enabled = true;
            soundAnalyzer.StartRecording(); // 마이크 입력 시작
            StartCoroutine(WaitAndReceiveDbValues()); // 코루틴 시작
        }
    }

    IEnumerator WaitAndReceiveDbValues()
    {
        turn++;

        float sumDb = 0f;
        int countDb = 0;

        yield return new WaitForSeconds(5); // 5초 기다림

        if (!soundAnalyzer.isRecording) // 녹음 끝났는지 확인
        {
            staff_bubble1.GetComponent<Renderer>().enabled = false; // 말풍선
            staff_line1.GetComponent<TextMeshProUGUI>().enabled = false; //텍스트
            staff_line2.GetComponent<TextMeshProUGUI>().enabled = false; //텍스트
            player_bubble.GetComponent<Renderer>().enabled = false; // 말풍선
            line1.GetComponent<TextMeshProUGUI>().enabled = false; //텍스트
            line2.GetComponent<TextMeshProUGUI>().enabled = false; //텍스트
            microphone.GetComponent<Renderer>().enabled = false;

            DBL = soundAnalyzer.GetDbValues();
            UnityEngine.Debug.Log("getdb");

            foreach (float dbValue in DBL)
            {
                if (dbValue != 0f) // 0이 아닌 값인 경우에만 처리
                {
                    sumDb += dbValue;
                    countDb++;
                }
            }

            float averageDb = 0f;
            if (countDb > 0)
            {
                averageDb = sumDb / countDb; // 0이 아닌 값의 평균 데시벨 값 계산
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
                TriggerEvent_loud(); // turn 상관없이
            }
            else // 데시벨 값이 적절한 경우
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
            // 녹음 진행중 -> 녹음 종료
            UnityEngine.Debug.Log("Recording is still in progress.");
        }
    }

    void TriggerEvent_Wait()
    {
        staff_line3.GetComponent<TextMeshProUGUI>().enabled = true; // 잠시만 기다려주세요
        staff_bubble1.GetComponent<Renderer>().enabled = true; // 말풍선
        //staff_audio2.Play(); //오디오
        StartCoroutine(WaitAndExecuteFunction(4.0f));
    }

    IEnumerator WaitAndExecuteFunction(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        staff_line3.GetComponent<TextMeshProUGUI>().enabled = false;
        cafe_board.GetComponent<TextMeshProUGUI>().enabled = false;
        blank_image.GetComponent<Renderer>().enabled = true;
        wait.GetComponent<TextMeshProUGUI>().enabled = true;
        //세개의 음료 오브젝트가 1초씩 번갈아 나타나도록
        StartCoroutine(ShowDrinksSequentially());

        if (isSoft1 == true && isSoft2 == true) 
        {
            //차가운 딸기
        }
        else if (isSoft1 == true && isSoft2 == false)
        {
            //따뜻한 딸기
        }
        else if (isSoft1 == false && !isSoft2 == true)
        {
            //차가운 초코
        }
        else
        {
            TriggerEvent_good(); // 따뜻한 초코
        }
    }

    IEnumerator ShowDrinksSequentially()
    {
        // 각 음료 오브젝트를 순서대로 활성화하고 1초씩 대기한 후 비활성화
        for (int j = 0; j<3; j++)
        {
            for (int i = 0; i < drinkObjects.Length; i++)
            {
                drinkObjects[i].SetActive(true); // 음료 오브젝트 활성화
                yield return new WaitForSeconds(1.0f); // 1초 대기
                drinkObjects[i].SetActive(false); // 음료 오브젝트 비활성화
            }
        }
        blank_image.GetComponent<Renderer>().enabled = false;
        wait.GetComponent<TextMeshProUGUI>().enabled = false;
        cafe_board.GetComponent<TextMeshProUGUI>().enabled = true;

    }

    void TriggerEvent_loud() // 시끄러우니 나가주세욧
    {
        // 손님이미지, 말풍선, 너무 시끄러워요!
        customer.GetComponent<Renderer>().enabled = true; // 손님 이미지
        customer_angry.GetComponent<Renderer>().enabled = true;
        customer_bubble.GetComponent<Renderer>().enabled = true; // 말풍선
        customer_line.GetComponent<TextMeshProUGUI>().enabled = true; //텍스트
        
        StartCoroutine(Retry_loud1(5.0f));
    }
    
    IEnumerator Retry_loud1(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        // 다른 손님들이 불편해하니 나가주세요ㅜㅜ
        staff_bubble2.GetComponent<Renderer>().enabled = true; // 말풍선
        staff_smile.GetComponent<Renderer>().enabled = false;
        staff_sad.GetComponent<Renderer>().enabled = true;
        staff_line4.GetComponent<TextMeshProUGUI>().enabled = true; //텍스트

        StartCoroutine(Retry_loud2(5.0f));
    }
    IEnumerator Retry_loud2(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        // 목소리가 너무 컸나봐.. 다시 주문해보자~
        fairy.GetComponent<Renderer>().enabled = true;
        blank.GetComponent<Renderer>().enabled = true;
        blank_image.GetComponent<Renderer>().enabled = true;
        fairy1.GetComponent<TextMeshProUGUI>().enabled = true;
        retry_button.SetActive(true);
        retry.GetComponent<TextMeshProUGUI>().enabled = true;

        customer.GetComponent<Renderer>().enabled = false; // 손님 이미지
        customer_angry.GetComponent<Renderer>().enabled = false;
        customer_bubble.GetComponent<Renderer>().enabled = false; // 말풍선
        customer_line.GetComponent<TextMeshProUGUI>().enabled = false; //텍스트
        staff_bubble2.GetComponent<Renderer>().enabled = false; // 말풍선
        staff_line4.GetComponent<TextMeshProUGUI>().enabled = false; //텍스트
        staff_smile.GetComponent<Renderer>().enabled = true;
        staff_sad.GetComponent<Renderer>().enabled = false;

        turn = 0;
        isSoft1 = false;
        isSoft2 = false;
    }

    
    void TriggerEvent_TooSoft() // 다른 음료 나옴
    {
        // 다른 음료 나옴, 주문하신 딸기우유 나왔습니다~
        // 네? 핫초코 주문했다구요?
        // 요정 : 목소리가 너무 작아서 잘 안들렸나봐 다시 주문해보자~
    }
    
    void TriggerEvent_good() // 적절한 음료 나옴
    {
        // 올바른 음료 나옴, 주문하신 핫초코 나왔습니다~
        // 잘했어!
    }

}