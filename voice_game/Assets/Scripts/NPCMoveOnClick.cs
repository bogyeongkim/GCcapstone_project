using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class NPCMoveOnClick : MonoBehaviour
{
    public Transform player; // �÷��̾��� Transform, Inspector���� �Ҵ�
    public float moveSpeed = 5f; // NPC �̵� �ӵ�
    private bool isMoving = false; // NPC ������ ���� üũ

    public Animator playerAnimator; // �÷��̾� �ִϸ��̼� ����

    private SoundAnalyzer soundAnalyzer;

    List<float> DBL = new List<float>();

    public GameObject text01;
    public GameObject text02;
    public GameObject microphone_icon;
    public GameObject fairy;
    public GameObject loud01;
    public GameObject arrow02;


    void Start()
    {
        // SoundAnalyzer ������Ʈ ã��
        soundAnalyzer = FindObjectOfType<SoundAnalyzer>();
        if (soundAnalyzer == null)
        {
            UnityEngine.Debug.LogError("SoundAnalyzer ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToPlayer();
        }
    }

    // NPC Ŭ���� ȣ��Ǵ� �޼ҵ�
    private void OnMouseDown()
    {
        isMoving = true;
        UnityEngine.Debug.Log("click");
        arrow02.GetComponent<Renderer>().enabled = false;
        text01.GetComponent<Renderer>().enabled = false;
        fairy.GetComponent<Renderer>().enabled = false;
    }

    // �÷��̾� ��ġ ��ó�� NPC �̵�
    void MoveToPlayer()
    {
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.position, step);

        // �÷��̾� run
        playerAnimator.SetBool("isRun",true);

        // NPC�� �÷��̾� ��ó�� �����ߴ��� Ȯ��
        if (Vector3.Distance(transform.position, player.position) < 3.0f)
        {
            isMoving = false;
            playerAnimator.SetBool("isRun", false);
            // �̵� ������ ��ȭ ����
            TriggerDialogue();
        }
    }

    void TriggerDialogue()
    {
        // "~å ����־��?" ȭ�鿡 �ؽ�Ʈ ����
        text02.GetComponent<Renderer>().enabled = true;
        microphone_icon.GetComponent<Renderer>().enabled = true;
        // ����ũ �Է� �ް� ���ú� �� �޾ƿ���
        UnityEngine.Debug.Log("trigger");
        soundAnalyzer.StartRecording();
        StartCoroutine(WaitAndReceiveDbValues()); // �ڷ�ƾ ����
    }

    IEnumerator WaitAndReceiveDbValues()
    {
        yield return new WaitForSeconds(5); // 5�� ��ٸ�

        if (!soundAnalyzer.isRecording) // ���� �������� Ȯ��
        {
            DBL = soundAnalyzer.GetDbValues();
            UnityEngine.Debug.Log("getdb");

            /* ���ú� ���� ���� ó�� �ʿ�
             * ��Ҹ� Ŭ ��() : ������������ ������ ���ؾ���. & �亯 & ���� �ܰ�
             * ��Ҹ� ���� ��() : �� �� ���. �ٽ� �����ٷ�? & ��õ�
             * ��Ҹ� ������ ��() : �亯 & ���� �ܰ�
             */
            foreach (float dbValue in DBL)
            {
                if (dbValue > 40) // 40���� ū ���� �ִ� ���
                {
                    TriggerEvent_loud(); // �̺�Ʈ�� �߻���Ŵ
                    break; // �ϳ��� ������ �����ϴ� ��� �߰� �˻� ���� loop ����
                }
            }
        }
        else
        {
            // ���� ������ -> ���� ����
            UnityEngine.Debug.Log("Recording is still in progress.");
        }
    }

    void TriggerEvent_loud()
    {
        text02.GetComponent<Renderer>().enabled = false;
        microphone_icon.GetComponent<Renderer>().enabled = false;
        loud01.GetComponent<Renderer>().enabled = true;
    }
}