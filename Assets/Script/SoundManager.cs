using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class SoundManager : MonoBehaviour
{

    private static SoundManager _instance;

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singleton = new GameObject("SFX Manager Singleton");
                singleton.AddComponent<AudioSource>();
                _instance = singleton.AddComponent<SoundManager>();
                DontDestroyOnLoad(singleton);
            }

            return _instance;
        }
    }

    public static Dictionary<string, AudioClip> audios;
    public AudioSource source;
    public AudioSource topoutWarning;
    public int renAudioLength;
    public SFXPackInfo info;

    void OnEnable()
    {
        Reload();
    }

    public void Reload()
    {
        string basePath = ConfigFile.GetBasePath();
        source = GetComponent<AudioSource>();
        if (ConfigFile.Instance.GetString("soundpack", "Default").Equals("Default", StringComparison.OrdinalIgnoreCase))
        {
            LoadInternal();
        }
        else
        {
                string path = basePath + "/soundpacks/" + ConfigFile.Instance.GetString("soundpack", "Default") + "/";
                LoadExternal(path);
            
        }
    }
    
    void LoadInternal()
    {
        string info_text = Resources.Load<TextAsset>("SFX/info").text;
        info = JsonUtility.FromJson<SFXPackInfo>(info_text);
        audios = new Dictionary<string, AudioClip>();
        CreateObjects();
        foreach (AudioFileInfo audio in info.sfx)
        {
            LoadInternalAudio(audio.name, "SFX/" + audio.file);
        }

        renAudioLength = info.combos.Length;
        for (int ren = 0; ren < renAudioLength; ren++)
        {
            LoadInternalAudio("combo" + (ren + 1), "SFX/combo/" + info.combos[ren]);
        }

    }

    private void CreateObjects()
    {
        GameObject alarmObj = new GameObject("Alarm");
        alarmObj.AddComponent<AudioSource>();
        GameObject topoutObj = new GameObject("Top Out Warning");
        topoutWarning = topoutObj.AddComponent<AudioSource>();
        topoutWarning.playOnAwake = false;
        topoutWarning.loop = true;
        topoutWarning.priority = 256;
        DontDestroyOnLoad(topoutObj);
        UpdateVolume();
    }

    private void LoadInternalAudio(string name,string path)
    {

        ResourceRequest request = Resources.LoadAsync<AudioClip>(path);
        request.completed += (obj) =>
        {
            AudioClip clip = (AudioClip)request.asset;
            audios.Add(name, clip);
            if (name.Equals("death-warning", StringComparison.OrdinalIgnoreCase))
            {
                topoutWarning.clip = clip;
            }

        };


    }


    void LoadExternal(string path)
    {

        if (!Directory.Exists(path))
        {
            ConfigFile.Instance.SetString("soundpack", "Default");
            LoadInternal();
            return;
        }
        string jsonInfo = File.ReadAllText(path + "info.json");

        info = JsonUtility.FromJson<SFXPackInfo>(jsonInfo);
        CreateObjects();
        audios = new Dictionary<string, AudioClip>();


        foreach (AudioFileInfo audio in info.sfx)
        {
            StartCoroutine(LoadAudio(audio.name, path + audio.file));
        }

        renAudioLength = info.combos.Length;
        for (int ren = 0; ren < renAudioLength; ren++)
        {
            StartCoroutine(LoadAudio("combo" + (ren + 1), path + info.combos[ren]));
        }
    }

    private IEnumerator LoadAudio(string name, string path)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://"+path, AudioType.OGGVORBIS))
        {
            var response = www.SendWebRequest();
            yield return response;

            if (www.isNetworkError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audios.Add(name, clip);
                if (name.Equals("death-warning"))
                {
                    topoutWarning.clip = clip;
                }
            }

        }
    }


    public void UpdateVolume()
    {
        if (source != null)
        {
            source.volume = Controls.Instance.volume_sfx;
            topoutWarning.volume = Controls.Instance.volume_sfx / 2;
        }
    }

    public void PlayAudio(string audio)
    {
        if (source == null) return;
        if (!audios.ContainsKey(audio))
        {
            return;
        }
        if (SoundManager.audios[audio] == null) return;
        source.PlayOneShot(SoundManager.audios[audio]);
    }

    public void PlayTopoutWarning()
    {
        if (topoutWarning == null) return;
        topoutWarning.Play();
    }

    public void StopTopoutWarning()
    {
        if (topoutWarning == null) return;
        topoutWarning.Stop();
    }
    void OnDestroy()
    {
        _instance = null;
    }
}
[Serializable]
public class SFXPackInfo
{
    public AudioFileInfo[] sfx;
    public string[] combos;
}

[Serializable]
public class AudioFileInfo
{
    public string name, file;
}

[Serializable]
public class B2BAudio
{
    public string file;
    public bool playAlone;
}