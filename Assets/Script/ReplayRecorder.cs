using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReplayRecorder : MonoBehaviour
{

    public ReplayData replay;
    public int currentIndex = 0;
    public float lastMs;
    public bool recordWhenBoardStart = true;
    public bool record = false;
    public Randomizer[] randomizer;
    public GameMusic music;

    void Start()
    {
        int[] seeds = new int[randomizer.Length];
        for (int i = 0; i < randomizer.Length; i++)
        {
            seeds[i] = randomizer[i].seed;
        }

        replay = new ReplayData((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, seeds, randomizer[0].useSprintRandom);

        Board board = randomizer[0].board;

        replay.lineClearGoal = board.lineClearGoal;
        replay.timeLimit = board.timeLimit;
        replay.force20G = board.force20G;
        replay.useMasterLevels = board.useMasterLevels;
        replay.shouldLevelUp = board.shouldLevelUp;

        replay.musicAuto = music.auto;
        replay.musicId = music.id;
        

    }

    public void StartRecord()
    {
        record = true;
    }


    //Note: This update should happen BEFORE Board (otherwise it might not work as intended), Autoshift and Mino scripts. Careful changing Script Execution Order.
    void Update()
    {
        if (record)
        {
            if (!randomizer[0].board.paused)
            {
                lastMs += Time.deltaTime * 1000;
                replay.frameData.Add(new FrameData(lastMs));
                currentIndex = replay.frameData.Count - 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StopRecord();
        }
    }

    void LateUpdate()
    {
        if (record && replay.frameData.Count != 0)
        {
            if (replay.frameData[currentIndex].inputs.Count == 0)
            {
                replay.frameData.RemoveAt(currentIndex);
                currentIndex = replay.frameData.Count - 1;
            }
        }

    }

    public void PauseRecord()
    {
        record = false;
    }
    public void UnpauseRecord()
    {
        record = true;
    }

    public void AddInputToFrameData(int player, string action)
    {
        //Debug.Log("Added " + action);
        if(replay.frameData.Count == 0 || currentIndex > replay.frameData.Count - 1)
        {
            replay.frameData.Add(new FrameData(lastMs));
        }
        replay.frameData[currentIndex].players.Add(player);
        replay.frameData[currentIndex].inputs.Add(action);
    }

    public void StopRecord()
    {
        AddInputToFrameData(0, "end");
        replay.time = randomizer[0].board.elapsedTime;
        record = false;
    }

    public void Save()
    {

        string basePath = ConfigFile.GetBasePath() + "/replays";
        Directory.CreateDirectory(basePath);
        string file = (Directory.GetFiles(basePath).Length + 1) + ".bin";

        string path = basePath + "/" + file;
        FileStream fs = File.Create(path);
        BinaryWriter writer = new BinaryWriter(fs);
        replay.Serialize(writer);
        writer.Flush();
        writer.Close();
        fs.Close();
    }

}
[Serializable]
public class ReplayData
{
    public long creationDate;
    public float time;
    public int lineClearGoal;
    public int timeLimit;
    public bool shouldLevelUp;
    public string musicAuto;
    public string musicId;
    public bool force20G;
    public bool useMasterLevels;
    public int[] seeds;
    public bool sprintRandomizer;

    public List<FrameData> frameData;

    public ReplayData()
    {
        this.frameData = new List<FrameData>();
    }

    public ReplayData(long creationDate, int[] seeds, bool sprintRandomizer)
    {
        this.creationDate = creationDate;
        this.seeds = seeds;
        this.sprintRandomizer = sprintRandomizer;
        this.frameData = new List<FrameData>();
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(creationDate);
        writer.Write(time);
        writer.Write(lineClearGoal);
        writer.Write(timeLimit);
        writer.Write(musicAuto);
        writer.Write(musicId);
        writer.Write(force20G);
        writer.Write(useMasterLevels);
        writer.Write(shouldLevelUp);

        writer.Write(seeds.Length);
        for (int i = 0; i < seeds.Length; i++)
        {
            writer.Write(seeds[i]);
        }
        writer.Write(sprintRandomizer);
        writer.Write(frameData.Count);
        for (int i = 0; i < frameData.Count; i++)
        {
            frameData[i].Serialize(writer);
        }
    }

    public static ReplayData Deserialize(BinaryReader reader)
    {

        ReplayData data = new ReplayData();
        data.creationDate = reader.ReadInt64();
        data.time = reader.ReadSingle();
        data.lineClearGoal = reader.ReadInt32();
        data.timeLimit = reader.ReadInt32();
        data.musicAuto = reader.ReadString();
        data.musicId = reader.ReadString();
        data.force20G = reader.ReadBoolean();
        data.useMasterLevels = reader.ReadBoolean();
        data.shouldLevelUp = reader.ReadBoolean();

        int seedsLength = reader.ReadInt32();
        data.seeds = new int[seedsLength];
        for (int i = 0; i < seedsLength; i++)
        {
            data.seeds[i] = reader.ReadInt32();
        }
        data.sprintRandomizer = reader.ReadBoolean();
        int framesLength = reader.ReadInt32();
        for (int i = 0; i < framesLength; i++)
        {
            data.frameData.Add(FrameData.Deserialize(reader));
        }

        return data;
    }

}

[Serializable]
public class FrameData
{
    public float millisecond;
    public List<int> players;
    public List<string> inputs;

    public FrameData()
    {
        players = new List<int>();
        inputs = new List<string>();
    }

    public FrameData(float millisecond)
    {
        this.millisecond = millisecond;
        players = new List<int>();
        inputs = new List<string>();
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(millisecond);
        writer.Write(inputs.Count);
        for (int i = 0; i < inputs.Count; i++)
        {
            writer.Write(players[i]);
            writer.Write(inputs[i]);
        }
    }

    public static FrameData Deserialize(BinaryReader reader)
    {
        FrameData data = new FrameData();
        data.millisecond = reader.ReadSingle();
        int inputLength = reader.ReadInt32();
        for (int i = 0; i < inputLength; i++)
        {
            data.players.Add(reader.ReadInt32());
            data.inputs.Add(reader.ReadString());
        }
        return data;
    }

}
