using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/**
* <summary>
* Workaround to fix issue where the scroll rect would not scroll for off-screen items.
* </summary>
* */
public class ScrollviewController : MonoBehaviour
{

    private ScrollRect scrollRect;
    public RectTransform contentPanel;

    public float offset;

    private void Awake()
    {

        foreach (Selectable child in contentPanel.gameObject.GetComponentsInChildren<Selectable>())
        {
            child.gameObject.AddComponent<ScrollItem>().controller = this;
        }
        
    }

    void OnEnable()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    public void SnapTo(RectTransform target, bool up)
    {
        float panelX = contentPanel.anchoredPosition.x;
        contentPanel.anchoredPosition =
            (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position)
            - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

        contentPanel.anchoredPosition = new Vector2(panelX, contentPanel.anchoredPosition.y - (up ? offset : 0));
    }

}
