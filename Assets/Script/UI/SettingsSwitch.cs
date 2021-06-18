using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsSwitch : MonoBehaviour
{

    public string property;
    public bool defaultValue;
    public ToggleSwitch toggle;


    void Start()
    {
        toggle.IsOn = ConfigFile.Instance.GetBool(property, defaultValue);    
    }

    public void OnToggle(bool value)
    {
        ConfigFile.Instance.SetBool(property, value);
        Controls.Instance.UpdateGraphics();
    }

}
