using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAnalyzer : MonoBehaviour
{
    public DbVisualizer dbVisualizer; // DbVisualizer에 대한 참조

    const float REFERENCE = 0.00002f; // 레퍼런스 값

    public string microphoneName; // 사용할 마이크 이름
    public int sampleRate = 44100; // 샘플링 속도
    public int bufferSize = 2048; // 버퍼 크기
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris; // FFT 창 함수

    public bool isRecording = false; // 녹음 상태 확인 flag
    float recordStartTime = 0f; // 녹음 시작 시간

    float[] buffer; // 오디오 버퍼
    public List<float> dbValues = new List<float>();//데시벨 값 저장 리스트

    private AudioSource audioSource;

    // === 측정 타이머 ===
    private float measurementTimer = 0f;
    public float measurementInterval = 0.05f; // 측정 간격 (초)

    // === 자동 캘리브레이션 관련 ===
    public bool enableAutoCalibration = true;
    public float calibrationDuration = 6f; // 주변 소음 측정 시간(초)

    private bool isCalibrating = false;
    private bool calibrationDone = false;
    private float calibrationStartTime = 0f;
    private float calibrationTimer = 0f;
    private List<float> calibrationDbSamples = new List<float>();
    private float calibrationOffsetDb = 0f; // 이후 dBA에 더해질 오프셋

    void Start()
    {
        string[] devices = Microphone.devices;
        if (devices.Length > 0)
        {
            microphoneName = devices[0];
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.mute = true; // 게임에는 소리가 들리지 않도록

            StartCoroutine(SetupMicrophone());
        }
        else
        {
            Debug.LogError("마이크를 찾을 수 없습니다.");
        }
    }

    IEnumerator SetupMicrophone()
    {
        audioSource.clip = Microphone.Start(microphoneName, true, 1, sampleRate);
        audioSource.loop = true;
        // 마이크가 준비될 때까지 기다림
        yield return new WaitUntil(() => Microphone.GetPosition(microphoneName) > 0);
        audioSource.Play();
        buffer = new float[bufferSize];

        // === 마이크 준비 직후 자동 캘리브레이션 시작 ===
        if (enableAutoCalibration)
        {
            isCalibrating = true;
            calibrationDone = false;
            calibrationTimer = 0f;
            calibrationStartTime = Time.time;
            calibrationDbSamples.Clear();

            Debug.Log("[Calibration] 주변 소음 측정 시작");
        }
    }

    void Update()
    {
        if (audioSource == null || audioSource.clip == null) return;

        // === 1. 자동 캘리브레이션 구간 ===
        if (isCalibrating)
        {
            calibrationTimer += Time.deltaTime;

            if (calibrationTimer >= measurementInterval)
            {
                calibrationTimer -= measurementInterval;

                // 현재 프레임의 dBA 측정
                if (TryGetCurrentDbA(out float dbA_raw))
                {
                    calibrationDbSamples.Add(dbA_raw);
                }
            }

            if (Time.time - calibrationStartTime >= calibrationDuration)
            {
                isCalibrating = false;
                calibrationDone = true;

                if (calibrationDbSamples.Count > 0)
                {
                    float sum = 0f;
                    foreach (var v in calibrationDbSamples) sum += v;
                    float avg = sum / calibrationDbSamples.Count;

                    // 주변 소음 평균을 0 dBA 근처로 보정
                    calibrationOffsetDb = -avg;
                    Debug.Log($"[Calibration] 완료. 평균 주변 소음: {avg:F2} dBA, Offset: {calibrationOffsetDb:F2} dB");
                }
                else
                {
                    calibrationOffsetDb = 0f;
                    Debug.Log("[Calibration] 샘플이 없어 Offset 0으로 설정");
                }
            }
        }

        // === 2. 기존 5초 녹음 로직 ===
        if (isRecording)
        {
            if (Time.time - recordStartTime > 5f) // 5초 동안만 측정
            {
                isRecording = false; // 녹음 상태 종료
                return;
            }

            measurementTimer += Time.deltaTime; // 타이머 업데이트

            if (measurementTimer >= measurementInterval)
            {
                measurementTimer -= measurementInterval;

                // 현재 프레임의 dBA 측정
                if (!TryGetCurrentDbA(out float dbA_raw))
                    return;

                // === 자동 보정 적용 ===
                float dbA = dbA_raw + calibrationOffsetDb - 10f;

                if (dbA < 0f) dbA = 0f;

                // 데시벨 값 리스트에 추가
                dbValues.Add(dbA);
                dbVisualizer.UpdateDbVisualization(dbA); // 시각화 업데이트
                Debug.Log("[" + dbValues.Count + "] dBA (calibrated): " + dbA);

                if (dbValues.Count > 99)
                    dbVisualizer.SetZero(); // 시각화 초기화
            }
        }
    }

    /// <summary>
    /// 마이크 버퍼에서 현재 프레임의 A-weighted dBA를 계산
    /// </summary>
    bool TryGetCurrentDbA(out float dbA)
    {
        dbA = 0f;

        if (audioSource == null || audioSource.clip == null)
            return false;

        int position = Microphone.GetPosition(microphoneName); // null 대신 장치명 사용
        if (position <= 0)
            return false;

        // 안전한 랩어라운드 읽기: bufferSize 만큼 항상 채우도록 함
        if (buffer == null || buffer.Length < bufferSize)
        {
            Debug.LogError("Buffer size is too small");
            return false;
        }

        int clipSamples = audioSource.clip.samples;
        int startPosition = position - bufferSize;
        if (startPosition >= 0)
        {
            audioSource.clip.GetData(buffer, startPosition);
        }
        else
        {
            // tail + head로 나누어 읽기
            int tailStart = clipSamples + startPosition; // startPosition가 음수
            int tailCount = Mathf.Max(0, clipSamples - tailStart);
            if (tailCount > 0)
            {
                var tailBuf = new float[tailCount];
                audioSource.clip.GetData(tailBuf, tailStart);
                System.Array.Copy(tailBuf, 0, buffer, 0, tailCount);
            }

            int headCount = bufferSize - Mathf.Min(bufferSize, tailCount);
            if (headCount > 0)
            {
                var headBuf = new float[headCount];
                audioSource.clip.GetData(headBuf, 0);
                System.Array.Copy(headBuf, 0, buffer, Mathf.Max(0, tailCount), headCount);
            }
        }

        // 스펙트럼 얻기
        float[] spectrum = new float[bufferSize];
        audioSource.GetSpectrumData(spectrum, 0, fftWindow);

        // 최대 진폭 주파수 계산 (0번 DC 제외 가능)
        float maxFrequency = 0f;
        float maxAmplitude = 0f;
        for (int i = 1; i < spectrum.Length; i++)
        {
            float amplitude = spectrum[i];
            if (amplitude > maxAmplitude)
            {
                maxAmplitude = amplitude;
                maxFrequency = i * sampleRate / (float)bufferSize;
            }
        }

        if (maxFrequency < 0f)
            maxFrequency = 0f;

        // 음압 레벨 계산 (평균 절대값). 0으로 인해 로그가 무한대가 되지 않도록 하한 적용
        float pressure = 0f;
        for (int i = 0; i < bufferSize; i++)
        {
            pressure += Mathf.Abs(buffer[i]);
        }
        pressure /= bufferSize;
        pressure = Mathf.Max(pressure, 1e-20f); // 안전 하한

        // 압력을 데시벨로 변환
        float db = 20f * Mathf.Log10(pressure / REFERENCE);

        // A-weighting 계산 (안전한 Math)
        float f = Mathf.Max(1e-6f, maxFrequency);
        float f2 = f * f;
        float Ra = (12194f * 12194f * f2 * f2)
            / ((f2 + 20.6f * 20.6f) * Mathf.Sqrt((f2 + 107.7f * 107.7f) * (f2 + 737.9f * 737.9f)) * (f2 + 12194f * 12194f));

        float weight = 20f * Mathf.Log10(Mathf.Max(Ra, 1e-20f)) + 2.0f;
        if (weight < -50f) weight = 0f;

        // A-weighted 데시벨
        dbA = db + weight;
        if (float.IsNaN(dbA) || float.IsInfinity(dbA))
            return false;

        if (dbA < 0f) dbA = 0f;
        return true;
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
        dbVisualizer.SetZero(); // 시각화 초기화
        return dbValues;
    }

    public void StartRecording()
    {
        Debug.Log("StartRecording");
        Debug.Log(isRecording);
        Debug.Log(Microphone.IsRecording(microphoneName));

        
        Debug.Log("isRecording -> true");
        isRecording = true;
        recordStartTime = Time.time;
        measurementTimer = 0f; // 타이머 초기화
        dbValues.Clear(); // 기존 측정값 초기화
    }
}
