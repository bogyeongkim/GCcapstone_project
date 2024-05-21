using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class SoundAnalyzer2 : MonoBehaviour
{
    const float REFERENCE = 0.00002f; // ���۷��� ��

    public string microphoneName; // ����� ����ũ �̸�
    public int sampleRate = 44100; // ���ø� �ӵ�
    public int bufferSize = 2048; // ���� ũ��
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris; // FFT â �Լ�


    //******************************************************��� �߰�
    public float warningThreshold = 70f;
    public GameObject warningUI;
    public Image screenOverlay;
    private bool isWarningActive = false;

    public bool isRecording = false; // ���� ���� Ȯ�� flag
    float recordStartTime = 0f; // ���� ���� �ð�

    float[] buffer; // ����� ����
    public List<float> dbValues = new List<float>();//���ú� �� ���� ����Ʈ

    private AudioSource audioSource;

    private float timer = 0f;
    public float measurementInterval = 0.05f; // ���� ���� (��)

    void Start()
    {
        string[] devices = Microphone.devices;
        if (devices.Length > 0)
        {
            microphoneName = devices[0]; // ù ��° ����ũ ����
            audioSource = GetComponent<AudioSource>();
            audioSource.mute = true; // 오디오 소스 음소거
            StartCoroutine(SetupMicrophone());
        }
        else
        {
            UnityEngine.Debug.LogError("����ũ�� ã�� �� �����ϴ�.");
        }
    }

    IEnumerator SetupMicrophone()
    {
        audioSource.clip = Microphone.Start(microphoneName, true, 1, sampleRate);
        audioSource.loop = true;
        // ����ũ�� �غ�� ������ ��ٸ�
        yield return new WaitUntil(() => Microphone.GetPosition(microphoneName) > 0);
        audioSource.Play();
        buffer = new float[bufferSize];
    }

    void Update()
    {
        if (isRecording)
        {
            if (Time.time - recordStartTime > 5f) // 5�� ���ȸ� ����
            {
                isRecording = false; // ���� ���� ����
                return;
            }

            timer += Time.deltaTime; // Ÿ�̸� ������Ʈ

            if (timer >= measurementInterval)
            {
                timer -= measurementInterval;

                // ����� ������ �б�
                AudioSource audioSource = GetComponent<AudioSource>();
                int position = Microphone.GetPosition(null);


                int startPosition = position - bufferSize;
                if (startPosition < 0) startPosition = 0;

                if (buffer.Length < bufferSize)
                {
                    UnityEngine.Debug.LogError("Buffer size is too small");
                    return;
                }

                if (position > audioSource.clip.samples)
                {
                    UnityEngine.Debug.LogError("Position out of range");
                    return;
                }


                audioSource.clip.GetData(buffer, startPosition);

                float[] spectrum = new float[bufferSize];
                audioSource.GetSpectrumData(spectrum, 0, fftWindow);

                float maxFrequency = 0f;
                float maxAmplitude = 0f;

                // �ִ� ���� ������ ���ļ� ����
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


                //���ļ� ���
                //UnityEngine.Debug.Log("���ļ�: " + maxFrequency.ToString("F2") + " Hz");

                // ���� ���� ���
                float pressure = 0f;
                for (int i = 0; i < bufferSize; i++)
                {
                    pressure += Mathf.Abs(buffer[i]);
                }
                pressure /= bufferSize;

                // �з��� ���ú��� ��ȯ
                float db = 20 * Mathf.Log10(pressure / REFERENCE);


                float Ra = (Mathf.Pow(12194f, 2) * Mathf.Pow(maxFrequency, 4))
                    / ((Mathf.Pow(maxFrequency, 2) + Mathf.Pow(20.6f, 2))
                    * Mathf.Sqrt((Mathf.Pow(maxFrequency, 2) + Mathf.Pow(107.7f, 2))
                    * (Mathf.Pow(maxFrequency, 2) + Mathf.Pow(737.9f, 2)))
                    * (Mathf.Pow(maxFrequency, 2) + Mathf.Pow(12194f, 2)));

                float weight = 20 * Mathf.Log10(Ra) + 2.0f;
                if (weight < -50f) weight = 0;

                // A-weighted ���ú� ���� ���� ���ú� ���� ����
                float dbA = db + weight;

                if (dbA < 0f) dbA = 0f;

                // ���ú� �� ����Ʈ�� �߰�
                dbValues.Add(dbA);

                UnityEngine.Debug.Log("[" + dbValues.Count + "]" + "dBA : " + dbA);

                //*************************************************************��� �߰�
                if (dbA > warningThreshold)
                {
                    TriggerWarning();
                }
            }
        }
    }

    /*
    void Start()
    {
        // ����ũ ����̽� ����
        string[] devices = Microphone.devices;
        if (devices.Length > 0)
        {
            microphoneName = devices[0]; // ù ��° ����ũ ����
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = Microphone.Start(microphoneName, true, 1, sampleRate);
            audioSource.loop = true;
            while (!(Microphone.GetPosition(null) > 0)) { } // ����ũ ���۱��� ���
            audioSource.Play();
            buffer = new float[bufferSize];
        }
        else
        {
            UnityEngine.Debug.LogError("����ũ�� ã�� �� �����ϴ�.");
        }
    }
    
    void Update()
    {
        if (isRecording)
        {
            if (Time.time - recordStartTime > 5f) // 5�� ���ȸ� ����
            {
                isRecording = false; // ���� ���� ����
                return;
            }

            timer += Time.deltaTime; // Ÿ�̸� ������Ʈ

            if (timer >= measurementInterval)
            {
                timer -= measurementInterval;

                // ����� ������ �б�
                AudioSource audioSource = GetComponent<AudioSource>();
                int position = Microphone.GetPosition(null);
                audioSource.clip.GetData(buffer, position);

                float[] spectrum = new float[bufferSize];
                audioSource.GetSpectrumData(spectrum, 0, fftWindow);

                float maxFrequency = 0f;
                float maxAmplitude = 0f;

                // �ִ� ���� ������ ���ļ� ����
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


                //���ļ� ���
                //UnityEngine.Debug.Log("���ļ�: " + maxFrequency.ToString("F2") + " Hz");

                // ���� ���� ���
                float pressure = 0f;
                for (int i = 0; i < bufferSize; i++)
                {
                    pressure += Mathf.Abs(buffer[i]);
                }
                pressure /= bufferSize;

                // �з��� ���ú��� ��ȯ
                float db = 20 * Mathf.Log10(pressure / REFERENCE);


                float Ra = (Mathf.Pow(12194f, 2) * Mathf.Pow(maxFrequency, 4))
                    / ((Mathf.Pow(maxFrequency, 2) + Mathf.Pow(20.6f, 2))
                    * Mathf.Sqrt((Mathf.Pow(maxFrequency, 2) + Mathf.Pow(107.7f, 2))
                    * (Mathf.Pow(maxFrequency, 2) + Mathf.Pow(737.9f, 2)))
                    * (Mathf.Pow(maxFrequency, 2) + Mathf.Pow(12194f, 2)));

                float weight = 20 * Mathf.Log10(Ra) + 2.0f;
                if (weight < -50f) weight = 0;

                // A-weighted ���ú� ���� ���� ���ú� ���� ����
                float dbA = db + weight;

                if (dbA < 0f) dbA = 0f;

                // ���ú� �� ����Ʈ�� �߰�
                dbValues.Add(dbA);

                UnityEngine.Debug.Log("[" + dbValues.Count + "]" + "dBA : " + dbA);

                //*************************************************************��� �߰�
                if (dbA > warningThreshold)
                {
                    TriggerWarning();
                }
            }
        }
    }
    */

    void TriggerWarning()
    {
        //StartCoroutine(TriggerWarningUI());
        Handheld.Vibrate(); // ����������
        StartCoroutine(FlashWarning()); // ȭ�� �Ӱ� �����̵���
    }

    IEnumerator TriggerWarningUI()
    {
        if (warningUI != null)
        {
            warningUI.SetActive(true); // ��� UI ������Ʈ ǥ��
            yield return new WaitForSeconds(0.5f); // 0.5�� ���� ���
            warningUI.SetActive(false); // ��� UI ������Ʈ ��Ȱ��ȭ
        }
    }

    IEnumerator FlashWarning()
    {
        if (isWarningActive) // ȭ�� �����̴� ������ Ȯ��
        {
            yield break;
        }

        isWarningActive = true; // ��� ���� Ȱ��ȭ

        Color originalColor = screenOverlay.color;
        Color warningColor = new Color(1, 0, 0, 0.2f);

        screenOverlay.color = warningColor; // �������� ������ ȭ������
        yield return new WaitForSeconds(0.3f);
        screenOverlay.color = originalColor;
        yield return new WaitForSeconds(0.3f);

        isWarningActive = false; // ��� ���� ��Ȱ��ȭ
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

    // ���ú� �� ����Ʈ ��ȯ
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
            dbValues.Clear(); // ���� ������ �ʱ�ȭ
        }
    }
}