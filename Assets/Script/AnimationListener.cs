using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationListener : MonoBehaviour
{

    void Hide()
    {
        gameObject.SetActive(false);
    }

    void HideSpin()
    {
        transform.Find("mini").gameObject.SetActive(false);
        transform.Find("zero").gameObject.SetActive(false);
        transform.Find("single").gameObject.SetActive(false);
        transform.Find("double").gameObject.SetActive(false);
        transform.Find("triple").gameObject.SetActive(false);
    }


}
