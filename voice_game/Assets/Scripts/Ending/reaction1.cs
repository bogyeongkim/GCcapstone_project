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

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayDragonLine());
    }

    IEnumerator PlayDragonLine()
    {
        bgm.Stop();

        yield return new WaitForSeconds(1.5f);

        bgm_next.Play();

        // "이게 뭐야? 내 마음에 드는 게 없잖아!"
        // "흠.. 마음에 썩 들진 않지만 한 번 봐주겠어"
        // "내가 좋아하는 것들이잖아!"
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

            Dictionary<int, int> allStageScores = ScoreManager.instance.GetAllStageScores();

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

        // "너도 목소리 조절을 못하는 녀석이구나!"
        // "공공장소에선 목소리 조절을 더 잘하도록 해"
        // "네 덕분에 마음이 풀렸어"
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
            // "화해하기 싫어! 돌아가!"
            // "앞으로도 목소리 조절을 잘해주면 좋겠어"
            
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
                UnityEngine.Debug.LogError("SortingGroup 컴포넌트가 없습니다.");
        }

        blank_image.GetComponent<Renderer>().enabled = true;
        mainmenu_button.SetActive(true);
        quit_button.SetActive(true);
    }
}
