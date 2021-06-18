using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HighlightFix : MonoBehaviour, IPointerEnterHandler
{

    private Selectable selectable;

    void Start()
    {
        selectable = GetComponent<Selectable>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectable != null && selectable.interactable)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

}
