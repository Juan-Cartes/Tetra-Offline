using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyboardSettings : MonoBehaviour
{
    public GameObject parent;
    public GameObject template;

    private readonly Dictionary<string, string> readableNames = new Dictionary<string, string>()
    {
        {
            "key_left", "Piece Shift Left"
        },
        {
            "key_right", "Piece Shift Right"
        },
        {
            "rotate_right", "Rotate Clockwise"
        },
        {
            "rotate_left", "Rotate Counter-clockwise"
        },
        {
            "softdrop", "Fast Fall"
        },
        {
            "harddrop", "Instant Drop"
        },
        {
            "hold", "Hold"
        },
        {
            "retry", "Retry"
        },
        {
            "pause", "Pause"
        }
    };

    private bool changingKey;
    private string currentlyChangingKey;
    private GameObject previouslySelectedGameObject; //Selected game object before the player choose to change a key binding.
    public GameObject keyPrompt;
    public GameObject cancelBtn;

    void OnEnable()
    {
        bool first = true;

        foreach(Transform children in transform)
        {
            GameObject obj = children.gameObject;
            if (obj.name.Equals(template.name))
            {
                continue;
            }

            Destroy(obj);
        }

        foreach(string key in readableNames.Keys)
        {
            GameObject obj = Instantiate(template, parent.transform);
            obj.name = key;
            obj.transform.Find("Key").GetComponent<TextMeshProUGUI>().text = readableNames[key];
            obj.transform.Find("Button").Find("Value").GetComponent<TextMeshProUGUI>().text = Controls.Instance.controls[key].ToString();
            obj.SetActive(true);
            first = false;
        }

    }


    public void ChangeKey(GameObject parent)
    {
        currentlyChangingKey = parent.name;
        changingKey = true;
        keyPrompt.SetActive(true);
    }

    public void Cancel()
    {
        changingKey = false;
        keyPrompt.SetActive(false);
    }

    private void OnGUI()
    {
        if ((Event.current.isKey || Event.current.shift || Event.current.alt) && changingKey)
        {
            if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.None) return;
            keyPrompt.SetActive(false);
            changingKey = false;
            if (Event.current.shift)
            {
                Controls.Instance.controls[currentlyChangingKey] = KeyCode.LeftShift;
            }
            else if (Event.current.alt)
            {
                Controls.Instance.controls[currentlyChangingKey] = KeyCode.LeftAlt;
            }
            else
            {
                Controls.Instance.controls[currentlyChangingKey] = Event.current.keyCode;
            }
            ConfigFile.Instance.SetString(currentlyChangingKey, Controls.Instance.controls[currentlyChangingKey].ToString());
            parent.transform.Find(currentlyChangingKey).Find("Button").Find("Value")
                .GetComponent<TextMeshProUGUI>().text = Controls.Instance.controls[currentlyChangingKey].ToString();
        }
    }

}
