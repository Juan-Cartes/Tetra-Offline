using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    public Animator dialogAnimator;
    public TextMeshProUGUI message;
    public GameObject button;

    public GameObject defaultSelect;

    public bool open;

    public void Display(string text)
    {
        this.message.text = text;

        gameObject.SetActive(true);
        dialogAnimator.SetTrigger("Open");
        open = true;
    }

    public void Display()
    {
        gameObject.SetActive(true);
        dialogAnimator.SetTrigger("Open");
        open = true;
    }

    public void Close()
    {
        open = false;
        dialogAnimator.SetTrigger("Close");
        defaultSelect.GetComponent<Selectable>().Select();
    }



    public void Disable()
    {
        gameObject.SetActive(false);
    }


}
