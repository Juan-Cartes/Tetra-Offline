using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public string bgm;

    void Start()
    {
        MusicManager.StopDanger();
        SoundManager.Instance.StopTopoutWarning();
        if(bgm.Equals("Main Menu", System.StringComparison.OrdinalIgnoreCase))
        {
            if(MusicManager.currentlyPlayingClip.Equals("menu-first", System.StringComparison.InvariantCultureIgnoreCase)
                || MusicManager.currentlyPlayingClip.Equals("menu-reprise", System.StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }
        }
        MusicManager.ChangeMusic(bgm);
    }

}
