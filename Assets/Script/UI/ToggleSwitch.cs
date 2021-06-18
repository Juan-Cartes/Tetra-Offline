using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour
{
    private bool _ison = false;
    public bool IsOn
    {
        get => _ison;
        set
        {
            Toggle(value);
            _ison = value;
        }
    }

    [SerializeField]
    private RectTransform indicator = null;

    [SerializeField]
    private RectTransform offX = null;
    [SerializeField]
    private RectTransform onX = null;

    [SerializeField]
    private float animationTime = 0;

    public BoolEvent valueChanged; 
    void Start()
    {
    }

    public void Toggle(bool value)
    {
        if (value != IsOn)
        {
            _ison = value;

            if (value)
            {
                indicator.LeanMoveLocalX(onX.localPosition.x, animationTime);
            }
            else
            {
                indicator.LeanMoveLocalX(offX.localPosition.x, animationTime);
            }
            valueChanged?.Invoke(value);
        }
    }

    public void Toggle()
    {
        Toggle(!IsOn);
    }

}
[Serializable]
public class BoolEvent : UnityEvent<bool> { }