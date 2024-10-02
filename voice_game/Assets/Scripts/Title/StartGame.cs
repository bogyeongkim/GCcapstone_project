using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    private bool start = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnableStart(4f)); 
    }

    IEnumerator EnableStart(float delay)
    {
        yield return new WaitForSeconds(delay);

        start = true;

        UnityEngine.Debug.Log("let's start");
    }

    // Update is called once per frame
    void Update()
    {
        if(start)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                LoadNextScene();
            }
            else if (Input.GetMouseButtonDown(0))
            {
                LoadNextScene();
            }
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("Menu");
    }
}
