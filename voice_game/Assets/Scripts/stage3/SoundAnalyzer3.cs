using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SoundAnalyzer3 : MonoBehaviour
{
    public DbVisualizer dbVisualizer; // DbVisualizer에 대한 참조

    const float REFERENCE = 0.00002f; // 레퍼런스 값

    public string microphoneName; // 사용할 마이크 이름
    public int sampleRate = 44100; // 샘플링 속도
    public int bufferSize = 2048; // 버퍼 크기
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris; // FFT 창 함수

    private float[] buffer; // 오디오 버퍼
    private List<float> dbValues = new List<float>(); // 데시벨 값 저장 리스트

    private AudioSource audioSource;

    private float timer = 0f;
    public float measurementInterval = 0.05f; // 측정 간격 (초)

    // ******************************************************
    // [추가됨] 자동 캘리브레이션 관련 변수
    // ******************************************************
    [Header("Auto Calibration Settings")]
    public bool enableAutoCalibration = true;
    public float calibrationDuration = 3f; // 주변 소음 측정 시간(초)

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
            microphoneName = devices[0]; // 첫 번째 마이크 선택
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.mute = true; // 오디오 소스 음소거 (하울링 방지)

            StartCoroutine(SetupMicrophone());
        }
        else
        {
            UnityEngine.Debug.LogError("마이크를 찾을 수 없습니다.");
        }
    }

    IEnumerator SetupMicrophone()
    {
        audioSource.clip = Microphone.Start(microphoneName, true, 1, sampleRate);
        audioSource.loop = true;

        // 마이크가 준비될 때까지 기다림 (SoundAnalyzer2 방식 적용 - 안전함)
        yield return new WaitUntil(() => Microphone.GetPosition(microphoneName) > 0);

        audioSource.Play();
        buffer = new float[bufferSize];

        // === [추가됨] 마이크 준비 직후 자동 캘리브레이션 시작 ===
        if (enableAutoCalibration)
        {
            isCalibrating = true;
            calibrationDone = false;
            calibrationTimer = 0f;
            calibrationStartTime = Time.time;
            calibrationDbSamples.Clear();

            UnityEngine.Debug.Log("[Calibration] 주변 소음 측정 시작 (SoundAnalyzer3)");
        }
    }

    void Update()
    {
        // 게임이 종료되었는지 확인 (SoundAnalyzer3 고유 로직 유지)
        if (GameManager.Instance.isGameEnd)
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
                Microphone.End(microphoneName);
            }
            return;
        }

        if (audioSource == null || audioSource.clip == null) return;

        // =================================================
        // 1. 자동 캘리브레이션 구간 (Calibration Phase)
        // =================================================
        if (isCalibrating)
        {
            calibrationTimer += Time.deltaTime;

            if (calibrationTimer >= measurementInterval)
            {
                calibrationTimer -= measurementInterval;

                // 현재 프레임의 Raw dBA 측정
                if (TryGetCurrentDbA(out float dbA_raw))
                {
                    calibrationDbSamples.Add(dbA_raw);
                }
            }

            // 설정한 시간이 지나면 캘리브레이션 종료 및 오프셋 계산
            if (Time.time - calibrationStartTime >= calibrationDuration)
            {
                isCalibrating = false;
                calibrationDone = true;

                if (calibrationDbSamples.Count > 0)
                {
                    float sum = 0f;
                    foreach (var v in calibrationDbSamples) sum += v;
                    float avg = sum / calibrationDbSamples.Count;

                    // 주변 소음 평균을 0 dBA로 맞추기 위한 오프셋 설정
                    calibrationOffsetDb = -avg;
                    UnityEngine.Debug.Log($"[Calibration] 완료. 평균 소음: {avg:F2} dBA, 설정된 Offset: {calibrationOffsetDb:F2} dB");
                }
                else
                {
                    calibrationOffsetDb = 0f;
                    UnityEngine.Debug.Log("[Calibration] 샘플 부족으로 Offset 0 설정");
                }
            }
            return; // 캘리브레이션 중에는 아래 일반 측정 로직 실행 안 함
        }

        // =================================================
        // 2. 일반 측정 로직 (Normal Phase)
        // =================================================
        timer += Time.deltaTime;

        if (timer >= measurementInterval)
        {
            timer -= measurementInterval;

            // 안전한 dBA 계산 함수 호출
            if (!TryGetCurrentDbA(out float dbA_raw))
                return;

            // === [보완됨] 자동 보정값 적용 ===
            float dbA = dbA_raw; //+ calibrationOffsetDb;
            dbA = dbA + calibrationOffsetDb/2;

            if (dbA < 25f) dbA = 0f;
            
            // 데시벨 값 리스트에 추가
            dbValues.Add(dbA);

            if (dbVisualizer != null)
                dbVisualizer.UpdateDbVisualization(dbA); // 시각화 업데이트

            UnityEngine.Debug.Log($"    dBA : {dbA:F2} (Raw: {dbA_raw:F2}, Offset: {calibrationOffsetDb:F2})");

            // if (dbValues.Count >= 100) GetDbValues(); // 필요 시 사용
        }
    }

    /// <summary>
    /// [추가됨] 마이크 버퍼에서 현재 프레임의 A-weighted dBA를 계산하는 헬퍼 함수
    /// SoundAnalyzer2와 동일한 로직을 사용하여 안정성을 확보
    /// </summary>
    bool TryGetCurrentDbA(out float dbA)
    {
        dbA = 0f;

        if (audioSource == null || audioSource.clip == null) return false;

        int position = Microphone.GetPosition(microphoneName);
        if (position <= 0) return false;

        // 버퍼 크기 체크
        if (buffer == null || buffer.Length < bufferSize)
        {
            UnityEngine.Debug.LogError("Buffer size is too small");
            return false;
        }

        // 안전한 데이터 읽기 (Ring Buffer 처리)
        int clipSamples = audioSource.clip.samples;
        int startPosition = position - bufferSize;

        if (startPosition >= 0)
        {
            audioSource.clip.GetData(buffer, startPosition);
        }
        else
        {
            // 랩어라운드 처리
            int tailStart = clipSamples + startPosition;
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

        // FFT 및 스펙트럼 분석
        float[] spectrum = new float[bufferSize];
        audioSource.GetSpectrumData(spectrum, 0, fftWindow);

        float maxFrequency = 0f;
        float maxAmplitude = 0f;
        for (int i = 1; i < spectrum.Length; i++) // DC(0Hz) 제외
        {
            if (spectrum[i] > maxAmplitude)
            {
                maxAmplitude = spectrum[i];
                maxFrequency = i * sampleRate / (float)bufferSize;
            }
        }

        // 음압(Pressure) 계산
        float pressure = 0f;
        for (int i = 0; i < bufferSize; i++)
        {
            pressure += Mathf.Abs(buffer[i]);
        }
        pressure /= bufferSize;
        pressure = Mathf.Max(pressure, 1e-20f); // 0 나누기 방지

        // dB 변환
        float db = 20f * Mathf.Log10(pressure / REFERENCE);

        // A-Weighting 계산 (SoundAnalyzer2의 최적화된 계산식 사용)
        float f = Mathf.Max(1e-6f, maxFrequency);
        float f2 = f * f;
        float Ra = (12194f * 12194f * f2 * f2)
            / ((f2 + 20.6f * 20.6f) * Mathf.Sqrt((f2 + 107.7f * 107.7f) * (f2 + 737.9f * 737.9f)) * (f2 + 12194f * 12194f));

        float weight = 20f * Mathf.Log10(Mathf.Max(Ra, 1e-20f)) + 2.0f;
        if (weight < -50f) weight = 0f;

        dbA = db + weight;

        if (float.IsNaN(dbA) || float.IsInfinity(dbA)) return false;
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

    public List<float> GetDbValues()
    {
        return dbValues;
    }
}