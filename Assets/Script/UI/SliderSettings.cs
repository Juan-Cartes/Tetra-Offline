using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderSettings : MonoBehaviour
{

    public Slider slider;
    public float defaultValue;
    public string property;
    public TextMeshProUGUI currentValue;

    void Start()
    {
        slider.SetValueWithoutNotify(ConfigFile.Instance.GetFloat(property, defaultValue));
        if(currentValue != null)
        {
            currentValue.text = slider.value.ToString();
        }
    }

    public void OnValueChange(float value)
    {
        ConfigFile.Instance.SetFloat(property, value);
        if (currentValue != null)
        {
            currentValue.text = value.ToString();
        }

        if (property.Equals("volume_music", System.StringComparison.OrdinalIgnoreCase))
        {
            Controls.Instance.volume_music = value;
            MusicManager.UpdateVolume();
        }
        else if(property.Equals("volume_sfx", System.StringComparison.OrdinalIgnoreCase))
        {
            Controls.Instance.volume_sfx = value;
            SoundManager.Instance.UpdateVolume();
            SoundManager.Instance.PlayAudio("move");
        }
        else if(property.Equals("maxfps", System.StringComparison.OrdinalIgnoreCase))
        {
            if(value == 1000)
            {
                currentValue.text = "Unlimited";
            }
            Controls.Instance.UpdateGraphics();
        }

    }


}
