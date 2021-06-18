using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingButton : MonoBehaviour
{

    public string setting;
    public TextMeshProUGUI text;
    public GameObject panel;


    private string configKey;
    private new bool enabled;
    

    void Start()
    {
        text.text = Controls.Instance.controls[setting].ToString();
    }


    public void SetControls(string configKey)
    {
        enabled = true;
        this.configKey = configKey;
        panel.SetActive(true);
        GameObject eventsystem = GameObject.Find("EventSystem");
        eventsystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }

    void OnGUI()
    {
        if ((Event.current.isKey || Event.current.shift)&& enabled)
        {
            panel.SetActive(false);
            enabled = false;
            if (Event.current.shift)
            {
                Controls.Instance.controls[setting] = KeyCode.LeftShift;
            }
            else
            {
                Controls.Instance.controls[setting] = Event.current.keyCode;
            }
            ConfigFile.Instance.SetString(configKey, Controls.Instance.controls[setting].ToString());
            GameObject eventsystem = GameObject.Find("EventSystem");
            eventsystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(gameObject);
            text.text = Controls.Instance.controls[setting].ToString();
        }

    }

}
