using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreScript : MonoBehaviour
{

    public TextMeshProUGUI highMarathonScore;
    public TextMeshProUGUI bestSprintTime;
    public TextMeshProUGUI highUltraScore;

    public TextMeshProUGUI uploadingHighScore;
    public GameObject gameOverMenu;
    private ReplayRecorder currentReplay;
    private string currentFilename;

    void Start()
    {


        if (highMarathonScore != null)
        {
            highMarathonScore.text = "<sprite=0>" + ConfigFile.Instance.GetInt("highscore_marathon", 0).ToString();
        }

        if (highUltraScore != null) {

            highUltraScore.text = "<sprite=0>" + ConfigFile.Instance.GetInt("highscore_ultra", 0).ToString();
        }

        if (highMarathonScore != null)
        {
            highMarathonScore.text = "<sprite=0>" + ConfigFile.Instance.GetInt("highscore_marathon", 0).ToString();
        }

        if (bestSprintTime != null)
        {
            TimeSpan t = TimeSpan.FromSeconds(ConfigFile.Instance.GetFloat("besttime_sprint", 0));

            string time = string.Format("{0:D2}:{1:D2}.{2:D3}",
                t.Minutes,
                t.Seconds,
                t.Milliseconds
            );
            bestSprintTime.text = "<sprite=0>" + time;
        }

    }

    public void UpdateMarathonScore(int newScore, ReplayRecorder recorder)
    {
        ConfigFile.Instance.SetInt("highscore_marathon", newScore);

        
        highMarathonScore.text = "<sprite=0>" + newScore.ToString();
    }

    public void UpdateSprintScore(float newTime, ReplayRecorder recorder)
    {
        ConfigFile.Instance.SetFloat("besttime_sprint", newTime);

       
        TimeSpan t = TimeSpan.FromSeconds(newTime);

        string time = string.Format("{0:D2}:{1:D2}.{2:D3}",
            t.Minutes,
            t.Seconds,
            t.Milliseconds
        );
        bestSprintTime.text = "<sprite=0>" + time;
    }

    public void UpdateUltraScore(int score, ReplayRecorder recorder)
    {
        ConfigFile.Instance.SetInt("highscore_ultra", score);

        //Leaderboard
        currentReplay = recorder;
        
        highUltraScore.text = "<sprite=0>" + score.ToString();

    }


}
