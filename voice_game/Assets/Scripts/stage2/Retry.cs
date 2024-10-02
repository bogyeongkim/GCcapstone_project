using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Retry : MonoBehaviour
{
    public GameObject fairy;
    public GameObject blank;
    public GameObject blank_image;
    public TextMeshProUGUI fairy1;
    public TextMeshProUGUI retry;
    public TextMeshProUGUI cafe_board;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        fairy.GetComponent<Renderer>().enabled = false;
        blank.GetComponent<Renderer>().enabled = false;
        blank_image.GetComponent<Renderer>().enabled = false;
        fairy1.GetComponent<TextMeshProUGUI>().enabled = false;
        retry.GetComponent<TextMeshProUGUI>().enabled = false;
        cafe_board.GetComponent<TextMeshProUGUI>().enabled = true;
        gameObject.SetActive(false);
    }
}
