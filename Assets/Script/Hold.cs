using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hold : MonoBehaviour
{

    private int heldMino = -1;
    private bool isLocked;

    public Board board;

    public GameObject holdBox;
    public Randomizer randomizer;

    public bool ihsIsActive;

    private List<GameObject> cache = new List<GameObject>(); //Contains the current hold piece's gameobject instance

    private Skin skin;

    void Start()
    {
        skin = SkinManager.Instance.currentSkin;
    }

    public void Hide()
    {
        holdBox.SetActive(false);
    }

    public void Show()
    {
        holdBox.SetActive(true);
    }

    public void HoldPiece(int activePiece, bool useIhsSound)
    {
        Mino mino = GetComponent<Mino>();

        if (mino == null || mino.lockedDown || !mino.enabled)
        {
            if (ConfigFile.Instance.GetBool("ihs", true))
            {
                ihsIsActive = true;
                SoundManager.Instance.PlayAudio("buffer-set");
            }
            return;
        }

        if (isLocked) return;
        if (heldMino != -1)
        {
            mino.id = heldMino;
            mino.board = board;
            mino.remote = board.remote;
            mino.Init();
        }
        else
        {
            if (!board.replay)
            {
                GetComponent<Board>().NextPiece();
            }
        }
        heldMino = activePiece;
        isLocked = true;

        SoundManager.Instance.PlayAudio(useIhsSound ? "buffered-hold" : "hold");
        Render();

    }

    private void Render()
    {

        int[][] shape = Pieces.GetDefaultShapeFromId(heldMino);


        Clear();

        for (int y = 0; y < shape.Length; y++)
        {
            for (int x = 0; x < shape[y].Length; x++)
            {
                if (shape[y][x] != 0)
                {
                    DrawBlockAt(x, y, shape[y][x]);
                }
            }
        }



    }

    private void Clear()
    {

        foreach (Transform child in holdBox.transform)
        {
            Destroy(child.gameObject);
        }

        cache.Clear();

    }

    public void ResetHold()
    {

        Clear();
        heldMino = -1;
        ihsIsActive = false;
        isLocked = false;

    }

    private void DrawBlockAt(int x, int y, int block)
    {

        float offsetX = 0;
        float offsetY = 0;
        PieceOffsetInformation[] information = skin.HoldData.offsets;
        for (int i = 0; i < information.Length; i++)
        {
            if (Pieces.GetIdFromString(information[i].piece) == block)
            {
                offsetX = information[i].x;
                offsetY = information[i].y;
                break;
            }
        }


        GameObject instance = Instantiate(randomizer.GetMino(block));

        ((RectTransform)instance.transform).sizeDelta = new Vector2(board.width_mino * skin.HoldData.multiplier, board.height_mino * skin.HoldData.multiplier);
        instance.transform.SetParent(holdBox.transform);
        instance.transform.localPosition = new Vector3(
            ToUnitsX(x, offsetX),
            ToUnitsY(y, offsetY)
         );
        instance.transform.localScale = Vector3.one;

        if (isLocked)
        {
            instance.GetComponent<Image>().color = Color.gray;
        }

        instance.SetActive(true);

        cache.Add(instance);

    }

    public void SetLocked(bool locked)
    {
        this.isLocked = locked;
        if (!locked)
        {
            foreach (GameObject obj in cache)
            {
                obj.GetComponent<Image>().color = Color.white;
            }
        }
    }

    public void SetHeldMino(int mino)
    {
        if (mino != heldMino)
        {
            this.heldMino = mino;
            Render();
        }
    }

    public int GetHeldMino()
    {
        return heldMino;
    }

    public float ToUnitsX(int x, float offset)
    {
        return x * (board.width_mino * skin.HoldData.multiplier) + (offset * (board.width_mino * skin.HoldData.multiplier));
    }

    public float ToUnitsY(int y, float offset)
    {
        return 0 - y * (board.height_mino * skin.HoldData.multiplier) + (offset * (board.height_mino * skin.HoldData.multiplier));
    }


}
