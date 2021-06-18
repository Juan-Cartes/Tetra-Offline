using System;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public static string currentlyPlayingClip = string.Empty;
    private static float timeBeforeDanger;
    private static AudioClip clipBeforeDanger;

    private static bool mainMenuFirstTime = false;

    
    public static void ChangeMusic(string clip)
    {
        float volume = ConfigFile.Instance.GetFloat("volume_music", .5f);
        MusicPlayer.Instance.source.volume = volume;
        if (MusicPlayer.Instance.source.clip != null &&
            clip.Equals(MusicPlayer.Instance.source.clip.name, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        
        if (clip.Equals("Main Menu", StringComparison.OrdinalIgnoreCase))
        {
            if (!mainMenuFirstTime)
            {
                clip = "menu-first";
                mainMenuFirstTime = true;
            }
            else
            {
                clip = "menu-reprise";
            }
        }
        ResourceRequest request = Resources.LoadAsync<AudioClip>("Music/" + clip);
        request.completed += (obj) =>
        {
            AudioClip audioClip = (AudioClip)request.asset;
            currentlyPlayingClip = clip;
            MusicPlayer.Instance.source.clip = audioClip;
            MusicPlayer.Instance.source.Play();
        };
    }

    public static void StartDanger()
    {
        
        AudioClip dangerTheme = Resources.Load<AudioClip>("Music/danger");
        timeBeforeDanger = MusicPlayer.Instance.source.time;
        MusicPlayer.Instance.source.clip = dangerTheme;
        MusicPlayer.Instance.source.Play();
        
    }

    public static void StopDanger()
    {
        if (clipBeforeDanger == null) return;
        if (clipBeforeDanger.name.Equals(MusicPlayer.Instance.source.clip.name, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        MusicPlayer.Instance.source.clip = clipBeforeDanger;
        MusicPlayer.Instance.source.time = timeBeforeDanger;
        MusicPlayer.Instance.source.Play();
    }


    public static void UpdateVolume()
    {
        float volume = ConfigFile.Instance.GetFloat("volume_music", .5f);
        MusicPlayer.Instance.source.volume = volume;
    }

}

class MusicPlayer : MonoBehaviour
{

    private static MusicPlayer _instance;
    public static MusicPlayer Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject obj = new GameObject();
                DontDestroyOnLoad(obj);
                _instance = obj.AddComponent<MusicPlayer>();
                _instance.source = obj.AddComponent<AudioSource>();
                _instance.source.loop = true;
            }
            return _instance;
        }
    }

    public AudioSource source;

}