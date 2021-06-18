using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinDropdown : MonoBehaviour
{

    public TMP_Dropdown dropdown;

    private List<string> options = new List<string>();
    private List<string> workshopItems = new List<string>();
    private List<ulong> workshopItemsIds = new List<ulong>();


    void Awake()
    {

        foreach (string s in SkinManager.bundledSkins)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = s });
            options.Add(s);
        }

        if(!Directory.Exists(ConfigFile.GetBasePath() + "/skins/"))
        {
            Directory.CreateDirectory(ConfigFile.GetBasePath() + "/skins/");
        }

        string[] localSkins = Directory.GetDirectories(ConfigFile.GetBasePath() + "/skins/");

        foreach (string skin in localSkins)
        {
            string skinName = Path.GetFileNameWithoutExtension(skin);
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = skinName });
            options.Add(skinName);
        }
        
        int index = options.IndexOf(ConfigFile.Instance.GetString("skin", "Default"));
        dropdown.SetValueWithoutNotify(Mathf.Max(index, 0));
        
    }

    public void OnValueChanged(int index)
    {
        string value = dropdown.options[index].text;
        if (options.Contains(value))
        {
            ConfigFile.Instance.SetString("skin", value);
            ConfigFile.Instance.SetBool("skin_workshop", false);
            SkinManager.Instance.LoadSkin(value);
        }else if (workshopItems.Contains(value))
        {
            int workshopIndex = index - options.Count;
            string id = workshopItemsIds[workshopIndex].ToString();
            ConfigFile.Instance.SetBool("skin_workshop", true);
            ConfigFile.Instance.SetString("skin", id);
            SkinManager.Instance.LoadSkin(id);
        }

        //ConfigFile.Instance.SetString("skin", dropdown.options[index].text);
        //SkinManager.Instance.LoadSkin(dropdown.options[index].text);

    }

}
