using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GarbageQueue : MonoBehaviour
{

    public int incomingGarbage = 0;

    public List<GarbageQueueItem> onQueue;
    private List<int> toRemove = new List<int>();
    public float TimerLimit = .6f;

    public int garbageCap = 8;

    public GameObject container;

    public Image[] squares;
    private int garbageDisplayCap;

    public Skin skin;


    void Start()
    {
        onQueue = new List<GarbageQueueItem>();
        garbageDisplayCap = squares.Length;
    }

    public void AddToQueue(int lines, Board board)
    {
        GarbageQueueItem item = new GarbageQueueItem()
        {
            amount = lines
        };
        onQueue.Add(item);
        RenderGarbage();

    }

    public void ReleaseGarbage(Board board)
    {
        if (board.remote) return;
        if (incomingGarbage == 0) return;
        if (incomingGarbage > 40)
        {
            incomingGarbage = 40;
        }
        int garbageToRelease = incomingGarbage;
        if(incomingGarbage > garbageCap)
        {
            garbageToRelease = garbageCap;
        }

        SoundManager.Instance.PlayAudio("garbage-spawn");
        board.PushUp(garbageToRelease);
        for (int i = 0; i < board.lineClears.Count; i++)
        {
            board.lineClears[i] = board.lineClears[i] - garbageToRelease;
        }
        for(int i = 0; i < board.flashY.Count; i++)
        {
            board.flashY[i] -= garbageToRelease;
        }



        int cleanHole = Random.Range(0, 10);
        int tmp = garbageToRelease;
        float cleanHoleChange = .8f;

        for (int y = 40 - tmp; y < 40; y++)
        {
            if(Random.Range(0f, 1f) >= cleanHoleChange)
            {
                cleanHole = Random.Range(0, 10);
            }

            for(int x = 0; x < 10; x++)
            {
                if(x == cleanHole)
                {
                    board.matrix[y, x] = 0;
                }
                else
                {
                    board.matrix[y, x] = Mino.ID_GARBAGE;
                }
            }
            incomingGarbage--;

        }



        RenderGarbage();
    }






    /**
     * <param name="counter">Amount of lines the player is sending</param>
     * */
    public GarbageCounterResult Counter(int counter)
    {

        GarbageCounterResult result = new GarbageCounterResult();

        int linesSentAfterCounter = counter;
        toRemove.Clear();
        for(int i = 0; i < onQueue.Count; i++)
        {
            if(linesSentAfterCounter < 0)
            {
                break;
            }
            int rest = onQueue[i].amount - linesSentAfterCounter;
            linesSentAfterCounter -= onQueue[i].amount;
            if (rest <= 0)
            {
                toRemove.Add(i);
            }
            else
            {
                onQueue[i].amount = rest;
            }


        }

        foreach(int index in toRemove)
        {
            onQueue.RemoveAt(index);
        }

        int tmp = Mathf.Max(linesSentAfterCounter, 0);
        linesSentAfterCounter -= incomingGarbage;
        incomingGarbage -= tmp;
        linesSentAfterCounter = Mathf.Max(linesSentAfterCounter, 0);

        if(incomingGarbage > 0 && linesSentAfterCounter > 0)
        {
            result.counter = true;
        }

        if(incomingGarbage < 0)
        {
            incomingGarbage = 0;
        }



        if(linesSentAfterCounter < 0)
        {
            linesSentAfterCounter = 0;
        }
        result.linesLeft = linesSentAfterCounter;


        RenderGarbage();

        return result;
    }

    private int GetOnQueueLines()
    {
        int onQueueLines = 0;
        foreach(GarbageQueueItem item in onQueue)
        {
            onQueueLines += item.amount;
        }
        return onQueueLines;
    }

    void RenderGarbage()
    {

        for (int i = 0; i < garbageDisplayCap; i++)
        {
            squares[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < incomingGarbage; i++)
        {
            squares[i % garbageDisplayCap].gameObject.SetActive(true);
            squares[i % garbageDisplayCap].color = Color.red;
        }

        int index = incomingGarbage - 1;
        for (int i = 0; i < onQueue.Count; i++)
        {
            for (int i2 = 0; i2 < onQueue[i].amount; i2++)
            {
                index++;
                if (index >= garbageDisplayCap) return;
                squares[index].gameObject.SetActive(true);
                squares[index].color = Color.white;
            }
        }

    }

    void Update()
    {
        toRemove.Clear();

        for (int i = 0; i < onQueue.Count; i++)
        {
            onQueue[i].timer += Time.deltaTime;
            if (onQueue[i].timer >= TimerLimit)
            {
                incomingGarbage += onQueue[i].amount;
                toRemove.Add(i);
            }
        }

        foreach (int index in toRemove)
        {
            onQueue.RemoveAt(index);
        }

        if (toRemove.Count != 0)
        {
            RenderGarbage();
        }


    }

    public void ClearQueue()
    {
        incomingGarbage = 0;
        onQueue.Clear();
        RenderGarbage();
    }


}

public class GarbageCounterResult
{
    public bool counter;
    public int linesLeft;
}

public class GarbageQueueItem
{
    public int amount;
    public float timer;
}