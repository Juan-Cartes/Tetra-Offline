using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toast : MonoBehaviour
{

    public bool open;
    public Animation animation;
    public string inAnimation;
    public string outAnimation;

    public void Open()
    {
        open = true;
        animation.Play(inAnimation);
    }

    public void Close()
    {
        open = false;
        animation.Play(outAnimation);
    }

}
