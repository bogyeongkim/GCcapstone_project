using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoMainmenu : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Menu"); 
    }
}
