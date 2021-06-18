using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusic : MonoBehaviour
{

    public string auto;
    public string id;

    void Start()
    {

        string preference = ConfigFile.Instance.GetString(id, "Auto");
        if (preference.Equals("Auto"))
        {
            MusicManager.ChangeMusic(auto);
        }else if (preference.Equals("Random"))
        {
            int selected = Random.Range(1, BGMSelector.bgms.Count - 1);
            MusicManager.ChangeMusic(BGMSelector.bgms[selected]);
        }
        else
        {
            MusicManager.ChangeMusic(preference);
        }

        
    }

}
