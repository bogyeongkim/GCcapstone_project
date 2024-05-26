using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class reaction1 : MonoBehaviour
{

    public GameObject bubble;
    public GameObject fairy;
    public GameObject blank;


    public TextMeshProUGUI dragon_line1;
    public TextMeshProUGUI dragon_line2;
    public TextMeshProUGUI dragon_line3;
    public TextMeshProUGUI fairy1;
    public GameObject blank_image;
    public GameObject mainmenu_button;
    public GameObject quit_button;


    public AudioSource a_dragon_line1;
    public AudioSource a_dragon_line2;
    public AudioSource a_dragon_line3;
    public AudioSource a_fairy1;
    public AudioSource bgm_next;
    public AudioSource bgm;


    public Animator dragonAnimator;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayDragonLine());
    }

    IEnumerator PlayDragonLine()
    {
        bgm.Stop();

        yield return new WaitForSeconds(0.5f);

        bgm_next.Play();

        // "이게 뭐야? 내 마음에 드는 게 없잖아!"
        // "흠.. 마음에 썩 들진 않지만 한 번 봐주겠어"
        // "내가 좋아하는 것들이잖아!"
        bubble.GetComponent<Renderer>().enabled = true;
        dragon_line1.enabled = true;
        a_dragon_line1.Play();

        yield return new WaitForSeconds(a_dragon_line1.clip.length + 1.0f);

        // "너도 목소리 조절을 못하는 녀석이구나!"
        // "공공장소에선 목소리 조절을 더 잘하도록 해"
        // "네 덕분에 마음이 풀렸어"
        dragon_line1.enabled = false;
        dragon_line2.enabled = true;
        a_dragon_line2.Play();

        yield return new WaitForSeconds(a_dragon_line2.clip.length + 1.0f);

        if (gameObject.name == "reaction1")
        {
            dragonAnimator.SetTrigger("Attack");
        }

        if (gameObject.name == "reaction1" || gameObject.name == "reaction3")
        {
            // "화해하기 싫어! 돌아가!"
            // "앞으로도 목소리 조절을 잘해주면 좋겠어"
            dragon_line2.enabled = false;
            dragon_line3.enabled = true;
            a_dragon_line3.Play();

            yield return new WaitForSeconds(a_dragon_line3.clip.length + 1.0f);
        }

        bubble.GetComponent<Renderer>().enabled = false;
        dragon_line3.enabled = false;

        yield return new WaitForSeconds(2.0f);

        blank_image.GetComponent<Renderer>().enabled = true;
        mainmenu_button.SetActive(true);
        quit_button.SetActive(true);
    }
}
