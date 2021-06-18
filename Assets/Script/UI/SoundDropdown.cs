using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class SoundDropdown : MonoBehaviour
{

    private TMP_Dropdown dropdown;

    private List<string> options = new List<string>();
    private List<string> workshopItems = new List<string>();
    private List<ulong> workshopItemsIds = new List<ulong>();


    void OnEnable()
    {
        dropdown = GetComponent<TMP_Dropdown>();

        dropdown.options.Add(new TMP_Dropdown.OptionData() { text = "Default" });
        options.Add("Default");

        string basePath = ConfigFile.GetBasePath();
        string path = basePath + "/soundpacks/";
        if (Directory.Exists(path))
        {
            string[] localSkins = Directory.GetDirectories(path);

            foreach (string soundpack in localSkins)
            {
                string name = Path.GetFileNameWithoutExtension(soundpack);
                dropdown.options.Add(new TMP_Dropdown.OptionData() { text = name });
                options.Add(name);
            }
        }

            int index = options.IndexOf(ConfigFile.Instance.GetString("skin", "Default"));
            dropdown.SetValueWithoutNotify(Mathf.Max(index, 0));
        
    }


    public void OnValueChanged(int index)
    {
        string value = dropdown.options[index].text;
    
        if (options.Contains(value))
        {
            ConfigFile.Instance.SetString("soundpack", value);
            ConfigFile.Instance.SetBool("soundpack_workshop", false);
            SoundManager.Instance.Reload();
        }

    }


}
