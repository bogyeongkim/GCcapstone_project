using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemtest : MonoBehaviour
{
    public GameObject test1;
    public GameObject test2;

    // Start is called before the first frame update
    void Start()
    {
        ScoreManager.instance.AddItem(test1);
        ScoreManager.instance.AddItem(test2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
