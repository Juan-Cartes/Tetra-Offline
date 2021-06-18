using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISoundEffect : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlayAudio("change");
    }

    public void OnSelect(BaseEventData eventData)
    {
        SoundManager.Instance.PlayAudio("change");
    }
}
