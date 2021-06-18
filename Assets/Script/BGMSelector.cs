
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BGMSelector : MonoBehaviour
{

    public static List<string> bgms = new List<string>() { "Auto", "Falling Blocks", "Classic", "Time To Battle", "Step By Step", "Sprint", "Random" };

    private TMP_Dropdown dropdown;
    public string id;

    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.AddOptions(bgms);

        dropdown.value = bgms.IndexOf(ConfigFile.Instance.GetString(id, "Auto"));

    }

    public void OnValueChanged(int index)
    {
        ConfigFile.Instance.SetString(id, bgms[index]);
    }

}
