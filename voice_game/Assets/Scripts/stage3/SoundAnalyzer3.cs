using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.IO;

public class SoundAnalyzer3 : MonoBehaviour
{
    const float REFERENCE = 0.00002f; // ??????? ??

    public string microphoneName; // ????? ????? ???
    public int sampleRate = 44100; // ???��? ???
    public int bufferSize = 2048; // ???? ???
    public FFTWindow fftWindow = FFTWindow.BlackmanHarris; // FFT ? ???

    float[] buffer; // ????? ????
    private List<float> dbValues = new List<float>();//????? ?? ???? ?????

    private AudioSource audioSource;

    private float timer = 0f;
    public float measurementInterval = 0.05f; // ???? ???? (??)

    void Start()
    {
        string[] devices = Microphone.devices;
        if (devices.Length > 0)
        {
            microphoneName = devices[0]; // 첫 번째 마이크 선택
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = Microphone.Start(microphoneName, true, 1, sampleRate);
            audioSource.loop = true;
            //audioSource.mute = true; // ????? ???????? ????
            while (!(Microphone.GetPosition(null) > 0)) { } // ????? ??????? ???
            audioSource.Play();
            buffer = new float[bufferSize];
            audioSource.mute = true; // 오디오 소스 음소거
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
        // 마이크가 준비될 때까지 기다림
        yield return new WaitUntil(() => Microphone.GetPosition(microphoneName) > 0);
        audioSource.Play();
        buffer = new float[bufferSize];
    }

    void Update()
    {
        // 게임이 종료되었는지 확인
        if (GameManager.Instance.isGameEnd) 
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                Microphone.End(microphoneName);
            }
            return;
        }

        timer += Time.deltaTime; // ???? ???????

        if (timer >= measurementInterval)
        {
            timer -= measurementInterval;

            // ????? ?????? ?��?
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

            // ??? ???? ?????? ????? ????
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


            //????? ???
            //UnityEngine.Debug.Log("?????: " + maxFrequency.ToString("F2") + " Hz");

            // ???? ???? ???
            float pressure = 0f;
            for (int i = 0; i < bufferSize; i++)
            {
                pressure += Mathf.Abs(buffer[i]);
            }
            pressure /= bufferSize;

            // ?��??? ??????? ???
            float db = 20 * Mathf.Log10(pressure / REFERENCE);
            

            float Ra = (Mathf.Pow(12194f, 2) * Mathf.Pow(maxFrequency, 4)) 
                / ((Mathf.Pow(maxFrequency, 2) + Mathf.Pow(20.6f, 2)) 
                * Mathf.Sqrt((Mathf.Pow(maxFrequency, 2) + Mathf.Pow(107.7f, 2)) 
                * (Mathf.Pow(maxFrequency, 2) + Mathf.Pow(737.9f, 2))) 
                * (Mathf.Pow(maxFrequency, 2) + Mathf.Pow(12194f, 2)));
            
            float weight = 20 * Mathf.Log10(Ra) + 2.0f; 
            if (weight < -50f) weight = 0;

            // A-weighted ????? ???? ???? ????? ???? ????
            float dbA = db + weight;

            if (dbA < 0f) dbA = 0f;

            // ????? ?? ??????? ???
            dbValues.Add(dbA);
            
            UnityEngine.Debug.Log("    dBA : " + dbA);

            //UnityEngine.Debug.Log("dB : " + db);

            if (dbValues.Count >= 100) GetDbValues();
        }
    }

    void OnDisable()
    {
        if (audioSource != null && Microphone.IsRecording(microphoneName))
        {
            audioSource.Stop();
            Microphone.End(microphoneName);
        }
    }

    
    // ????? ?? ????? ???
    public List<float> GetDbValues()
    {
        return dbValues;
    }
    
}