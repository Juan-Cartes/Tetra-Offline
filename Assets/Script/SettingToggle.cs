using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingToggle : MonoBehaviour
{
    public string property;
    public bool defaultValue = true;
    private Toggle toggle;

    void Start()
    {   
        toggle = GetComponent<Toggle>();
        toggle.isOn = ConfigFile.Instance.GetBool(property, defaultValue);

    }

    public void OnValueChange()
    {
        ConfigFile.Instance.SetBool(property, toggle.isOn);
        Controls.Instance.UpdateGraphics();
    }


}
