using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{

    public Dictionary<string, KeyCode> controls = new Dictionary<string, KeyCode>();

    private static Controls _instance;
    
    public static Controls Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject singleton = new GameObject("Controls Singleton");
                _instance = singleton.AddComponent<Controls>();
            }

            return _instance;
        }
    }

    public float volume_music { get; set; }
    public float volume_sfx { get; set; }

    void Awake()
    {
        controls.Add("key_left", (KeyCode) System.Enum.Parse(typeof(KeyCode), ConfigFile.Instance.GetString("key_left", "LeftArrow")));
        controls.Add("key_right", (KeyCode)System.Enum.Parse(typeof(KeyCode), ConfigFile.Instance.GetString("key_right", "RightArrow")));
        controls.Add("softdrop", (KeyCode)System.Enum.Parse(typeof(KeyCode), ConfigFile.Instance.GetString("softdrop", "DownArrow")));
        controls.Add("rotate_right", (KeyCode)System.Enum.Parse(typeof(KeyCode), ConfigFile.Instance.GetString("rotate_right", "UpArrow")));
        controls.Add("rotate_left", (KeyCode)System.Enum.Parse(typeof(KeyCode), ConfigFile.Instance.GetString("rotate_left", "Z")));
        controls.Add("harddrop", (KeyCode)System.Enum.Parse(typeof(KeyCode), ConfigFile.Instance.GetString("harddrop", "Space")));
        controls.Add("hold", (KeyCode)System.Enum.Parse(typeof(KeyCode), ConfigFile.Instance.GetString("hold", "C")));
        controls.Add("retry", (KeyCode)System.Enum.Parse(typeof(KeyCode), ConfigFile.Instance.GetString("retry", "R")));
        controls.Add("pause", (KeyCode)System.Enum.Parse(typeof(KeyCode), ConfigFile.Instance.GetString("pause", "Escape")));

        volume_music = ConfigFile.Instance.GetFloat("volume_music", .5f);
        volume_sfx = ConfigFile.Instance.GetFloat("volume_sfx", .5f);

        UpdateGraphics();

    }


    public void UpdateGraphics()
    {
        Screen.fullScreen = ConfigFile.Instance.GetBool("fullscreen", true);
        
        QualitySettings.vSyncCount = ConfigFile.Instance.GetBool("vsync", false) ? 1 : 0;
        int frameLimit = ConfigFile.Instance.GetInt("maxfps", 300);
        if (frameLimit == 1000)
        {
            Application.targetFrameRate = -1;
        }
        else
        {
            Application.targetFrameRate = ConfigFile.Instance.GetInt("maxfps", 300);
        }
    }



}
