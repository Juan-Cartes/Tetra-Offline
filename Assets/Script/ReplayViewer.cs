using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReplayViewer : MonoBehaviour
{

    public Board[] boards;
    public string path;
    public float multiplier;
    public bool playback = false;
    public bool UGC; //Set to true if the ReplayViewer should read from a file path from ExternalReplayInfo, used for Steam-based replays.

    private ReplayData replay;
    private float currentMs = 0;
    private int currentIndex = 0;
    public GameMusic music;
    public bool finished = false;


    void Awake()
    {

        foreach (Board board in boards)
        {
            board.replay = true;
            board.remote = true;
            board.doGravity = false;
        }

        boards[0].introAnimator.SetTrigger("Start");
        if (UGC)
        {
            ReadReplay(UGCReplay.replayRawBytes);
        }
        else
        {
            ReadFromFile(path);
        }
        foreach (Board board in boards)
        {
            board.lineClearGoal = replay.lineClearGoal;
            board.timeLimit = replay.timeLimit;
            board.force20G = replay.force20G;
            board.useMasterLevels = replay.useMasterLevels;
            board.shouldLevelUp = replay.shouldLevelUp;
        }

        music.auto = replay.musicAuto;
        music.id = replay.musicId;

        for (int i = 0; i < replay.seeds.Length; i++)
        {
            Randomizer randomizer = boards[i].GetComponent<Randomizer>();
            randomizer.useSprintRandom = replay.sprintRandomizer;
            randomizer.seed = replay.seeds[i];
            randomizer.OnEnable();
        }
        

        currentMs = replay.frameData[0].millisecond;

    }

    private void ReadFromFile(string path)
    {
        byte[] data = File.ReadAllBytes(path);
        ReadReplay(data); 
   }

    private void ReadReplay(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            replay = ReplayData.Deserialize(reader);
        }
    }

    void Update()
    {
        if (playback && !boards[0].paused)
        {
            currentMs += Time.deltaTime * 1000 * multiplier;

            for (int i = currentIndex; i < replay.frameData.Count; i++)
            {
                if (replay.frameData[i].millisecond > currentMs)
                {
                    currentIndex = i;
                    break;
                }
                ExecuteFrameData(replay.frameData[i]);
            }
        }


    }

    private void ExecuteFrameData(FrameData data)
    {
        for (int i = 0; i < data.inputs.Count; i++)
        {
            string[] inputData = data.inputs[i].Split(' ');
            int player = data.players[i];
            Mino active = boards[player].GetComponent<Mino>();
            switch (inputData[0].ToLower())
            {
                case "moveleft":
                    {
                        int fromX = int.Parse(inputData[1]);
                        int fromY = int.Parse(inputData[2]);

                        active.x = fromX;
                        active.y = fromY;
                        active.ShiftLeft();
                    }
                    break;
                case "moveright":
                    {

                        int fromX = int.Parse(inputData[1]);
                        int fromY = int.Parse(inputData[2]);

                        active.x = fromX;
                        active.y = fromY;
                        active.ShiftRight();
                    }
                    break;
                case "movedown":
                    {
                        active.y++;
                    }
                    break;
                case "harddrop":
                    {
                        int fromX = int.Parse(inputData[1]);
                        bool sound = bool.Parse(inputData[2]);
                        active.x = fromX;
                        active.HardDrop(sound);
                    }
                    break;
                case "rotate":
                    {
                        int direction = int.Parse(inputData[1]);

                        if ((active != null && (!active.enabled || active.lockedDown) && !boards[player].dead) || inputData.Length == 2)
                        {
                            boards[player].Rotate(direction);
                        }
                        else
                        {
                            int x = int.Parse(inputData[2]);
                            int y = int.Parse(inputData[3]);
                            active.x = x;
                            active.y = y;
                            active.Rotate(direction);
                        }
                    }
                    break;
                case "queue":
                    {
                        List<int> queue = new List<int>();
                        for (int i2 = 1; i2 <= 5; i2++)
                        {
                            queue.Add(int.Parse(inputData[i2]));
                        }
                        boards[player].GetComponent<Randomizer>().queue = queue;
                        boards[player].GetComponent<Randomizer>().ForceRerender();
                    }
                    break;
                case "nextpiece":
                    {
                        boards[player].NextPiece();
                        Mino mino = boards[player].GetComponent<Mino>();
                        if (inputData.Length == 2)
                        {
                            mino.id = int.Parse(inputData[1]);
                            mino.remote = true;
                            mino.softdropping = false;
                            mino.board = boards[player];
                            mino.Init();
                        }
                    }
                    break;
                case "collapse":
                    {
                        boards[player].Collapse();
                    }
                    break;
                case "ground":
                    {
                        int x = int.Parse(inputData[1]);
                        active.x = x;
                        active.y += active.GetGroundDistance();
                    }
                    break;
                case "hold":
                    {
                        boards[player].Hold();
                    }
                    break;
                case "danger":
                    {
                        MusicManager.StartDanger();
                    }
                    break;
                case "stopdanger":
                    {
                        MusicManager.StopDanger();
                    }
                    break;
                case "sfxplayforcelock":
                    {
                        SoundManager.Instance.PlayAudio("forced-lock");
                    }
                    break;
                case "die":
                    {
                        boards[player].Die();
                        boards[player].elapsedTime = replay.time;
                        playback = false;
                    }
                    break;

                case "end":
                    {
                        finished = true;
                        boards[player].elapsedTime = replay.time;
                        playback = false;
                        GameObject eventsystem = GameObject.Find("EventSystem");
                        eventsystem.GetComponent<EventSystem>().SetSelectedGameObject(null);
                        boards[0].paused = false;
                        boards[0].PauseMenu.SetActive(false);
                    }
                    break;
            }

        }
    }

}