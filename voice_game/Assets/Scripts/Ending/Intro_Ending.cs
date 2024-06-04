using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class Intro_Ending : MonoBehaviour
{
    public GameObject fairy;
    public GameObject blank;

    public TextMeshProUGUI fairy1; // 동화책과 음료를 용에게 전해주자!
    public TextMeshProUGUI fairy2; // 마음에 들면 용이 화를 풀어줄거야!

    public AudioSource a_fairy1;
    public AudioSource a_fairy2;

    public ItemTransfer itemtransferscript;

    public GameObject dragon;

    // Start is called before the first frame update
    void Start()
    {
        if (itemtransferscript != null)
        {
            itemtransferscript.enabled = false;
        }
        StartCoroutine(Fairy_audio());

        
    }

    IEnumerator Fairy_audio()
    {
        yield return new WaitForSeconds(1.5f);
        a_fairy1.Play();


        yield return new WaitForSeconds(a_fairy1.clip.length + 1.0f);
        fairy1.GetComponent<TextMeshProUGUI>().enabled = false;
        fairy2.GetComponent<TextMeshProUGUI>().enabled = true;
        a_fairy2.Play();

        yield return new WaitForSeconds(a_fairy1.clip.length + 1.0f);
        fairy.GetComponent<Renderer>().enabled = false;
        blank.GetComponent<Renderer>().enabled = false;
        fairy2.GetComponent<TextMeshProUGUI>().enabled = false;

        dragon.SetActive(true);

        if (itemtransferscript != null)
        {
            itemtransferscript.enabled = true;
        }
    }

}
