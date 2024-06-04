using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testing : MonoBehaviour
{
    public GameObject book;
    public GameObject drink;

    // Start is called before the first frame update
    void Start()
    {
        ScoreManager.instance.AddItem(book);
        ScoreManager.instance.AddItem(drink);
        ScoreManager.instance.AddScore2(1,3);
        ScoreManager.instance.AddScore2(2,1);
        ScoreManager.instance.AddScore2(3,1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
