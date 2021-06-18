using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableRect : Selectable
{

    private Outline outline;

    protected override void Awake()
    {
        base.Awake();
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        outline.enabled = true;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        outline.enabled = false;
    }

}
