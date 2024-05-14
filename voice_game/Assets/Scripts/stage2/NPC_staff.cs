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

    //오브젝트
    public GameObject staff_bubble;
    public GameObject player_bubble;
    public GameObject fairy;
    public GameObject blank;
    public GameObject microphone;
    public GameObject blank_image;

    public GameObject[] drinkObjects; // 음료 오브젝트 배열

    //텍스트
    public TextMeshProUGUI staff_line1;
    public TextMeshProUGUI staff_line2;
    public TextMeshProUGUI line1;
    public TextMeshProUGUI wait;
    public TextMeshProUGUI cafe_board;

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
        UnityEngine.Debug.Log("click");
        // 어서오세요 주문하시겠어요? 소리&말풍선 출력
        staff_bubble.GetComponent<Renderer>().enabled = true; // 말풍선
        staff_line1.GetComponent<TextMeshProUGUI>().enabled = true; //텍스트
        staff_audio1.Play(); //오디오
        StartCoroutine(StartRecordingAfterAudioEnds(staff_audio1.clip.length + 1.0f));//오디오 끝날때까지
    }

    private System.Collections.IEnumerator StartRecordingAfterAudioEnds(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // 오디오가 끝날 때까지 기다림
        if (soundAnalyzer != null)
        {
            player_bubble.GetComponent<Renderer>().enabled = true; // 말풍선
            line1.GetComponent<TextMeshProUGUI>().enabled = true; //텍스트
            microphone.GetComponent<Renderer>().enabled = true;
            soundAnalyzer.StartRecording(); // 마이크 입력 시작
            StartCoroutine(WaitAndReceiveDbValues()); // 코루틴 시작
        }
    }

    IEnumerator WaitAndReceiveDbValues()
    {
        float sumDb = 0f;
        int countDb = 0;

        yield return new WaitForSeconds(5); // 5초 기다림

        if (!soundAnalyzer.isRecording) // 녹음 끝났는지 확인
        {
            staff_bubble.GetComponent<Renderer>().enabled = false; // 말풍선
            staff_line1.GetComponent<TextMeshProUGUI>().enabled = false; //텍스트
            player_bubble.GetComponent<Renderer>().enabled = false; // 말풍선
            line1.GetComponent<TextMeshProUGUI>().enabled = false; //텍스트
            microphone.GetComponent<Renderer>().enabled = false;

            DBL = soundAnalyzer.GetDbValues();
            UnityEngine.Debug.Log("getdb");

            /* 데시벨 값에 따른 처리 필요
             * 목소리 클 때() : 카페에서는 시끄럽게 하면 안돼요 & 처음부터 재시도
             * 목소리 작을 때() : 다른 음료 나옴
             * 목소리 적절할 때() : 적절한 음료 나옴
             */
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
            else // 데시벨 값이 적절한 경우
            {
                TriggerEvent_Wait();
            }
        }
        else
        {
            // 녹음 진행중 -> 녹음 종료
            UnityEngine.Debug.Log("Recording is still in progress.");
        }
    }

    void TriggerEvent_TooSoft()
    {
        //손님 시끄러워요! 직원이 나가주세요!
    }

    void TriggerEvent_Wait()
    {
        staff_line2.GetComponent<TextMeshProUGUI>().enabled = true; // 잠시만 기다려주세요
        staff_bubble.GetComponent<Renderer>().enabled = true; // 말풍선
        //staff_audio2.Play(); //오디오
        StartCoroutine(WaitAndExecuteFunction(4.0f));
    }

    IEnumerator WaitAndExecuteFunction(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        staff_line2.GetComponent<TextMeshProUGUI>().enabled = false;
        cafe_board.GetComponent<TextMeshProUGUI>().enabled = false;
        blank_image.GetComponent<Renderer>().enabled = true;
        wait.GetComponent<TextMeshProUGUI>().enabled = true;
        //세개의 음료 오브젝트가 1초씩 번갈아 나타나도록 구현
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
        
    }

    void TriggerEvent_loud()
    {

    }

    void TriggerEvent_good()
    {

    }
}