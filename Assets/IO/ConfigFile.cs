using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class ConfigFile : MonoBehaviour
{

    private static ConfigFile m_instance;

    public static ConfigFile Instance 
    {
        get
        {
            if(m_instance == null)
            {
                GameObject m = new GameObject("Config File Manager");
                DontDestroyOnLoad(m);
                m_instance = m.AddComponent<ConfigFile>();
            }
            return m_instance;
        }
    }

    private static Dictionary<string, System.Object> settings;


    void Awake()
    {
        settings = new Dictionary<string, System.Object>();
        string basePath = GetBasePath();

        string path = @basePath + "/saves/config.cfg";
        Directory.CreateDirectory(basePath + "/saves/");
        if (!File.Exists(path))
        {
            File.Create(path);
            return;
        }

        StreamReader reader = new StreamReader(path);
        string line;

        while((line = reader.ReadLine()) != null)
        {
            string[] props = line.Split('=');
            if (props[0].Equals("skin") || props[0].Equals("soundpack"))
            {
            
                settings.Add(props[0], props[1]);
            }
            else if(props[1].Equals("true", StringComparison.OrdinalIgnoreCase) ||
                props[1].Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                settings.Add(props[0], bool.Parse(props[1]));
            }
            else if(float.TryParse(props[1].Replace(',', '.'), out _)){
                float n= float.Parse(props[1].Replace(',', '.')); //Fuck you .NET.
                settings.Add(props[0], n);

            }
            else
            {
                settings.Add(props[0], props[1]);
            } 
        
        }

        reader.Close();


    }

    public static string GetBasePath()
    {
        string basePath = Application.dataPath;

        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            basePath += "/../..";
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.LinuxPlayer
            || Application.platform == RuntimePlatform.WindowsEditor)
        {
            basePath += "/..";
        }
        return basePath;
    }
    private void Save()
    {
        string basePath = GetBasePath();

        string path = @basePath + "/saves/config.cfg";
        if (!File.Exists(path)) //Failsafe
        {
            File.Create(path);
        }
        File.WriteAllText(path, string.Empty); //Reset the config
        StreamWriter writer = new StreamWriter(path);
        
        foreach(KeyValuePair<string, object> pair in settings)
        {
            if (pair.Value.GetType() == typeof(float))
            {
                float value = (float)pair.Value;
                writer.WriteLine(pair.Key + "=" + value.ToString(CultureInfo.InvariantCulture).Replace(',', '.'));
            }
            else
            {
                writer.WriteLine(pair.Key + "=" + pair.Value.ToString());
            }
        }
        writer.Close();
    }

    void OnApplicationQuit()
    {
        Save();
    }


    public string GetString(string key, string defaultValue)
    {
        if (settings.ContainsKey(key))
        {
            return settings[key].ToString();
        }
        else
        {
            settings.Add(key, defaultValue);
            return defaultValue;
        }
    }
    public float GetFloat(string key, float defaultValue)
    {
        if (settings.ContainsKey(key))
        {
            return (float)settings[key];
        }
        else
        {
            settings.Add(key, defaultValue);
            return defaultValue;
        }
    }

    public int GetInt(string key, int defaultValue)
    {
        return (int)GetFloat(key, defaultValue);
    }

    public bool GetBool(string key, bool defaultValue)
    {
        if (settings.ContainsKey(key))
        {
            return (bool)settings[key];
        }
        else
        {
            settings.Add(key, defaultValue);
            return defaultValue;
        }
    }

    public void SetString(string key, string newValue)
    {
        if (!settings.ContainsKey(key))
        {
            settings.Add(key, newValue);
        }
        else
        {
            settings[key] = newValue;
        }

    }

    public void SetFloat(string key, float newValue)
    {
        if (!settings.ContainsKey(key))
        {
            settings.Add(key, newValue);
        }
        else
        {
            settings[key] = newValue;
        }

    }
    public void SetInt(string key, int newValue)
    {
        if (!settings.ContainsKey(key))
        {
            settings.Add(key, (float)newValue);
        }
        else
        {
            settings[key] = (float)newValue;
        }

    }
    public void SetBool(string key, bool newValue)
    {
        if (!settings.ContainsKey(key))
        {
            settings.Add(key, newValue);
        }
        else
        {
            settings[key] = newValue;
        }

    }
}
