using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{

    public TextMeshProUGUI text;
    public new Camera camera;
    private RectTransform parent;
    private Vector2 offset = new Vector2();
    private float defaultWidth;
    private float defaultHeight;

    void Awake()
    {
        parent = transform.parent.GetComponent<RectTransform>();

        RectTransform rectTransform = GetComponent<RectTransform>();
        defaultWidth = rectTransform.rect.width;
        defaultHeight = rectTransform.rect.height;
    }

    public void ShowTooltip(string tooltipText)
    {
        gameObject.SetActive(true);
        text.text = tooltipText;
        RectTransform rectTransform = (RectTransform)transform;
        float width = text.preferredWidth + text.margin.x;
        width = Mathf.Min(width, defaultWidth);
        float height = text.preferredHeight;
        height = Mathf.Max(height, defaultHeight);

        rectTransform.sizeDelta = new Vector2(width, height);
        offset.x = width / 2f;
        offset.y = height / 2f;

    }

    public void HideTooltip()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        transform.localPosition = new Vector3(-1000f, -1000f, 0);
    }

    private void OnGUI()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, Input.mousePosition, camera, out pos);
        transform.localPosition = pos + offset;
    }

}
