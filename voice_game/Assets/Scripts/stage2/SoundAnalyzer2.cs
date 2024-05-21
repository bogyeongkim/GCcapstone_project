using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class SoundAnalyzer2 : MonoBehaviour
{
    const float REFERENCE = 0.00002f; // 레퍼런스 값

    public string microphoneName; // 사용할 마이크 이름
    public int sampleRate = 44100; // 샘플링 속도
    public int bufferSize = 2048; // 버퍼 크기
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris; // FFT 창 함수


    //******************************************************경고 추가
    public float warningThreshold = 70f;
    public GameObject warningUI;
    public Image screenOverlay;
    private bool isWarningActive = false;

    public bool isRecording = false; // 녹음 상태 확인 flag
    float recordStartTime = 0f; // 녹음 시작 시간

    float[] buffer; // 오디오 버퍼
    public List<float> dbValues = new List<float>();//데시벨 값 저장 리스트

    private AudioSource audioSource;

    private float timer = 0f;
    public float measurementInterval = 0.05f; // 측정 간격 (초)

    void Start()
    {
        // 마이크 디바이스 설정
        string[] devices = Microphone.devices;
        if (devices.Length > 0)
        {
            microphoneName = devices[0]; // 첫 번째 마이크 선택
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = Microphone.Start(microphoneName, true, 1, sampleRate);
            audioSource.loop = true;
            //audioSource.mute = true; // 소리를 무음으로 설정
            while (!(Microphone.GetPosition(null) > 0)) { } // 마이크 시작까지 대기
            audioSource.Play();
            buffer = new float[bufferSize];
        }
        else
        {
            UnityEngine.Debug.LogError("마이크를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        if (isRecording)
        {
            if (Time.time - recordStartTime > 5f) // 5초 동안만 측정
            {
                isRecording = false; // 녹음 상태 종료
                return;
            }

            timer += Time.deltaTime; // 타이머 업데이트

            if (timer >= measurementInterval)
            {
                timer -= measurementInterval;

                // 오디오 데이터 읽기
                AudioSource audioSource = GetComponent<AudioSource>();
                int position = Microphone.GetPosition(null);
                audioSource.clip.GetData(buffer, position);

                float[] spectrum = new float[bufferSize];
                audioSource.GetSpectrumData(spectrum, 0, fftWindow);

                float maxFrequency = 0f;
                float maxAmplitude = 0f;

                // 최대 진폭 가지는 주파수 도출
                for (int i = 0; i < bufferSize; i++)
                {
                    float amplitude = spectrum[i];
                    if (amplitude > maxAmplitude)
                    {
                        maxAmplitude = amplitude;
                        maxFrequency = i * sampleRate / (float)bufferSize;
                    }
                }

                if (maxFrequency < 0f)
                    maxFrequency = 0;


                //주파수 출력
                //UnityEngine.Debug.Log("주파수: " + maxFrequency.ToString("F2") + " Hz");

                // 음압 레벨 계산
                float pressure = 0f;
                for (int i = 0; i < bufferSize; i++)
                {
                    pressure += Mathf.Abs(buffer[i]);
                }
                pressure /= bufferSize;

                // 압력을 데시벨로 변환
                float db = 20 * Mathf.Log10(pressure / REFERENCE);


                float Ra = (Mathf.Pow(12194f, 2) * Mathf.Pow(maxFrequency, 4))
                    / ((Mathf.Pow(maxFrequency, 2) + Mathf.Pow(20.6f, 2))
                    * Mathf.Sqrt((Mathf.Pow(maxFrequency, 2) + Mathf.Pow(107.7f, 2))
                    * (Mathf.Pow(maxFrequency, 2) + Mathf.Pow(737.9f, 2)))
                    * (Mathf.Pow(maxFrequency, 2) + Mathf.Pow(12194f, 2)));

                float weight = 20 * Mathf.Log10(Ra) + 2.0f;
                if (weight < -50f) weight = 0;

                // A-weighted 데시벨 값을 기존 데시벨 값에 더함
                float dbA = db + weight;

                if (dbA < 0f) dbA = 0f;

                // 데시벨 값 리스트에 추가
                dbValues.Add(dbA);

                UnityEngine.Debug.Log("[" + dbValues.Count + "]" + "dBA : " + dbA);

                //*************************************************************경고 추가
                if (dbA > warningThreshold)
                {
                    TriggerWarning();
                }
            }
        }
    }

    void TriggerWarning()
    {
        //StartCoroutine(TriggerWarningUI());
        Handheld.Vibrate(); // 진동오도록
        StartCoroutine(FlashWarning()); // 화면 붉게 깜빡이도록
    }

    IEnumerator TriggerWarningUI()
    {
        if (warningUI != null)
        {
            warningUI.SetActive(true); // 경고 UI 오브젝트 표시
            yield return new WaitForSeconds(0.5f); // 0.5초 동안 대기
            warningUI.SetActive(false); // 경고 UI 오브젝트 비활성화
        }
    }

    IEnumerator FlashWarning()
    {
        if (isWarningActive) // 화면 깜빡이는 중인지 확인
        {
            yield break;
        }

        isWarningActive = true; // 경고 상태 활성화

        Color originalColor = screenOverlay.color;
        Color warningColor = new Color(1, 0, 0, 0.2f);

        screenOverlay.color = warningColor; // 반투명한 붉은색 화면으로
        yield return new WaitForSeconds(0.3f);
        screenOverlay.color = originalColor;
        yield return new WaitForSeconds(0.3f);

        isWarningActive = false; // 경고 상태 비활성화
    }

    public void ClearWarning()
    {
        if (warningUI != null)
        {
            warningUI.SetActive(false);
        }

        screenOverlay.color = new Color(0, 0, 0, 0); 
        isWarningActive = false; 
    }

    void OnDisable()
    {
        if (audioSource != null && Microphone.IsRecording(microphoneName))
        {
            audioSource.Stop();
            Microphone.End(microphoneName);
        }
    }

    // 데시벨 값 리스트 반환
    public List<float> GetDbValues()
    {
        return dbValues;
    }

    public void StartRecording()
    {
        UnityEngine.Debug.Log("StartRecording");
        UnityEngine.Debug.Log(isRecording);
        UnityEngine.Debug.Log(Microphone.IsRecording(microphoneName));
        //if (!isRecording && Microphone.IsRecording(microphoneName) == false)
        if (isRecording == false)
        {
            UnityEngine.Debug.Log("isRecording");
            isRecording = true;
            recordStartTime = Time.time;
            dbValues.Clear(); // 기존 측정값 초기화
        }
    }
}