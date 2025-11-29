using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundAnalyzer2 : MonoBehaviour
{
    public DbVisualizer dbVisualizer; // DbVisualizer에 대한 참조

    const float REFERENCE = 0.00002f; // 레퍼런스 값

    public string microphoneName; // 사용할 마이크 이름
    public int sampleRate = 44100; // 샘플링 속도
    public int bufferSize = 2048; // 버퍼 크기
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris; // FFT 창 함수

    // ******************************************************
    // 경고 UI 관련 변수
    // ******************************************************
    public GameObject warningUI;
    public Image screenOverlay;
    private bool isWarningActive = false;

    public bool isRecording = false; // 녹음 상태 확인 flag
    float recordStartTime = 0f; // 녹음 시작 시간

    float[] buffer; // 오디오 버퍼
    public List<float> dbValues = new List<float>(); // 데시벨 값 저장 리스트

    private AudioSource audioSource;

    // === 측정 타이머 ===
    private float measurementTimer = 0f; // (기존 timer 변수 대체)
    public float measurementInterval = 0.05f; // 측정 간격 (초)

    // ******************************************************
    // [추가됨] 자동 캘리브레이션 관련 변수
    // ******************************************************
    [Header("Auto Calibration Settings")]
    public bool enableAutoCalibration = true;
    public float calibrationDuration = 5f; // 주변 소음 측정 시간(초) (SoundAnalyzer2는 보통 빠를 수 있으므로 5초 권장, 필요시 수정)

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
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.mute = true; // 마이크 소리가 스피커로 나오지 않게 함
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

        // === [추가됨] 마이크 준비 직후 자동 캘리브레이션 시작 ===
        if (enableAutoCalibration)
        {
            isCalibrating = true;
            calibrationDone = false;
            calibrationTimer = 0f;
            calibrationStartTime = Time.time;
            calibrationDbSamples.Clear();

            Debug.Log("[Calibration] 주변 소음 측정 시작 (SoundAnalyzer2)");
        }
    }

    void Update()
    {
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

                    // 주변 소음 평균을 0 dBA 근처로 보정하기 위한 오프셋 설정
                    calibrationOffsetDb = -avg;
                    Debug.Log($"[Calibration] 완료. 평균 소음: {avg:F2} dBA, 설정된 Offset: {calibrationOffsetDb:F2} dB");
                }
                else
                {
                    calibrationOffsetDb = 20f;
                    Debug.Log("[Calibration] 샘플 부족으로 Offset 20 설정");
                }
            }
        }

        // =================================================
        // 2. 녹음 및 경고 로직 (Recording Phase)
        // =================================================
        if (isRecording)
        {
            if (Time.time - recordStartTime > 5f) // 5초 동안만 측정
            {
                isRecording = false; // 녹음 상태 종료
                return;
            }

            measurementTimer += Time.deltaTime;

            if (measurementTimer >= measurementInterval)
            {
                measurementTimer -= measurementInterval;

                // 현재 프레임의 dBA 측정
                if (!TryGetCurrentDbA(out float dbA_raw))
                    return;


                float dbA = dbA_raw + calibrationOffsetDb + 5;
                if (dbA_raw < -calibrationOffsetDb + 8f) dbA = 0f;

                // 데시벨 값 리스트에 추가
                dbValues.Add(dbA);
                dbVisualizer.UpdateDbVisualization(dbA); // 시각화 업데이트

                // 로그 출력
                Debug.Log("[" + dbValues.Count + "] dBA (calibrated): " + dbA +"  calibrated =" + calibrationOffsetDb);

                /* === 경고 로직 (보정된 값 기준) ===
                if (dbA > 55f)
                {
                    TriggerWarning();
                }
                */
            }
        }
    }

    /// <summary>
    /// [추가됨] 마이크 버퍼에서 현재 프레임의 A-weighted dBA를 계산하는 헬퍼 함수
    /// SoundAnalyzer.cs의 로직을 가져와 안정성을 높임 (Tail/Head 읽기 처리 포함)
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
            Debug.LogError("Buffer size is too small");
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
            // 버퍼가 처음으로 돌아가는 경우 (Wrap around)
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

        // A-Weighting 계산
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

    /*
    void TriggerWarning()
    {
        Handheld.Vibrate(); // 진동
        StartCoroutine(FlashWarning()); // 화면 깜빡임
    }
    */

    IEnumerator FlashWarning()
    {
        if (isWarningActive) yield break;

        isWarningActive = true;

        Color originalColor = screenOverlay.color;
        Color warningColor = new Color(1, 0, 0, 0.2f);

        screenOverlay.color = warningColor;
        yield return new WaitForSeconds(0.3f);
        screenOverlay.color = originalColor;
        yield return new WaitForSeconds(0.3f);

        isWarningActive = false;
    }

    public void ClearWarning()
    {
        if (warningUI != null)
        {
            warningUI.SetActive(false);
        }

        if (screenOverlay != null)
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

    public List<float> GetDbValues()
    {
        dbVisualizer.SetZero();
        return dbValues;
    }

    public void StartRecording()
    {
        Debug.Log("StartRecording (SoundAnalyzer2)");

        if (isRecording == false)
        {
            isRecording = true;
            recordStartTime = Time.time;
            measurementTimer = 0f;
            dbValues.Clear();
        }
    }
}