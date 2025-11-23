using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using System.Diagnostics;

public class reaction1 : MonoBehaviour
{

    public GameObject blank_image;
    public GameObject mainmenu_button;
    public GameObject quit_button;
    public GameObject report_button;
    public GameObject bubble;

    public TextMeshProUGUI dragon_line1;
    public TextMeshProUGUI dragon_line2;
    public TextMeshProUGUI dragon_line3;
    public TextMeshProUGUI[] feedbacks;

    public GameObject dragon;

    public AudioSource a_dragon_line1;
    public AudioSource a_dragon_line2;
    public AudioSource a_dragon_line3;
    public AudioSource bgm;
    public AudioSource bgm_next;
    public AudioSource[] a_feedbacks;


    public Animator dragonAnimator;

    private Dictionary<int, int> allStageScores;

    public EndingResultUI endingResultUI;

    // Start is called before the first frame update
    void Start()
    {
        allStageScores = ScoreManager.instance.GetAllStageScores();
        StartCoroutine(PlayDragonLine());
    }

    IEnumerator PlayDragonLine()
    {
        bgm.Stop();

        yield return new WaitForSeconds(1.5f);

        bgm_next.Play();

        // "�̰� ����? �� ������ ��� �� ���ݾ�!"
        // "��.. ������ �� ���� ������ �� �� ���ְھ�"
        // "���� �����ϴ� �͵����ݾ�!"
        bubble.GetComponent<Renderer>().enabled = true;
        dragon_line1.enabled = true;
        a_dragon_line1.Play();



        if (gameObject.name == "reaction2")
        {
            yield return new WaitForSeconds(a_dragon_line1.clip.length + 1.0f);
            dragon_line1.enabled = false;

            feedbacks[0].enabled = true;
            a_feedbacks[0].Play();

            yield return new WaitForSeconds(a_feedbacks[0].clip.length + 1.0f);
            feedbacks[0].enabled = false;

            //Dictionary<int, int> allStageScores = ScoreManager.instance.GetAllStageScores();

            foreach (var stageScore in allStageScores)
            {
                int stageNumber = stageScore.Key;
                int score = stageScore.Value;

                if (score <= 1)
                {

                    feedbacks[stageNumber].enabled = true;
                    a_feedbacks[stageNumber].Play();

                    yield return new WaitForSeconds(a_feedbacks[stageNumber].clip.length + 1.0f);
                    feedbacks[stageNumber].enabled = false;
                }

            }

        }
        else
        {
            yield return new WaitForSeconds(a_dragon_line1.clip.length + 1.0f);
            dragon_line1.enabled = false;
        }

        // "�ʵ� ��Ҹ� ������ ���ϴ� �༮�̱���!"
        // "������ҿ��� ��Ҹ� ������ �� ���ϵ��� ��"
        // "�� ���п� ������ Ǯ�Ⱦ�"
        dragon_line2.enabled = true;
        a_dragon_line2.Play();

        

        yield return new WaitForSeconds(a_dragon_line2.clip.length + 1.0f);
        dragon_line2.enabled = false;

        if (gameObject.name == "reaction1")
        {
            dragonAnimator.SetTrigger("Attack");
        }

        if (gameObject.name == "reaction1" || gameObject.name == "reaction3")
        {
            // "ȭ���ϱ� �Ⱦ�! ���ư�!"
            // "�����ε� ��Ҹ� ������ �����ָ� ���ھ�"
            
            dragon_line3.enabled = true;
            a_dragon_line3.Play();

            yield return new WaitForSeconds(a_dragon_line3.clip.length + 1.0f);

            dragon_line3.enabled = false;
        }

        bubble.GetComponent<Renderer>().enabled = false;
        

        yield return new WaitForSeconds(2.0f);

        if (dragonAnimator != null)
        {
            dragonAnimator.enabled = false;
        }


        SortingGroup sortingGroup = dragon.GetComponent<SortingGroup>();

        if (sortingGroup != null)
        {
            sortingGroup.sortingOrder = 0;
        }
        else
        {
                UnityEngine.Debug.LogError("SortingGroup ������Ʈ�� �����ϴ�.");
        }
        
        

        blank_image.GetComponent<Renderer>().enabled = true;
        
        endingResultUI.Show(allStageScores);
        report_button.SetActive(true);
        mainmenu_button.SetActive(true);
        quit_button.SetActive(true);
    }
}
