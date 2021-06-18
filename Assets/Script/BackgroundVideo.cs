using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class BackgroundVideo : MonoBehaviour
{
    private VideoPlayer player;

    void Start()
    {
        player = GetComponent<VideoPlayer>();
        if (!ConfigFile.Instance.GetBool("background", true))
        {
            player.Stop();
            player.gameObject.SetActive(false);
        }


    }

    public void UpdateVisibility()
    {
        if (!ConfigFile.Instance.GetBool("background", true))
        {
            player.Stop();
            player.gameObject.SetActive(false);
        }
        else
        {
            player.gameObject.SetActive(true);
            player.Play();
        }
    }
}
