using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Disable_Rendering : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Image image = GetComponent<Image>();
        if (image != null)
        {
            image.enabled = false;
        }
    }
}
