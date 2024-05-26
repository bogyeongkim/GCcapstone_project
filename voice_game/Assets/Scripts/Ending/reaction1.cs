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

        // "�̰� ����? �� ������ ��� �� ���ݾ�!"
        // "��.. ������ �� ���� ������ �� �� ���ְھ�"
        // "���� �����ϴ� �͵����ݾ�!"
        bubble.GetComponent<Renderer>().enabled = true;
        dragon_line1.enabled = true;
        a_dragon_line1.Play();

        yield return new WaitForSeconds(a_dragon_line1.clip.length + 1.0f);

        // "�ʵ� ��Ҹ� ������ ���ϴ� �༮�̱���!"
        // "������ҿ��� ��Ҹ� ������ �� ���ϵ��� ��"
        // "�� ���п� ������ Ǯ�Ⱦ�"
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
            // "ȭ���ϱ� �Ⱦ�! ���ư�!"
            // "�����ε� ��Ҹ� ������ �����ָ� ���ھ�"
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
