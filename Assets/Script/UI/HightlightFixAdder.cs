using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HightlightFixAdder : MonoBehaviour
{
    void Start()
    {
        Selectable[] selectables = Resources.FindObjectsOfTypeAll<Selectable>();

        foreach(Selectable selectable in selectables)
        {
            if (selectable.gameObject.GetComponent<HighlightFix>() == null)
            {
                selectable.gameObject.AddComponent<HighlightFix>();
            }
        }

    }
}
