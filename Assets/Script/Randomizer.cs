using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Randomizer : MonoBehaviour
{

    public List<int> queue = new List<int>(5);
    private List<int> lastRenderedQueue = new List<int>(5);
    private List<int> pool;

    public int seed = -1;

    public SkinSceneScript skinScene;
    private GameObject[] nextBoxes;

    public Board board;

    public bool initialized = false;

    public BlockImagePool mino_z;
    public BlockImagePool mino_t;
    public BlockImagePool mino_s;
    public BlockImagePool mino_o;
    public BlockImagePool mino_l;
    public BlockImagePool mino_j;
    public BlockImagePool mino_i;

    private Skin skin;

    public bool useSprintRandom;

    public void OnEnable()
    {
        if (seed != -1)
        {
            Random.InitState(seed);
        }
        else
        {
            seed = (int)System.DateTime.Now.Ticks;
            Random.InitState(seed);
        }

        skin = SkinManager.Instance.currentSkin;

        nextBoxes = skinScene.nextBoxes;
        if (mino_z == null) //Just check for mino_z, there's no need to check the other variables since if mino_z is initilized, so is everything else.
        {
            mino_z = new BlockImagePool(skin.GetMino("z"), 8); //Must be 8 because a piece can appear twice in the next queue. 8 stored gameobjects should be enough.
            mino_t = new BlockImagePool(skin.GetMino("t"), 8);
            mino_s = new BlockImagePool(skin.GetMino("s"), 8);
            mino_o = new BlockImagePool(skin.GetMino("o"), 8);
            mino_l = new BlockImagePool(skin.GetMino("l"), 8);
            mino_j = new BlockImagePool(skin.GetMino("j"), 8);
            mino_i = new BlockImagePool(skin.GetMino("i"), 8);
        }
        Clear();

        pool = new List<int>();
        pool.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7 });

        if (useSprintRandom)
        {
            pool.Remove(Mino.ID_S);
            pool.Remove(Mino.ID_Z);
            pool.Remove(Mino.ID_O);
        }
        
        queue.Clear();
        for (int i = 0; i < 5; i++)
        {
            if(i == 1 && useSprintRandom)
            {
                pool.Add(Mino.ID_S);
                pool.Add(Mino.ID_Z);
                pool.Add(Mino.ID_O);
            }
            queue.Add(Generate());
        }
        ForceRerender();
        initialized = true;
    }

    public void Hide()
    {
        foreach(GameObject gobject in nextBoxes)
        {
            gobject.SetActive(false);
        }
    }

    public void Show()
    {
        foreach (GameObject gobject in nextBoxes)
        {
            gobject.SetActive(true);
        }
    }


    int Generate()
    {
        if (pool.Count == 0)
        {
            pool.AddRange(new int[] { 1, 2, 3, 4, 5, 6, 7 });
        }

        int selectedMino = Random.Range(0, pool.Count);
        int mino = pool[selectedMino];
        pool.RemoveAt(selectedMino);
        return mino;
    }

    public int Next()
    {
        int mino = queue[0];
        queue.RemoveAt(0);
        queue.Add(Generate());
        return mino;
    }

    void Update()
    {
        if (!lastRenderedQueue.SequenceEqual(queue))
        {
            Clear();
            for (int box = 0; box < Mathf.Min(skin.NextBoxesData.Count, 5); box++)
            {
                RenderShape(Pieces.GetDefaultShapeFromId(queue[box]), box);
            }
            lastRenderedQueue = new List<int>(queue);
        }

    }

    public void ForceRerender()
    {
        lastRenderedQueue.Clear();
    }

    private void RenderShape(int[][] shape, int box)
    {

        for (int y = 0; y < shape.Length; y++)
        {
            for (int x = 0; x < shape[y].Length; x++)
            {
                if (shape[y][x] != 0)
                {
                    float widthMino = board.width_mino * skin.NextBoxesData[box].multiplier;
                    float heightMino = board.width_mino * skin.NextBoxesData[box].multiplier;
                    GameObject instance = GetMino(shape[y][x]);

                    float offsetx = 0;
                    float offsety = 0;
                    PieceOffsetInformation[] information = skin.NextBoxesData[box].offsets;
                    for (int i = 0; i < information.Length; i++)
                    {
                        if (Pieces.GetIdFromString(information[i].piece) == shape[y][x])
                        {
                            offsetx = information[i].x;
                            offsety = information[i].y;
                            break;
                        }
                    }

                    ((RectTransform)instance.transform).sizeDelta = new Vector2(widthMino, heightMino);
                    instance.transform.SetParent(nextBoxes[box].transform);
                    instance.transform.localScale = Vector3.one;
                    instance.transform.localPosition = new Vector3(
                        ToUnitsX(x, offsetx, box),
                        ToUnitsY(y, offsety, box));
                    instance.SetActive(true);
                }
            }
        }



    }

    private void Clear()
    {

        mino_z.DisableAll();
        mino_t.DisableAll();
        mino_s.DisableAll();
        mino_o.DisableAll();
        mino_l.DisableAll();
        mino_j.DisableAll();
        mino_i.DisableAll();

    }

    public float ToUnitsX(int x, float offset, int box)
    {
        return x * (board.width_mino * skin.NextBoxesData[box].multiplier) + (offset * (board.width_mino * skin.NextBoxesData[box].multiplier));
    }

    public float ToUnitsY(int y, float offset, int box)
    {
        return 0 - y * (board.height_mino * skin.NextBoxesData[box].multiplier) + (offset * (board.height_mino * skin.NextBoxesData[box].multiplier));
    }


    public GameObject GetMino(int id)
    {

        switch (id)
        {
            case 1:
                return mino_i.Next();
            case 2:
                return mino_o.Next();
            case 3:
                return mino_j.Next();
            case 4:
                return mino_l.Next();
            case 5:
                return mino_z.Next();
            case 6:
                return mino_s.Next();
            case 7:
                return mino_t.Next();
        }
        return null;

    }

}
